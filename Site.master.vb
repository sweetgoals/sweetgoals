Imports System.Net
Imports System.Net.Mail
Imports System.IO

Partial Class SiteMaster
    Inherits MasterPage

    Const AntiXsrfTokenKey As String = "__AntiXsrfToken"
    Const AntiXsrfUserNameKey As String = "__AntiXsrfUserName"
    Dim _antiXsrfTokenValue As String

    Protected Sub Page_Init(sender As Object, e As System.EventArgs)
        ' The code below helps to protect against XSRF attacks
        Dim requestCookie As HttpCookie = Request.Cookies(AntiXsrfTokenKey)
        Dim requestCookieGuidValue As Guid
        If ((Not requestCookie Is Nothing) AndAlso Guid.TryParse(requestCookie.Value, requestCookieGuidValue)) Then
            ' Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value
            Page.ViewStateUserKey = _antiXsrfTokenValue
        Else
            ' Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N")
            Page.ViewStateUserKey = _antiXsrfTokenValue

            Dim responseCookie As HttpCookie = New HttpCookie(AntiXsrfTokenKey) With {.HttpOnly = True, .Value = _antiXsrfTokenValue}
            If (FormsAuthentication.RequireSSL And Request.IsSecureConnection) Then
                responseCookie.Secure = True
            End If
            Response.Cookies.Set(responseCookie)
        End If
        AddHandler Page.PreLoad, AddressOf master_Page_PreLoad
    End Sub

    Private Sub master_Page_PreLoad(sender As Object, e As System.EventArgs)
        If (Not IsPostBack) Then
            ' Set Anti-XSRF token
            ViewState(AntiXsrfTokenKey) = Page.ViewStateUserKey
            ViewState(AntiXsrfUserNameKey) = If(Context.User.Identity.Name, String.Empty)
        Else
            ' Validate the Anti-XSRF token
            If (Not DirectCast(ViewState(AntiXsrfTokenKey), String) = _antiXsrfTokenValue _
                Or Not DirectCast(ViewState(AntiXsrfUserNameKey), String) = If(Context.User.Identity.Name, String.Empty)) Then
                Throw New InvalidOperationException("Validation of Anti-XSRF token failed.")
            End If
        End If

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageName As String = ""
        Dim pagesRequireGoalNumber As List(Of String) = Nothing

        pagesRequireGoalNumber = New List(Of String)(New String() {"reasonsgoal.aspx", "summarygoal.aspx", "createactivity.aspx",
                                                                 "detailactivity.aspx", "listactivity.aspx", "supporters.aspx"})
        pageName = Path.GetFileName(Request.Url.AbsolutePath).ToLower

        If Request.QueryString("Logout") IsNot Nothing Then
            If Request.QueryString("Logout").Contains("Yes") Then
                signOut()
            End If
        ElseIf checkGoalNumber(Request.QueryString("gNum")) = False Then
            If pagesRequireGoalNumber.IndexOf(pageName) > -1 Then
                Response.Redirect("~/Default.aspx")
            End If
        End If
    End Sub

    Sub signOut()
        FormsAuthentication.SignOut()
        Response.Redirect("~/Default.aspx")
    End Sub
End Class