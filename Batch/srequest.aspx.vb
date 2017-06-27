
Partial Class backend_srequest
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Sub unsubscribe()
        Dim emailAddress As String
        Dim unsub As New unsubscribe
        emailAddress = Request.QueryString("delEmail")
        If emailAddress.Contains("@") Then
            unsub.email = emailAddress
            db.unsubscribes.InsertOnSubmit(unsub)
            db.SubmitChanges()
            msgLabel.Text = "Added " & emailAddress & " to unsubscribe list. No worries mate! Feel free to come back anytime. "
        End If
    End Sub

End Class
