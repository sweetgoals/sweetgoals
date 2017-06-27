Imports Microsoft.VisualBasic

Public Module goalModule
    Dim db As New sgdataDataContext

    Public Function checkGoalNumber(ByVal goalNumber As Long,
                                    ByVal userName As String) As String
        Dim checkGoalUser As Integer = 999
        Dim checkSupportGoalUser As Integer = 999
        Dim checkPublicUser As Integer = 999
        Dim userMembership As New Membershipdb
        Dim userAccount As New User
        Dim theGoal As goal = Nothing

        checkGoalUser = (From g In db.goals
                             Where g.userName = userName And g.goalNumber = goalNumber
                             Select g).Count()
        If checkGoalUser > 0 Then
            Return "User"
        ElseIf goalNumber > 0 Then
            checkPublicUser = (From g In db.goals
                                 Where g.public.Contains("yes") And g.goalNumber = goalNumber
                                 Select g).Count
            If checkPublicUser > 0 Then
                Return "Public"
            End If
        Else : Return ""
        End If
        userAccount = (From ua In db.Users
                                Where ua.UserName = userName
                                Select ua).FirstOrDefault
        If userAccount IsNot Nothing Then
            userMembership = (From um In db.Membershipdbs
                                    Where um.UserId = userAccount.UserId
                                    Select um).FirstOrDefault
            checkSupportGoalUser = (From g In db.goalSupports
                                        Where g.sEmail = userMembership.Email And g.goalNum = goalNumber
                                        Select g).Count()
            If checkSupportGoalUser > 0 Then
                Return "Supporter"
            End If
        End If
            Return ""
    End Function

    Public Function checkGoalNumber(ByVal requestGoalNumber As String) As Boolean
        Dim checkGoal As New goal
        Dim goalNumber As Integer = -1

        Try
            goalNumber = Convert.ToInt32(requestGoalNumber)
        Catch ex As Exception
            Return Nothing
        End Try
        checkGoal = (From cg In db.goals
                        Where cg.goalNumber = goalNumber
                        Select cg).FirstOrDefault
        If checkGoal IsNot Nothing Then
            Return True
        Else : Return False
        End If
    End Function

    Function getMemberInformation(ByVal userName As String) As Membershipdb
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing

        currentUser = (From us In db.Users
                            Where us.UserName = userName
                            Select us).FirstOrDefault
        If currentUser IsNot Nothing Then
            currentMember = (From cm In db.Membershipdbs
                                Where cm.UserId = currentUser.UserId
                                Select cm).FirstOrDefault
            Return currentMember
        Else : Return Nothing
        End If
    End Function

    Function check24HourLapse(ByVal existingGoal As goal) As Boolean
        Dim timeDifference As TimeSpan

        If existingGoal.createDate IsNot Nothing Then
            timeDifference = DateTime.Now - Convert.ToDateTime(existingGoal.createDate)
            If timeDifference.Days > 0 Then
                Return True
            Else : Return False
            End If
        Else : Return True
        End If
    End Function

    Public Function findGoalsInCollection(ByVal status As String,
                                          ByVal goalCollection As IQueryable(Of goal)) As IQueryable(Of goal)
        Dim foundCollection As IQueryable(Of goal)

        foundCollection = From dg In goalCollection
                            Where dg.status.Contains(status)

        Return foundCollection
    End Function

    Public Function findUserSupportingGoals() As IQueryable(Of goal)
        Dim getUserGoalSupports As IQueryable(Of goalSupport)
        Dim getUserGoals As New List(Of goal)
        Dim goalItem As New goal

        getUserGoalSupports = From gs In db.goalSupports
                                    Where gs.sEmail.Contains(getUserEmail())
                                    Select gs
        If getUserGoalSupports.Count > 0 Then
            For Each gugs In getUserGoalSupports
                goalItem = (From g In db.goals
                                Where g.goalNumber = gugs.goalNum
                                Select g).FirstOrDefault
                getUserGoals.Add(goalItem)
            Next
        End If
        If getUserGoals.Count > 0 Then
            Return getUserGoals.AsQueryable()
        Else : Return Nothing
        End If
    End Function

    Public Function findUserPublicGoals() As IQueryable(Of goal)
        Dim userPublicGoals As IQueryable(Of goal) = Nothing

        userPublicGoals = From g In db.goals
                                Where g.userName.Contains(userName()) And g.public.Contains("yes")
                                Select g
        If userPublicGoals.Count > 0 Then
            Return userPublicGoals
        Else : Return Nothing
        End If
    End Function

    Public Function findCommunityGoals() As IQueryable(Of goal)
        Return From pg In db.goals
                Where pg.public.Contains("yes")
                Select pg
    End Function

    Function checkStartActivity() As String
        'at some point this function will need to be made better because right now it checks all the activities for every 
        'user and then compares then sees if any of those activities are on the current users goal. It really needs to only 
        'grab the activities from the users goals. If the database gets a lot of people on it, and there are a lot of 
        'activities going on at once it won't work that well. 

        Dim checkStartActs As IQueryable(Of activity)
        Dim checkUserGoals As IQueryable(Of goal)

        checkUserGoals = (From cg In db.goals
                                Where cg.userName.Contains(userName) And
                                      cg.status.Contains("working")
                                Select cg)
        checkStartActs = (From a In db.activities
                        Where a.status.Contains("Start")
                        Select a)

        If checkUserGoals.Count > 0 And checkStartActs.Count > 0 Then
            For Each checkAct In checkStartActs
                For Each cug In checkUserGoals
                    If checkAct.goalNumber = cug.goalNumber Then
                        Return cug.goalTitle
                    End If
                Next
            Next
        End If
        Return ""
    End Function

    Public Function GetGoal(ByVal requestGoalNumber As Integer,
                            ByVal localdb As sgdataDataContext) As goal
        'Don't use this unless you're just looking up the goal. Do not try and modify the goal with this function because it will 
        'not update the database some reason.
        Dim requestGoal As New goal

        requestGoal = localdb.goals.[Single](Function(u) u.goalNumber = requestGoalNumber)
        If requestGoal IsNot Nothing Then
            Return requestGoal
        Else
            Return Nothing
        End If
    End Function
End Module
