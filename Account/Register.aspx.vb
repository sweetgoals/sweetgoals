Imports System.Net.Mail
Imports System.IO

Partial Class Account_Register
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        RegisterUser.ContinueDestinationPageUrl = Request.QueryString("ReturnUrl")
    End Sub

    Protected Sub RegisterUser_CreatedUser(ByVal sender As Object, ByVal e As EventArgs) Handles RegisterUser.CreatedUser
        FormsAuthentication.SetAuthCookie(RegisterUser.UserName, createPersistentCookie:=False)
        Dim continueUrl As String = RegisterUser.ContinueDestinationPageUrl

        sendVerificationEmail()
        If RegisterUser IsNot Nothing Then
            Response.Redirect("~/Account/regcomplete.aspx?emailAddress=" & RegisterUser.Email)
        Else : Response.Redirect("~/Account/Logon.aspx")
        End If
    End Sub

    Sub sendVerificationEmail()
        Dim newUserMembership = Membership.GetUser(RegisterUser.UserName)
        Dim userGuid As New Guid
        Dim MailMsg As New MailMessage
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As _
            New StreamReader(System.Web.HttpContext.Current.Server.MapPath("/emailTemplates/accountActivation.html"))

        userGuid = DirectCast(newUserMembership.ProviderUserKey, Guid)
        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(RegisterUser.Email))
        MailMsg.Priority = MailPriority.Normal
        MailMsg.IsBodyHtml = True
        MailMsg.Subject = "Sweet Goals - Account Activation"
        MailMsg.Body = goalSupportForm.ReadToEnd()
        MailMsg.Body = MailMsg.Body.Replace("[verifyAccountNumber]", userGuid.ToString)
        MailMsg.Body = MailMsg.Body.Replace("[supporterEmailAddress]", RegisterUser.Email)
        setMailServer(MailMsg)
        'sendMeAnEmail(RegisterUser.UserName)

    End Sub

    Sub sendMeAnEmail(ByVal username As String)
        Dim MailMsg As New MailMessage

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress("apchampagne@gmail.com"))
        MailMsg.Priority = MailPriority.Normal
        MailMsg.IsBodyHtml = True
        MailMsg.Subject = "Sweet Goals - Account Activation"
        MailMsg.Body = username & " created an account on sweetgoals.com"
        setMailServer(MailMsg)
    End Sub
End Class