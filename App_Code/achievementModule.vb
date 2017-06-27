Imports Microsoft.VisualBasic

Public Module achievementModule
    Dim db As New sgdataDataContext

    Sub insertAch(ByVal userName As String,
                   ByVal type As String,
                   ByVal achTitle As String)
        Dim addAch As New achievement

        addAch.userName = userName
        addAch.type = type
        addAch.compDate = Today.Date.ToShortDateString
        addAch.title = achTitle
        db.achievements.InsertOnSubmit(addAch)
        db.SubmitChanges()
    End Sub

    Public Function createAccount(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Create Goal")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Created Account")
            Return "Created Account"
        End If
        Return ""
    End Function

    Public Function createGoal(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Created Goal")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Created Goal")
            Return "Created Goal"
        End If
        Return ""
    End Function

    Public Function completeGoal(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Complete Goal")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Complete Goal")
            Return "Complete Goal"
        End If
        Return ""
    End Function

    Public Function createActivity(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Created Activity")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Created Activity")
            Return "Created Activity"
        End If
        Return ""
    End Function

    Public Function startActivity(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Started Activity")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Started Activity")
            Return "Started Activity"
        End If
        Return ""
    End Function

    Public Function suggestActivity(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Suggest Activity")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Suggest Activity")
            Return "Suggest Activity"
        End If
        Return ""
    End Function

    Public Function completeActivity(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Complete Activity")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Complete Activity")
            Return "Complete Activity"
        End If
        Return ""
    End Function

    Public Function pictureActivity(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Activity Picture")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Activity Picture")
            Return "Activity Picture"
        End If
        Return ""
    End Function

    Public Function goalReason(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Create A Reason")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Create A Reason")
            Return "Create A Reason"
        End If
        Return ""
    End Function

    'not hooked up yet
    Public Function requestAccountSupport(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Request Account Support")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Request Account Support")
            Return "Request Account Support"
        End If
        Return ""
    End Function

    'not hooked up yet
    Public Function requestGoalSupport(ByVal userName As String) As String
        If (From ach In db.achievements
                    Where ach.userName.Contains(userName) And ach.title.Contains("Request Goal Support")
                    Select ach).Count() = 0 Then
            insertAch(userName, "account", "Request Goal Support")
            Return "Request Goal Support"
        End If
        Return ""
    End Function

End Module
