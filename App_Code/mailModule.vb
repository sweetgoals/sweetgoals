Imports Microsoft.VisualBasic
Imports System.Net
Imports System.Net.Mail
Imports System.IO

Public Module mailModule
    Dim db As New sgdataDataContext

    Public Sub setMailServer(ByVal msg As MailMessage, Optional netCreds As NetworkCredential = Nothing)
        Dim defaultCreds As New NetworkCredential("dreamchaser@sweetgoals.com", "Ihateyou123")
        Dim mailClient As New SmtpClient

        If Not netCreds Is Nothing Then
            defaultCreds = netCreds
        End If
        If System.Net.Dns.GetHostName.ToLower.Contains("donald") Then
            mailClient.Host = "smtp-server.san.rr.com"
        Else
            mailClient.Host = "mail.sweetgoals.com"
        End If
        mailClient.Credentials = defaultCreds
        mailClient.Send(msg)
    End Sub

    Public Function createEmailTrack(ByVal goalNum As Long, _
                                ByVal actnum As Long, _
                                ByVal email As String, _
                                ByVal type As String) As Long
        Dim msgData As New emailtrack
        Dim eCount As Long = 0

        msgData = New emailtrack
        msgData.actNum = actnum
        msgData.goalNum = goalNum
        msgData.email = email
        msgData.type = type
        msgData.processed = 0
        db.emailtracks.InsertOnSubmit(msgData)
        db.SubmitChanges()
        eCount = db.emailtracks.Count
        msgData.refNumber = eCount
        db.SubmitChanges()

        Return msgData.refNumber
    End Function

    Function inUnsubscribes(ByVal email As String) As Boolean
        'returns true if it's in the unsubscribe list
        'returns false if it's not in the unsubscribe list

        Dim checkEmailAddress As Integer = 999
        Dim db As New sgdataDataContext

        checkEmailAddress = (From d In db.unsubscribes
                                Where d.email = email
                                Select d).Count
        If (checkEmailAddress < 1) Then
            Return False
        Else : Return True
        End If
    End Function

    Function createCompleteGoalEmail(ByVal thegoal As goal,
                              ByVal theSupporterEmail As String,
                              ByVal rNum As Long) As MailMessage
        Dim MailMsg As New MailMessage
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As _
            New StreamReader(System.Web.HttpContext.Current.Server.MapPath("../emailTemplates/goalComplete.html"))
        Dim outMsgBody As String = ""

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(theSupporterEmail))
        currentMember = getMemberInformation(thegoal.userName)
        If currentMember IsNot Nothing Then
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True
            MailMsg.Subject = "Sweet Goals - " & thegoal.goalTitle & " Has Been Completed By " & thegoal.userName
            outMsgBody = goalSupportForm.ReadToEnd()
            outMsgBody = outMsgBody.Replace("[currentGoalTitle]", thegoal.goalTitle)
            outMsgBody = outMsgBody.Replace("[currentUserEmailAddress]", currentMember.Email)
            outMsgBody = outMsgBody.Replace("[currentGoalUsername]", thegoal.userName)
            outMsgBody = outMsgBody.Replace("[currentGoalDesc]", thegoal.goalDesc)
            outMsgBody = outMsgBody.Replace("[rNumber]", rNum)
            outMsgBody = outMsgBody.Replace("[supporterEmailAddress]", theSupporterEmail)
            MailMsg.Body = outMsgBody
        End If
        Return MailMsg
    End Function

    Function createRestartGoalEmail(ByVal thegoal As goal,
                                    ByVal theSupporterEmail As String,
                                    ByVal rNum As Long) As MailMessage
        Dim MailMsg As New MailMessage
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As _
                New StreamReader(System.Web.HttpContext.Current.Server.MapPath("../emailTemplates/goalRestart.html"))
        Dim outMsgBody As String = ""

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(theSupporterEmail))
        currentMember = getMemberInformation(thegoal.userName)
        If currentMember IsNot Nothing Then
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True
            MailMsg.Subject = "Sweet Goals - " & thegoal.goalTitle & " Has Been Restarted By " & thegoal.userName
            outMsgBody = goalSupportForm.ReadToEnd()
            outMsgBody = outMsgBody.Replace("[currentGoalTitle]", thegoal.goalTitle)
            outMsgBody = outMsgBody.Replace("[currentUserEmailAddress]", currentMember.Email)
            outMsgBody = outMsgBody.Replace("[currentGoalUsername]", thegoal.userName)
            outMsgBody = outMsgBody.Replace("[currentGoalDesc]", thegoal.goalDesc)
            outMsgBody = outMsgBody.Replace("[rNumber]", rNum)
            outMsgBody = outMsgBody.Replace("[supporterEmailAddress]", theSupporterEmail)
            MailMsg.Body = outMsgBody
        End If
        Return MailMsg
    End Function

End Module
