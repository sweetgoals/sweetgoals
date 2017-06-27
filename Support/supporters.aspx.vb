Imports System.Net
Imports System.Net.Mail
Imports System.IO

Partial Class supporters
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.QueryString("gNum") IsNot Nothing Then
            Session("gN") = Request.QueryString("gNum")
        End If
        createAccountSupportTable()
        If Request.QueryString.Count > 0 Then
            createGoalSupportTable()
        End If
    End Sub

    Sub createAccountSupportTable()
        Dim acell As New TableCell
        Dim arow As New TableRow
        Dim allSupporters As IQueryable(Of supporter)
        Dim userGoals As IQueryable(Of goal)
        Dim sMember As New Membershipdb
        Dim sUser As New User
        Dim sName As String = ""
        Dim cellString As String = ""
        Dim requestSupport As IQueryable(Of String)
        Dim i As Integer = 0
        Dim emailList As New List(Of String)

        arow.Cells.Add(createCell("Support Name"))
        arow.Cells.Add(createCell("Support Email"))
        arow.Cells.Add(createCell("Status"))
        allSupporters = From s In db.supporters
                        Where s.userName = HttpContext.Current.User.Identity.Name
                        Select s
        userGoals = From ug In db.goals
                   Where ug.userName = HttpContext.Current.User.Identity.Name
                   Select ug
        i = 0
        For Each ug In userGoals
            requestSupport = (From rs In db.emailtracks
                             Where rs.goalNum = ug.goalNumber And
                             (rs.type.Contains("raSupport") Or rs.type.Contains("rgSupport")) And
                              rs.processed = 0
                             Select rs.email).Distinct()

            For Each rs In requestSupport
                arow = New TableRow
                If checkAccountSupport(rs, userName).Contains("Request Sent") Then
                    arow.Cells.Add(createCell(checkUser(rs)))
                    arow.Cells.Add(createCellLink(rs, "dAccount", i))
                    arow.Cells.Add(createCell(checkGoalSupport(rs, ug.goalNumber)))
                    i += 1
                End If
            Next
        Next

        For Each support In allSupporters
            arow = New TableRow
            arow.Cells.Add(createCell(checkUser(support.supportEmail)))
            arow.Cells.Add(createCellLink(support.supportEmail, "dAccount", i))
            arow.Cells.Add(createCell(checkAccountSupport(support.supportEmail, userName)))
            i += 1
        Next
        'last row to add a supporter
        arow = New TableRow
        arow.Cells.Add(createCell(""))
        arow.Cells.Add(createCellLink("Add Supporter To Account", "aAccount", i))
        arow.Cells.Add(createCell(""))
    End Sub

    Function checkAccountSupport(ByVal email As String,
                                 ByVal username As String) As String
        Dim findSupporter As Integer = 0

        findSupporter = (From s In db.supporters
                            Where s.supportEmail.Contains(email) And s.userName.Contains(username)
                            Select s).Count()
        If findSupporter > 0 Then
            Return "Supporting"
        Else : Return "Request Sent"
        End If
    End Function
    Sub createGoalSupportTable()
        Dim goalNumber As Integer = 0
        Dim thegoal As New goal
        Dim acell As New TableCell
        Dim arow As New TableRow
        Dim supString As String = ""
        Dim theSupports As IQueryable(Of goalSupport)
        Dim rSupports As IQueryable(Of String)
        Dim ecount As Integer = -1
        Dim checkGoal As New goal
        Dim i As Integer = 0
        goalNumber = getGoalNumber()
        If goalNumber = -1 Then
            Exit Sub
        End If

        goalSupportTable.Rows.Clear()
        checkGoal = (From g In db.goals
                        Where g.userName = HttpContext.Current.User.Identity.Name And
                              g.goalNumber = goalNumber
                        Select g).FirstOrDefault
        If checkGoal IsNot Nothing Then
            goalSupportTable.Caption = "<h3>Goal: " & thegoal.goalTitle & " Supporters </h3>"
            goalSupportTable.Visible = True

            theSupports = (From gs In db.goalSupports
                           Where gs.goalNum = goalNumber)
            rSupports = (From ed In db.emailtracks
                            Where ed.goalNum = goalNumber And ed.type = "rgSupport" And ed.processed = 0
                            Select ed.email).Distinct()
            goalSupportTable.Rows.Clear()
            arow.Cells.Add(createCell("Support Name"))
            arow.Cells.Add(createCell("Support Email"))
            arow.Cells.Add(createCell("Status"))
            goalSupportTable.Rows.Add(arow)

            For Each ed In rSupports
                arow = New TableRow
                arow.Cells.Add(createCell(checkUser(ed)))
                arow.Cells.Add(createCellLink(ed, "dGoal", i))
                arow.Cells.Add(createCell(checkGoalSupport(ed, goalNumber)))
                goalSupportTable.Rows.Add(arow)
                i += 1
            Next
            For Each s In theSupports
                ecount = (From rs In rSupports
                            Where rs = s.sEmail).Count()
                If ecount < 1 Then
                    arow = New TableRow
                    arow.Cells.Add(createCell(checkUser(s.sEmail)))
                    arow.Cells.Add(createCellLink(s.sEmail, "dGoal", i))
                    arow.Cells.Add(createCell(checkGoalSupport(s.sEmail, goalNumber)))
                    goalSupportTable.Rows.Add(arow)
                    i += 1
                End If
            Next
            'add Add Goal Link
            arow = New TableRow
            arow.Cells.Add(createCell(""))
            arow.Cells.Add(createCellLink("Add Supporter To Goal", "aGoal", i))
            arow.Cells.Add(createCell(""))
            goalSupportTable.Rows.Add(arow)
        Else 'bad goal number just reload no goals
            Response.Redirect("supporters.aspx")
        End If
    End Sub

    Function checkGoalSupport(ByVal email As String, ByVal gn As Long) As String
        Dim theSupporter As New goalSupport
        Dim checkNoSupport As New unsubscribe
        Dim checkEmailTrack As New emailtrack

        theSupporter = (From s In db.goalSupports
                        Where s.goalNum = gn And s.sEmail.Contains(email)
                        Select s).FirstOrDefault
        checkNoSupport = (From c In db.unsubscribes
                            Where c.email.Contains(email)
                            Select c).FirstOrDefault
        checkEmailTrack = (From em In db.emailtracks
                            Where em.goalNum = gn And em.email.Contains(email) And em.processed = 0
                            Select em).FirstOrDefault

        If (theSupporter IsNot Nothing) Then
            Return "Supporting"
        ElseIf (checkEmailTrack IsNot Nothing) Then
            Return "Request Sent"
        ElseIf checkNoSupport IsNot Nothing Then
            Return "Unsubscribed"
        Else : Return "Not Supporting"
        End If
    End Function

    Function checkUser(ByVal email As String) As String
        Dim sMember As New Membershipdb
        Dim sUser As New User

        sMember = (From m In db.Membershipdbs
           Where m.Email = email
           Select m).FirstOrDefault
        If Not sMember Is Nothing Then
            sUser = (From u In db.Users
                     Where u.UserId = sMember.UserId
                     Select u).FirstOrDefault
            Return sUser.UserName
        Else
            Return "User Not Registered"
        End If
    End Function

    Function createCell(ByVal cellText As String) As TableCell
        Dim acell As New TableCell

        acell.Text = cellText
        Return acell
    End Function

    Function createCellLink(ByVal cellText As String,
                            ByVal cellClass As String,
                            ByVal i As Integer) As TableCell
        Dim acell As New TableCell
        Dim alink As New HtmlAnchor

        alink.ID = "email_" & cellClass & i
        alink.InnerText = cellText
        alink.HRef = "#"
        alink.Attributes("class") = cellClass '"dAccount"
        acell.Controls.Add(alink)
        Return acell
    End Function

    Function createCellBox(ByVal id As String, ByVal txt As String) As TableCell
        Dim acell As New TableCell
        Dim abutton As New Button

        abutton.ID = "remove_" & id
        abutton.Text = txt
        acell.Controls.Add(abutton)
        Return acell
    End Function

    Function createRow(ByVal cellStrings() As String) As TableRow
        Dim acell As New TableCell
        Dim arow As New TableRow

        For Each cs In cellStrings
            acell = New TableCell
            acell.Text = cs
            arow.Cells.Add(acell)
        Next
        Return arow
    End Function

    Function getGoalNumber() As Long
        Dim goalNumber As Long = -1
        Try
            goalNumber = Convert.ToInt32(Request.QueryString("gNum"))
        Catch ex As Exception           
        End Try
        If goalModule.checkGoalNumber(goalNumber, HttpContext.Current.User.Identity.Name).Length > 0 Then
            Return goalNumber
        Else
            Return -1
        End If
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function requestSupport(ByVal supemail As String,
                                     ByVal supmsg As String,
                                     ByVal gn As Integer) As String

        Dim accountSupport As Integer = -1
        Dim goalNumber As Integer = 0
        Dim thegoal As New goal
        Dim inNoSupport As Integer = -1
        Dim inGoalSupport As Integer = -1

        If supemail.Contains("@") Then
            Return (sendRequest(supemail, supmsg, gn, "raSupport"))
        Else : Return ("Invalid Email address")
        End If
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function requestGoalSupport(ByVal agsupemail As String,
                                     ByVal agsupmsg As String,
                                     ByVal gn As Integer) As String
        If agsupemail.Contains("@") Then
            Return (sendRequest(agsupemail, agsupmsg, gn, "rgSupport"))
        Else : Return ("Invalid Email Address")
        End If
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Sub removeGoalSupport(ByVal supemail As String,
                                        ByVal supmsg As String,
                                        ByVal gn As Integer)
        deleteGoalSupport(supemail, gn, supmsg)
        removeEmailTrack(gn, supemail, "rgSupport")
    End Sub

    <System.Web.Services.WebMethod()> _
    Shared Sub deleteGoalSupport(ByVal smail As String,
                                 ByVal goalNumber As Long,
                                 ByVal sMessage As String)
        Dim serverDB As New sgdataDataContext
        Dim goalItem As New goal
        Dim supporterItem As New goalSupport
        Dim reqGoalSupEmailTrackItem As New emailtrack
        Dim removedSupport As Boolean = False

        goalItem = (From g In serverDB.goals
                        Where g.userName = HttpContext.Current.User.Identity.Name And
                              g.goalNumber = goalNumber
                        Select g).FirstOrDefault
        If goalItem IsNot Nothing Then
            supporterItem = (From s In serverDB.goalSupports
                        Where s.sEmail = smail And s.goalNum = goalItem.goalNumber
                        Select s).FirstOrDefault
            reqGoalSupEmailTrackItem = (From eti In serverDB.emailtracks
                                Where eti.goalNum = goalNumber And eti.email = smail
                                Select eti).FirstOrDefault
            If supporterItem IsNot Nothing Then
                serverDB.goalSupports.DeleteOnSubmit(supporterItem)
                serverDB.SubmitChanges()
                removedSupport = True
            ElseIf reqGoalSupEmailTrackItem IsNot Nothing Then
                reqGoalSupEmailTrackItem.processed = 1
                serverDB.SubmitChanges()
                removedSupport = True
            End If
            If removedSupport = True Then
                If (smail.Contains("@")) Then
                    removeGoalSupportEmail(goalItem, smail, sMessage)
                End If
            End If
        End If

    End Sub

    Shared Sub deleteSupport(ByVal smail As String)
        Dim dSupport As supporter
        Dim shared_db As New sgdataDataContext

        dSupport = (From m In shared_db.supporters _
           Where m.supportEmail = smail And
                 m.userName = HttpContext.Current.User.Identity.Name
           Select m).SingleOrDefault
        If dSupport IsNot Nothing Then
            shared_db.supporters.DeleteOnSubmit(dSupport)
            shared_db.SubmitChanges()
        End If
    End Sub

    Shared Sub removeEmailTrack(ByVal gn As Long,
                                ByVal smail As String,
                                ByVal type As String)
        Dim rTrack As IQueryable(Of emailtrack)
        Dim shared_db As New sgdataDataContext
        rTrack = (From ed In shared_db.emailtracks
                    Where ed.email = smail And
                          ed.goalNum = gn And
                          ed.processed = 0 And
                          ed.type = type
                    Select ed)
        For Each ed In rTrack
            ed.processed = 1
            shared_db.SubmitChanges()
        Next
    End Sub

    Shared Function sendRequest(ByVal supemail As String,
                                ByVal supmsg As String,
                                ByVal gn As Integer,
                                ByVal type As String) As String

        Dim accountSupport As Integer = -1
        Dim goalNumber As Integer = 0
        Dim thegoal As New goal
        Dim inNoSupport As Integer = -1
        Dim inGoalSupport As Integer = -1
        Dim shared_db As New sgdataDataContext
        Dim checkEmailTrack As Integer = -1
        Dim sMember As New Membershipdb
        Dim currentUser As New User
        goalNumber = gn

        thegoal = (From g In shared_db.goals
                      Where g.goalNumber = goalNumber
                      Select g).FirstOrDefault

        currentUser = (From u In shared_db.Users
                        Where u.UserName = HttpContext.Current.User.Identity.Name
                        Select u).SingleOrDefault

        sMember = (From sm In shared_db.Membershipdbs
                        Where sm.UserId = currentUser.UserId
                        Select sm).SingleOrDefault

        accountSupport = (From m In shared_db.supporters
                        Where m.supportEmail = supemail And
                              m.userName = HttpContext.Current.User.Identity.Name
                        Select m.supportEmail).Count


        inNoSupport = (From s In shared_db.noSupports
                       Where s.goalNum = thegoal.goalNumber
                       Select s).Count()

        inGoalSupport = (From s In shared_db.goalSupports
                        Where s.sEmail = supemail And
                              s.goalNum = thegoal.goalNumber
                        Select s).Count()

        checkEmailTrack = (From em In shared_db.emailtracks
                                Where em.email = supemail And em.goalNum = thegoal.goalNumber And em.type = type And
                                em.processed = 0).Count
        If inNoSupport = 0 Then
            If checkEmailTrack > 0 Then
                Return "Support Already Requested"
            ElseIf String.Compare(sMember.Email, supemail) = 0 Then
                Return "Can't support your own goal"
            ElseIf type.Contains("rgSupport") And inGoalSupport = 0 Then 'And accountSupport > 0 
                requestSupportEmail(thegoal, supemail, supmsg, type)
                Return ""
            Else
                Return "Duplicate Entry"
            End If
        Else
            Return "On Unsubscribe List"
            End If
    End Function

    Public Shared Sub requestSupportEmail(ByVal theGoal As goal, _
                            ByVal theSupporterEmail As String,
                            ByVal supmsg As String,
                            ByVal type As String)
        Dim MailMsg As New MailMessage
        Dim rNum As Long
        Dim userName As String = HttpContext.Current.User.Identity.Name
        Dim shareddb As New sgdataDataContext

        If (mailModule.inUnsubscribes(theSupporterEmail) = False) And (theGoal IsNot Nothing) Then
            rNum = mailModule.createEmailTrack(theGoal.goalNumber, -1, theSupporterEmail, type)
            MailMsg = createRequestSupportEmail(theGoal, supmsg, rNum, theSupporterEmail)
            mailModule.setMailServer(MailMsg)
        End If
    End Sub

    Shared Function createRequestSupportEmail(ByVal thegoal As goal,
                                          ByVal supmsg As String,
                                          ByVal rNum As Long,
                                          ByVal theSupporterEmail As String) As MailMessage
        Dim sharedDb As New sgdataDataContext
        Dim MailMsg As New MailMessage
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As _
            New StreamReader(System.Web.HttpContext.Current.Server.MapPath("../EmailTemplates/reqGoalSupport.html"))
        Dim outMsgBody As String = ""

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(theSupporterEmail))
        currentMember = getMemberInformation(thegoal.userName)
        If currentMember IsNot Nothing Then
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True
            MailMsg.Subject = "Sweet Goals - Request to Support Goal: " & thegoal.goalTitle
            outMsgBody = goalSupportForm.ReadToEnd()
            outMsgBody = outMsgBody.Replace("[goalTitle]", thegoal.goalTitle)
            outMsgBody = outMsgBody.Replace("[currentUserEmailAddress]", currentMember.Email)
            outMsgBody = outMsgBody.Replace("[currentUserName]", thegoal.userName)
            outMsgBody = outMsgBody.Replace("[goalDesc]", thegoal.goalDesc)
            outMsgBody = outMsgBody.Replace("[supportMessage]", supmsg)
            outMsgBody = outMsgBody.Replace("[rNum]", rNum)
            outMsgBody = outMsgBody.Replace("[supporterEmailAddress]", theSupporterEmail)
            MailMsg.Body = outMsgBody
        End If
        Return MailMsg
    End Function

    Shared Function getMemberInformation(ByVal userName As String) As Membershipdb
        Dim sharedDb As New sgdataDataContext
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing

        currentUser = (From us In sharedDb.Users
                    Where us.UserName = userName
                    Select us).FirstOrDefault
        If currentUser IsNot Nothing Then
            currentMember = (From cm In sharedDb.Membershipdbs
                                Where cm.UserId = currentUser.UserId
                                Select cm).FirstOrDefault
            Return currentMember
        Else : Return Nothing
        End If
    End Function

    Shared Sub removeGoalSupportEmail(ByVal goalInfo As goal, _
                                      ByVal supporterEmail As String,
                                      ByVal messageToSupporter As String)
        Dim mailMsg As New MailMessage
        If mailModule.inUnsubscribes(supporterEmail) = False Then
            mailMsg = createRemoveSupportEmail(goalInfo, supporterEmail, messageToSupporter)
            mailModule.setMailServer(mailMsg)
        End If
    End Sub

    Shared Function createRemoveSupportEmail(ByVal goalInfo As goal,
                                             ByVal theSupporterEmail As String,
                                             ByVal messageToSupporter As String) As MailMessage
        Dim sharedDb As New sgdataDataContext
        Dim MailMsg As New MailMessage
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As _
            New StreamReader(System.Web.HttpContext.Current.Server.MapPath("../EmailTemplates/removeSupport.html"))
        Dim outMsgBody As String = ""

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(theSupporterEmail))
        currentMember = getMemberInformation(goalInfo.userName)
        If currentMember IsNot Nothing Then
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True
            MailMsg.Subject = "Sweet Goals - Removal of Support Goal: " & goalInfo.goalTitle

            outMsgBody = goalSupportForm.ReadToEnd()
            outMsgBody = outMsgBody.Replace("[goalTitle]", goalInfo.goalTitle)
            outMsgBody = outMsgBody.Replace("[currentUserEmailAddress]", currentMember.Email)
            outMsgBody = outMsgBody.Replace("[currentGoalUsername]", goalInfo.userName)
            outMsgBody = outMsgBody.Replace("[supportMessage]", messageToSupporter)
            outMsgBody = outMsgBody.Replace("[supporterEmailAddress]", theSupporterEmail)
            MailMsg.Body = outMsgBody
        End If
        Return MailMsg
    End Function
End Class
