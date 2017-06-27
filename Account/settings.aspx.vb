
Partial Class settings
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            getSettings()
        Else : Response.Redirect("~/Account/Login.aspx")
        End If
    End Sub

    Protected Sub updateButton_Click(sender As Object, e As EventArgs) Handles updateButton.Click
        Dim supportInfo As New supporter
        Dim delUnsubscribe As New unsubscribe
        Dim insertUnsubscribe As New unsubscribe
        Dim ucount As Integer = 0
        Dim memUser As MembershipUser
        Dim supportEmail As IQueryable(Of goalSupport)


        supportInfo = (From s In db.supporters
                           Where s.userName = HttpContext.Current.User.Identity.Name
                           Select s).FirstOrDefault
        If Not supportInfo Is Nothing Then
            delUnsubscribe = (From u In db.unsubscribes
                                    Where supportInfo.supportEmail = u.email
                                    Select u).FirstOrDefault
            ucount = (From u In db.unsubscribes
                      Where supportInfo.supportEmail = u.email
                      Select u).Count()
            If unsubscribeBox.Checked = False And
                ucount > 0 Then
                db.unsubscribes.DeleteOnSubmit(delUnsubscribe)
            ElseIf unsubscribeBox.Checked = True And
                   ucount = 0 Then
                insertUnsubscribe.email = supportInfo.supportEmail
                db.unsubscribes.InsertOnSubmit(insertUnsubscribe)
            End If
            db.SubmitChanges()

            memUser = Membership.GetUser(HttpContext.Current.User.Identity.Name)
            If Not memUser.Email.Contains(emailBox.Text.Trim) Then
                supportEmail = From se In db.goalSupports
                                    Where se.sEmail.Contains(memUser.Email)
                For Each se In supportEmail
                    se.sEmail = emailBox.Text.Trim.ToLower
                Next
                memUser.Email = emailBox.Text.Trim
                db.SubmitChanges()
            End If
            Try
                Membership.UpdateUser(memUser)
                errorLabel.Text = "Saved"
                errorLabel.Visible = True
                errorLabel.ForeColor = Drawing.Color.Green
            Catch ex As Exception
                errorLabel.Text = "Bad Email address"
                errorLabel.ForeColor = Drawing.Color.Red
                errorLabel.Visible = True
            End Try
        End If
    End Sub

    Sub getSettings()
        Dim supportInfo As New supporter
        Dim checkUnsubscribe As Integer = 0
        Dim memUser As MembershipUser
        If IsPostBack = False Then
            supportInfo = (From s In db.supporters
                               Where s.userName = HttpContext.Current.User.Identity.Name
                               Select s).FirstOrDefault
            If supportInfo IsNot Nothing Then
                checkUnsubscribe = (From u In db.unsubscribes
                                        Where supportInfo.supportEmail = u.email
                                        Select u).Count()
                If checkUnsubscribe = 0 Then
                    unsubscribeBox.Checked = False
                Else : unsubscribeBox.Checked = True
                End If
            End If
            memUser = Membership.GetUser(userName)
            emailBox.Text = memUser.Email
        End If
    End Sub
End Class
