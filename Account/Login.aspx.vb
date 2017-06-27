
Partial Class Account_Login
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        RegisterHyperLink.NavigateUrl = "Register"
        sendPassword.NavigateUrl = "password"

        'OpenAuthLogin.ReturnUrl = Request.QueryString("ReturnUrl")
        Session.Clear()
        Dim returnUrl = HttpUtility.UrlEncode(Request.QueryString("ReturnUrl"))
        If Not String.IsNullOrEmpty(returnUrl) Then
            'RegisterHyperLink.NavigateUrl &= "?ReturnUrl=" & returnUrl
            RegisterHyperLink.NavigateUrl &= "~/Default.aspx"

        End If
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            '  userName = HttpContext.Current.User.Identity.Name
            ' UserLoggedIn = HttpContext.Current.User.Identity.IsAuthenticated
            Response.Redirect("~/Default.aspx")
        End If

    End Sub
End Class