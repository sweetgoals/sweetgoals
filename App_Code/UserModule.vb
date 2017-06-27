Imports Microsoft.VisualBasic

Public Module UserModule
    'Public userName As String = HttpContext.Current.User.Identity.Name
    'Public UserLoggedIn As Boolean = HttpContext.Current.User.Identity.IsAuthenticated

    Function userName() As String
        Return HttpContext.Current.User.Identity.Name
    End Function

    Function UserLoggedIn() As Boolean
        Return HttpContext.Current.User.Identity.IsAuthenticated
    End Function

    Function getUserEmail() As String
        If UserLoggedIn() = True Then
            Return Membership.GetUser().Email
        Else
            Return ""
        End If
    End Function

    Function checkPicPermissions(ByVal pi As activityPicture,
                                 ByVal theGoal As goal) As Boolean
        Dim db As New sgdataDataContext
        Dim goalSupporters As IQueryable(Of goalSupport) = Nothing
        Dim userEmail As String = ""

        If userName.Length > 0 And
           userName.Contains(theGoal.userName) Then
            Return True
        ElseIf pi.shareSetting IsNot Nothing Then
            If pi.shareSetting.Contains("Supporters") Then
                goalSupporters = From gs In db.goalSupports
                                    Where gs.goalNum = theGoal.goalNumber
                                    Select gs
                userEmail = getUserEmail()
                For Each gs In goalSupporters
                    If userEmail.Length > 0 And
                       gs.sEmail.Contains(userEmail) Then
                        Return True
                    End If
                Next
            ElseIf pi.shareSetting.Contains("Public") Then
                Return True
            End If
        ElseIf pi.shareSetting Is Nothing Then
            Return True
        End If
        Return False
    End Function

End Module
