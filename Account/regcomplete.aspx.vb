
Partial Class Account_regcomplete
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Request.QueryString("emailAddress") IsNot Nothing Then
            registrationMessage.Text = "Registration Success!! Email has been sent to " & Request.QueryString("emailAddress")
            registrationMessage.Text &= " Please check address for verification."
        Else
            registrationMessage.Text = "<h2> Nothing Here </h2>"
        End If
    End Sub
End Class
