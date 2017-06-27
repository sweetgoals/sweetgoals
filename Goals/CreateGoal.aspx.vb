Imports System.Net
Imports System.Net.Mail

Partial Class creategoal
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Private Sub creategoal_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        freqBox.Attributes.Add("onblur", "checkMultiFreqUnit();")
        timeText.Attributes.Add("onblur", "checkMultiTimeUnit();")
        enableGoalModification()
    End Sub

    Protected Sub submitButton_Click(sender As Object, e As EventArgs) Handles submitButton.Click
        Dim createdGoal As New goal

        createdGoal.userName = userName()
        createdGoal.goalTitle = goalTitleText.Text
        createdGoal.goalDesc = goalDescText.Text
        createdGoal.timeLength = timeText.Text
        If dueDate.Value = "" Then
            dueDate.Value = Today.Date.ToShortDateString
        End If
        createdGoal.goalDueDate = Convert.ToDateTime(dueDate.Value).ToShortDateString
        createdGoal.frequency = freqBox.Text
        If freqDropDownSelect.Value = "Plural" Then
            createdGoal.frequencyUnit = freqDropDownPlural.SelectedValue.ToString
        Else
            createdGoal.frequencyUnit = freqDropDownSingle.SelectedValue.ToString
        End If
        If timeUnitSelect.Value = "Plural" Then
            createdGoal.timeUnit = timeUnitDropPlural.SelectedValue.ToString
        Else
            createdGoal.timeUnit = timeUnitDropSingle.SelectedValue.ToString
        End If
        createdGoal.status = "working"
        createdGoal.public = "no"
        createdGoal.customFields = "no"
        createdGoal.createDate = DateTime.Now.ToString
        If Request.QueryString("gNum") IsNot Nothing Then
            modifyGoal(createdGoal)
        Else : createGoal(createdGoal)
        End If
        Response.Redirect("../Default.aspx")
    End Sub

    Sub createGoal(ByVal createdGoal As goal)
        db.goals.InsertOnSubmit(createdGoal)
        db.SubmitChanges()
    End Sub

    Sub modifyGoal(ByVal createdGoal As goal)
        Dim existingGoal As goal = Nothing
        Dim previousGoal As goalHistory = Nothing

        existingGoal = New goal
        existingGoal = (From g In db.goals
                        Where Request.QueryString("gnum") = g.goalNumber
                        Select g).FirstOrDefault
        If existingGoal IsNot Nothing Then
            previousGoal = New goalHistory
            previousGoal.goalNumber = existingGoal.goalNumber
            previousGoal.goalTitle = existingGoal.goalTitle
            previousGoal.timeLength = existingGoal.timeLength
            previousGoal.timeUnit = existingGoal.timeUnit
            previousGoal.goalDesc = existingGoal.goalDesc
            previousGoal.frequency = existingGoal.frequency
            previousGoal.frequencyUnit = existingGoal.frequencyUnit
            previousGoal.modifyDate = existingGoal.createDate
            previousGoal.version = (From pg In db.goalHistories
                                    Where pg.goalNumber = existingGoal.goalNumber).Count() + 1
            existingGoal.goalTitle = createdGoal.goalTitle
            existingGoal.goalDueDate = createdGoal.goalDueDate
            existingGoal.timeLength = createdGoal.timeLength
            existingGoal.timeUnit = createdGoal.timeUnit
            existingGoal.goalDesc = createdGoal.goalDesc
            existingGoal.frequency = createdGoal.frequency
            existingGoal.frequencyUnit = createdGoal.frequencyUnit
            existingGoal.createDate = createdGoal.createDate

            db.goalHistories.InsertOnSubmit(previousGoal)
            db.SubmitChanges()
        End If
    End Sub


    Sub enableGoalModification()
        Dim goalAssociate As String = ""
        Dim theGoal As goal = Nothing
        If Request.QueryString("gNum") IsNot Nothing Then
            theGoal = GetGoal(Request.QueryString("gNum"), db)
            If theGoal IsNot Nothing Then
                goalAssociate = checkGoalNumber(Request.QueryString("gNum"), userName)
                goalTitleText.Text = theGoal.goalTitle
                goalDescText.Text = theGoal.goalDesc
                timeText.Text = theGoal.timeLength
                freqBox.Text = theGoal.frequency
                dueDate.Value = theGoal.goalDueDate
                If goalAssociate.Contains("User") And
                   check24HourLapse(theGoal) And
                   theGoal.status.Contains("working") Then
                    pageTitle.Text = "Modify Goal"
                    submitButton.Text = "Modify Goal"
                Else : Response.Redirect("../Default.aspx")
                End If
            End If
        End If
    End Sub

End Class