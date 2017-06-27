Imports System.Net.Mail
Imports System.Net

'This program runs at 1am
Partial Class backend_batch
    Inherits System.Web.UI.Page

    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' runBatchProgram()
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        sendBatchCompletEmail()
    End Sub

    'Sub runBatchProgram()
    '    Dim checkActs As IQueryable(Of activity)
    '    Dim checkGoals As IQueryable(Of goal)
    '    Dim findAct As Integer = -1
    '    Dim insertAct As New activity
    '    Dim dayName As String = ""
    '    Dim dayDate As String = ""
    '    Dim checkFailCount As Integer = 0
    '    Dim checkStart As Integer = 0
    '    Dim checkFinish As Integer = 0
    '    Dim agoal As New goal
    '    Dim theActs As IQueryable(Of activity)
    '    Dim maxSequence As Integer = 0

    '    'find missing acitivites for yesterday
    '    dayName = DateTime.Today.AddDays(-1).DayOfWeek.ToString
    '    dayName = dayName.Substring(0, 3)
    '    dayDate = DateTime.Today.AddDays(-1).ToShortDateString
    '    '**********testing purposes ******************
    '    checkGoals = From g In db.goals
    '              Where g.goalNumber = 73 And
    '                    g.failCount < 3 And
    '                    g.status.Contains("working")
    '              Select g
    '    '*********************************************

    '    'checkGoals = From g In db.goals
    '    '          Where g.scheduleDays.Contains(dayName) And
    '    '                g.failCount < 3 And
    '    '                g.status.Contains("working")
    '    '          Select g


    '    'insert the missed scheduled activities
    '    For Each g In checkGoals
    '        findAct = (From a In db.activities
    '                    Where a.goalNumber = g.goalNumber And a.actDate.Contains(dayDate)
    '                    Select a).Count
    '        checkStart = (From a In db.activities
    '                        Where a.goalNumber = g.goalNumber And a.status.Contains("Start")
    '                        Select a).Count
    '        'If findAct = 0 And
    '        '   checkStart = 0 Then
    '        '    insertAct = New activity
    '        '    theActs = (From a In db.activities
    '        '                Where a.goalNumber = g.goalNumber
    '        '                Order By a.sequence Descending
    '        '                Select a)
    '        '    Try
    '        '        maxSequence = theActs.FirstOrDefault.sequence
    '        '        insertAct.sequence = maxSequence + 1
    '        '    Catch ex As Exception
    '        '        insertAct.sequence = 1
    '        '    End Try

    '        '    insertAct.goalNumber = g.goalNumber
    '        '    insertAct.actTitle = "Missed Activity"
    '        '    insertAct.actDesc = "Sweet Goals inserted missed scheduled activity"
    '        '    insertAct.actDate = dayDate
    '        '    insertAct.good = "None"
    '        '    insertAct.bad = "You need to do activities when scheduled"
    '        '    insertAct.startTime = "00:00:00"
    '        '    insertAct.stopTime = "00:00:00"
    '        '    insertAct.timeDiff = "00:00:00"
    '        '    insertAct.status = "Fail"
    '        '    insertAct.timeMet = "0"
    '        '    db.activities.InsertOnSubmit(insertAct)
    '        '    g.failCount += 1
    '        '    If g.failCount = 3 Then
    '        '        g.status = "suspended"
    '        '        g.completeDate = Today.Date.ToShortDateString
    '        '    End If
    '        '    db.SubmitChanges()
    '        '    sendFailMessage(insertAct, g.failCount)
    '        'End If
    '    Next

    '    'deal with the activities that didn't meet the time requirement
    '    checkActs = From act In db.activities
    '              Where Convert.ToInt16(act.timeMet) = 0 And
    '                act.status = "Complete" And
    '                act.actDate = DateTime.Today.AddDays(-1)
    '                Select act
    '    For Each act In checkActs
    '        checkFailCount = (From g In db.goals
    '                          Where g.goalNumber = act.goalNumber
    '                          Select g).Count()
    '        sendFailMessage(act, checkFailCount)
    '    Next
    '    sendBatchCompletEmail()
    'End Sub

    Sub calculateFail()

    End Sub

    'Sub sendFailMessage(ByVal act As activity,
    '                    ByVal failNum As Integer)
    '    Dim thegoal As New goal

    '    Dim SmtpMail As New SmtpClient
    '    Dim MailMsg As New MailMessage
    '    Dim sEmailList As New List(Of String)
    '    '        Dim toEmail As String = ""
    '    Dim supportNameList As List(Of String)
    '    Dim theSupport As New supporter
    '    Dim i As Integer = 0
    '    Dim iSupport As New supporter
    '    Dim theUser As New User
    '    Dim themember As New Membershipdb
    '    Dim rNum As Long

    '    thegoal = (From g In db.goals
    '              Where g.goalNumber = act.goalNumber
    '              Select g).FirstOrDefault
    '    If thegoal Is Nothing Then
    '        Exit Sub
    '    End If
    '    supportNameList = New List(Of String)(thegoal.supporters.Split(New String() {", "}, _
    '                                                                   StringSplitOptions.RemoveEmptyEntries))
    '    For i = 0 To supportNameList.Count - 1 Step 1
    '        iSupport = (From s In db.supporters
    '                        Where s.userName = thegoal.userName And s.supportName = supportNameList(i)
    '                        Select s).SingleOrDefault
    '        sEmailList.Add(iSupport.supportEmail)
    '    Next
    '    theUser = (From u In db.Users
    '                Where u.UserName = thegoal.userName
    '                Select u).FirstOrDefault
    '    themember = (From m In db.Membershipdbs
    '                    Where m.UserId = theUser.UserId
    '                    Select m).FirstOrDefault()
    '    userFailMsg(themember.Email, thegoal, act)

    '    For i = 0 To sEmailList.Count - 1 Step 1
    '        If onUnscribedList(sEmailList(i)) = False Then
    '            MailMsg = New MailMessage(New MailAddress("enforcer@sweetgoals.com"), New MailAddress(sEmailList(i).Trim()))
    '            MailMsg.BodyEncoding = Encoding.Default
    '            MailMsg.Priority = MailPriority.Normal
    '            MailMsg.IsBodyHtml = True
    '            MailMsg.Subject = "Sweet Goals - Activity Not Completed: " & thegoal.goalTitle
    '            MailMsg.Body &= "Hi, I'm The Enforcer from Sweetgoals.com.<br>"
    '            MailMsg.Body &= thegoal.userName & " didn't do the scehduled activity for goal. <br>"
    '            MailMsg.Body &= "<br><br><br>"
    '            MailMsg.Body &= "Goal Summary<br>"
    '            MailMsg.Body &= "Title: " & thegoal.goalTitle & "<br>"
    '            MailMsg.Body &= "Description: " & thegoal.goalDesc & "<br>"
    '            MailMsg.Body &= "Planned Completion Date: " & thegoal.goalDueDate & "<br>"
    '            MailMsg.Body &= "Schedule: " & thegoal.scheduleDays & "<br>"
    '            MailMsg.Body &= "Time Length: " & thegoal.timeUnit & " " & thegoal.timeLength & "<br>"
    '            MailMsg.Body &= "<br><br><br>"
    '            MailMsg.Body &= "Activity Information<br>"
    '            MailMsg.Body &= "Title: " & act.actTitle & "<br>"
    '            MailMsg.Body &= "Description: " & act.actDesc & "<br>"
    '            MailMsg.Body &= "Date:" & act.actDate & "<br>"
    '            MailMsg.Body &= "Start Time: " & act.startTime & "<br>"
    '            MailMsg.Body &= "End Time: " & act.stopTime & "<br>"
    '            MailMsg.Body &= "Time Spent: " & act.timeDiff & "<br>"
    '            MailMsg.Body &= "Good Part: " & act.good & "<br>"
    '            MailMsg.Body &= "Bad Part: " & act.bad & "<br>"
    '            MailMsg.Body &= "<br><br><br>"
    '            If failNum > 2 Then ' suspend the goal
    '                rNum = mailModule.createEmailTrack(thegoal.goalNumber, act.actNumber, sEmailList(i), "sGoal")
    '                MailMsg.Body &= "They have failed three or more activities which means they are not motovaited and I'm suspending their goal"
    '            Else : rNum = mailModule.createEmailTrack(thegoal.goalNumber, act.actNumber, sEmailList(i), "vAct") 'just fail the activity
    '            End If

    '            MailMsg.Body &= "Our friend has fallen off the horse. They need your help to get them back on the horse. <br>"
    '            theSupport = (From s In db.supporters
    '                          Where s.supportEmail = sEmailList(i) And s.userName = thegoal.userName
    '                          Select s).SingleOrDefault
    '            MailMsg.Body &= "<a href='http://www.sweetgoals.com/backend/response.aspx?delemail=" & sEmailList(i).Trim & "'>Unsubscribe</a> <br>"
    '            mailModule.setMailServer(MailMsg)
    '        End If
    '    Next
    'End Sub

    'Sub userFailMsg(ByVal email As String,
    '                ByVal thegoal As goal,
    '                ByVal act As activity)
    '    Dim SmtpMail As New SmtpClient
    '    Dim MailMsg As New MailMessage
    '    Dim sEmailList As New List(Of String)
    '    Dim toEmail As String = ""
    '    Dim netcred As New NetworkCredential("enforcer@sweetgoals.com", "blah1234")

    '    If onUnscribedList(email) = False Then
    '        MailMsg = New MailMessage(New MailAddress("enforcer@sweetgoals.com"), New MailAddress(email))
    '        MailMsg.BodyEncoding = Encoding.Default
    '        MailMsg.Priority = MailPriority.Normal
    '        MailMsg.IsBodyHtml = True

    '        MailMsg.Subject = "Sweet Goals - Activity Not Completed: " & thegoal.goalTitle
    '        MailMsg.Body &= "Hey you! Yeah YOU!!! I'm talking to you SLACKER!!! You were suppose to work on this yesterday for" & thegoal.timeUnit & " " & thegoal.timeLength & ".<br>"
    '        MailMsg.Body &= "<br><br><br>"
    '        MailMsg.Body &= "Goal Summary<br>"
    '        MailMsg.Body &= "Title: " & thegoal.goalTitle & "<br>"
    '        MailMsg.Body &= "Description: " & thegoal.goalDesc & "<br>"
    '        MailMsg.Body &= "Planned Completion Date: " & thegoal.goalDueDate & "<br>"
    '        MailMsg.Body &= "Schedule: " & thegoal.scheduleDays & "<br>"
    '        MailMsg.Body &= "Time Length: " & thegoal.timeUnit & " " & thegoal.timeLength & "<br>"
    '        MailMsg.Body &= "<br><br><br>"
    '        MailMsg.Body &= "And you didn't!!! Which means you're a slacker!!! Save your excuse for someone who cares because I've heard it all before and it SUCKS anyway!<br>"
    '        MailMsg.Body &= "You took the time and energy so far working on this goal and you said you'd do it too. Your supporters are expecting you <br>"
    '        MailMsg.Body &= "to work on this. But more importantly I'm expecting you to do this. If you don't work on this you'll never complete it!! <br>"
    '        MailMsg.Body &= "You'll also feel way better about yourself if you do this. Plus you'll get the prize. <br>"
    '        MailMsg.Body &= "Stop slacking off and stop wasting peoples time and just get this done. It's important to you and your supporters and most importantly ME! <br>"
    '        MailMsg.Body &= "We want to see you complete this goal!! You can do it! and you WILL DO IT!!! GO GO GO!!! <br><br><br>"
    '        MailMsg.Body &= "I also don't want to hear anymore about you missing activities either! Just do the activies  at the scheduled time and for the specified length and you'll never<br>"
    '        MailMsg.Body &= "have to hear from me again. BUT I will be back if you don't! Stay focused!! You can do it!! Your supporters and I believe in you now lets get this done!!!"
    '        MailMsg.Body &= "<br><br><br>"
    '        If thegoal.failCount > 2 Then
    '            MailMsg.Body &= "You also have failed three activities so I'm suspending your goal. You'll have to log on and reactiviate it!!"
    '        End If

    '        mailModule.setMailServer(MailMsg)
    '    End If
    'End Sub

    Sub sendBatchCompletEmail()
        Dim SmtpMail As New SmtpClient
        Dim MailMsg As New MailMessage
        Dim netcred As New NetworkCredential("enforcer@sweetgoals.com", "blah1234")

        MailMsg = New MailMessage(New MailAddress("enforcer@sweetgoals.com"), New MailAddress("apchampagne@gmail.com"))
        MailMsg.BodyEncoding = Encoding.Default
        MailMsg.Priority = MailPriority.Normal
        MailMsg.IsBodyHtml = True

        MailMsg.Subject = "Sweet Goals - Ran Batch Process Complete"
        MailMsg.Body &= "Ran batch process"
        mailModule.setMailServer(MailMsg)
    End Sub

    Function onUnscribedList(ByVal email As String) As Boolean
        Dim checkunsubscribe As Integer = 0
        checkunsubscribe = (From u In db.unsubscribes
                    Where u.email = email
                    Select u).Count
        If checkunsubscribe > 0 Then
            Return True
        Else : Return False
        End If
    End Function
End Class
