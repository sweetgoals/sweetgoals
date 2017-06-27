Imports System.Net
Imports System.Net.Mail
Imports System.IO

Partial Class backend_response
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim eData As New emailtrack

        unsubLabel.Visible = False
        If Request.QueryString.HasKeys Then
            If Request.QueryString("d") = "yes" Then
                simpleResponse.Visible = True
                mainDetail.Visible = False
                simpleMsg.Text = "<h1> SWEET!! We'll Be In Touch!!! </h1>"
                eData = getEmailTrackData()
                buttonProcedures(eData, 1)
            ElseIf Request.QueryString("d") = "no" Then
                simpleResponse.Visible = True
                mainDetail.Visible = False
                simpleMsg.Text = "<h1> Alright no worries.</h1>"
                eData = getEmailTrackData()
                buttonProcedures(eData, -1)
            ElseIf Request.QueryString("delemail") IsNot Nothing Then
                simpleResponse.Visible = True
                mainDetail.Visible = False
                simpleMsg.Text = "<h1> Unsubscribed. </h1> <br> If you wish to ever subscribe again you'll need to create "
                simpleMsg.Text &= "an account and go to settings and remove the unsubscribe setting."
                unsubscribe(Request.QueryString("delemail"))
            ElseIf Request.QueryString("mainman") = "y" Then
                simpleResponse.Visible = True
                mainDetail.Visible = False
                simpleMsg.Text = "<h1> Main Man </h1> <br> Dblake is my main man. He'll be along."
                setupDefaultSupporter(Request.QueryString("gnum"))
            ElseIf Request.QueryString("va") IsNot Nothing Then
                activateNewUser()
            Else
                displayGoalDetail()
            End If
        Else
            standardResponse()
        End If
    End Sub

    Sub standardResponse()
        simpleResponse.Visible = True
        mainDetail.Visible = False
        simpleMsg.Text = "<h1> Nothing here. I am a Meat Popsicle</h1>"
    End Sub
    Sub activateNewUser()
        Dim userInfo As Membershipdb

        userInfo = (From ui In db.Membershipdbs
                        Where ui.UserId.ToString = Request.QueryString("va")
                        Select ui).FirstOrDefault
        mainDetail.Visible = False
        simpleResponse.Visible = True
        If userInfo IsNot Nothing Then
            sendMeAnEmail(userInfo.UserId.ToString())
            userInfo.IsApproved = True
            db.SubmitChanges()
            simpleMsg.Text = "<h1> User Approved </h1> <br> Your Account has been approved! <br> Now all you need to do is"
            simpleMsg.Text &= "logon.Sweet! "
        Else
            simpleMsg.Text = "<H1> User Not Approved </h1> <br> So sorry, you'll need to recreate your account<br>"
        End If
    End Sub


    Sub sendMeAnEmail(ByVal username As String)
        Dim MailMsg As New MailMessage

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress("apchampagne@gmail.com"))
        MailMsg.Priority = MailPriority.Normal
        MailMsg.IsBodyHtml = True
        MailMsg.Subject = "Sweet Goals - Account Activation"
        MailMsg.Body = username & " created an account on sweetgoals.com"
        setMailServer(MailMsg)
    End Sub
    Sub setupDefaultSupporter(ByVal goalNum As String)
        Dim defaultSupporter As New goalSupport

        defaultSupporter.goalNum = goalNum
        defaultSupporter.sEmail = "apchampagne@gmail.com"
        db.goalSupports.InsertOnSubmit(defaultSupporter)
        db.SubmitChanges()
    End Sub

    Protected Sub yesButton_Click(sender As Object, e As EventArgs) Handles yesButton.Click
        Dim eData As New emailtrack

        simpleResponse.Visible = True
        mainDetail.Visible = False
        simpleMsg.Text = "<h1> SWEET!! We'll Be In Touch!!! </h1>"
        eData = getEmailTrackData()
        buttonProcedures(eData, 1)
    End Sub

    Protected Sub noButton_Click(sender As Object, e As EventArgs) Handles noButton.Click
        Dim eData As New emailtrack

        simpleResponse.Visible = True
        mainDetail.Visible = False
        simpleMsg.Text = "<h1> Alright no worries.</h1>"
        eData = getEmailTrackData()
        buttonProcedures(eData, -1)
    End Sub

    Protected Sub unsubscribeButton_Click(sender As Object, e As EventArgs) Handles unsubscribeButton.Click
        Dim etrack As New emailtrack

        If Request.QueryString.AllKeys.Contains("r") Then
            etrack = (From et In db.emailtracks
                        Where et.refNumber = Convert.ToInt64(Request.QueryString("r"))
                        Select et).FirstOrDefault
            unsubscribe(etrack.email)
            unsubLabel.Visible = True
        End If
    End Sub

    Sub displayGoalDetail()
        Dim theGoal As New goal
        Dim theactivity As New activity
        Dim eData As New emailtrack

        eData = getEmailTrackData()
        If eData IsNot Nothing Then
            If eData.type.Contains("rgSupport") Or
               eData.type.Contains("vAct") Or
               eData.type.Contains("cGoal") Then
                If eData.type.Contains("rgSupport") Then
                    supMsgText.Visible = False
                End If
                If (Not eData Is Nothing) Then
                    msgOut(eData)
                    theGoal = (From g In db.goals
                       Where g.goalNumber = eData.goalNum
                       Select g).SingleOrDefault
                End If
                If Not theGoal Is Nothing Then
                    helpCreateRow("Description", theGoal.goalDesc)
                    helpCreateRow("Time Allocated", theGoal.timeLength & " " & theGoal.timeUnit)
                    helpCreateRow("Schedule", theGoal.scheduleDays)
                    helpCreateRow("Goal Due Date", theGoal.goalDueDate)
                    helpCreateRow("Total Time", goalTotalTime(theGoal.goalNumber))
                    If eData.actNum > 0 Then
                        theactivity = (From a In db.activities
                                       Where a.actNumber = eData.actNum
                                       Select a).FirstOrDefault
                        If Not theactivity Is Nothing Then
                            If theactivity.goalNumber <> theGoal.goalNumber Then
                                theactivity = Nothing
                            End If
                            displayActivity(theactivity, theGoal)
                            displayPictures(theactivity.actNumber)
                            displayGoalActivities(theGoal.goalNumber, eData.actNum)
                        End If
                    Else
                        displayGoalActivities(theGoal.goalNumber)
                    End If
                End If
            Else
                standardResponse()
            End If
        Else : standardResponse()
        End If
    End Sub

    Sub displayActivity(ByVal theactivity As activity, _
                        ByVal theGoal As goal)
        Dim actCount As Integer = 0

        actCount = (From m In db.activities _
                       Where m.goalNumber = theGoal.goalNumber And m.status.Contains("Complete")
                       Select m).Count
        If theactivity.actNumber <> 0 Then
            helpCreateRow("Activities", actCount.ToString)
            helpCreateRow("<b><h2>Activity Title</h2></b>", "<b><h2>" & theactivity.actTitle & "</h2></b>")
            helpCreateRow("Description", theactivity.actDesc)
            helpCreateRow("Date", theactivity.actDate)
            helpCreateRow("Good", theactivity.good)
            helpCreateRow("Bad", theactivity.bad)
            helpCreateRow("Start Time", theactivity.startTime)
            helpCreateRow("Stop Time", theactivity.stopTime)
            helpCreateRow("Duration", theactivity.timeDiff)
            helpCreateRow("Status", theactivity.status)
        End If
    End Sub

    Sub displayGoalActivities(ByVal goalNum As Integer, Optional ByVal ac As Integer = 0)
        'ac = 0 means it's a activity review just show 3 activities
        'ac >0 means its a goal review so show all activities
        Dim arow As New TableRow
        Dim values As New List(Of String)
        Dim theActsComplete As IQueryable(Of activity)

        values = New List(Of String)
        values.Add("<b>Title</b>")
        values.Add("<b>Date</b>")
        values.Add("<b>Description</b>")
        values.Add("<b>Time Spent</b>")
        values.Add("<b>Status</b>")
        arow = helpCreateRow(values)
        actTable.Rows.Add(arow)
        If ac = 0 Then
            theActsComplete = getActivities(goalNum, "complete")
            createActTable(theActsComplete)
            theActsComplete = getActivities(goalNum, "created")
            createActTable(theActsComplete)
            theActsComplete = getActivities(goalNum, "fail")
            createActTable(theActsComplete)
            msgLabel.Text = "Based on the Evidence Presented did they complete the Goal?"
        Else
            theActsComplete = From a In db.activities
                      Where (a.goalNumber = goalNum And a.status.Contains("complete"))
                      Order By Convert.ToDateTime(a.actDate) Descending
            theActsComplete = getActivities(goalNum)
            createActTable(theActsComplete, 3)
        End If
    End Sub

    Sub displayPictures(ByVal actNum As Integer)
        Dim actPics As IQueryable(Of activityPicture)
        Dim arow As TableRow = Nothing
        Dim acell As TableCell = Nothing
        Dim pImage As Image = Nothing
        Dim hAnchor As New HtmlAnchor
        Dim multirow As Boolean = False
        Dim i As Integer = 0

        picTable.Visible = True
        picTable.Rows.Clear()
        actPics = From p In db.activityPictures
                  Where p.actNumber = actNum
                  Select p

        'For Each pic In actPics
        arow = New TableRow
        acell = New TableCell
        pImage = New Image
        i = 0
        For Each p In actPics
            acell = New TableCell
            pImage = New Image
            hAnchor = New HtmlAnchor
            pImage.ImageUrl = p.pictureLocation
            pImage.AlternateText = p.picDesc
            pImage.Attributes("rel") = "prettyPhoto[gallery1]"
            pImage.Width = 100
            pImage.Height = 100

            hAnchor.HRef = p.pictureLocation
            hAnchor.Attributes("rel") = "prettyPhoto[gallery1]"
            hAnchor.Controls.Add(pImage)
            acell.Controls.Add(hAnchor)
            arow.Cells.Add(acell)
            If i = 3 Then
                picTable.Rows.Add(arow)
                arow = New TableRow
                i = 0
                multirow = True
            End If
            i += 1
        Next
        picTable.Rows.Add(arow)
    End Sub

    Sub msgOut(ByVal etrack As emailtrack)
        'email type definition
        'vAct - verify activity
        'cGoal - complete goal
        'sGoal - suspend goal
        'rGoal - restart goal
        'unsub - unsubscribe

        If Request.QueryString.AllKeys.Contains("delemail") Then
            msgLabel.Text = "Click yes to confirm unsubscription"
        ElseIf etrack.type.Contains("cGoal") Then
            msgLabel.Text = "Please verify completed goal. Did they do it or not?"
            'ElseIf etrack.type.Contains("sGoal") Then
            '    msgLabel.Text = "Goal got suspended. Anything to say?"
        ElseIf etrack.type.Contains("rGoal") Then
            msgLabel.Text = "They restarted the goal"
        ElseIf etrack.type.Contains("raSupport") Then
            msgLabel.Text = "Would you like to support this person in completing their goals?"
        ElseIf etrack.type.Contains("rgSupport") Then
            msgLabel.Text = "Would you like to support this goal?"
        ElseIf etrack.type.Contains("vAct") Then
            msgLabel.Text = "Please verify Activity. Did they do it or not?"
        End If
    End Sub

    Function getActivities(ByVal gn As Integer, _
                           Optional ByVal status As String = "") As IQueryable(Of activity)
        Dim theActs As IQueryable(Of activity)

        If status.Length > 0 Then
            theActs = From a In db.activities
              Where (a.goalNumber = gn And a.status.Contains(status))
              Order By Convert.ToDateTime(a.actDate) Descending
        Else
            theActs = From a In db.activities
              Where a.goalNumber = gn
              Order By Convert.ToDateTime(a.actDate) Descending
        End If

        Return theActs
    End Function

    Sub createActTable(ByVal acts As IQueryable(Of activity), _
                       Optional ByVal nRows As Integer = 0)
        Dim values As New List(Of String)
        Dim arow As New TableRow
        Dim i As Integer = 0

        For Each a In acts
            values = New List(Of String)
            values.Add(a.actTitle)
            values.Add(a.actDate)
            values.Add(a.actDesc)
            values.Add(a.timeDiff)
            values.Add(a.status)
            arow = helpCreateRow(values)
            actTable.Rows.Add(arow)
            i += 1
            If (nRows > 0 And i = nRows) Then
                Exit Sub
            End If
        Next
    End Sub

    Sub helpCreateRow(ByVal rowTitle As String, _
                      ByVal cellInfo As String)
        Dim arow As New TableRow
        Dim acell As New TableCell

        acell.Text = rowTitle
        arow.Cells.Add(acell)
        acell = New TableCell
        acell.Text = cellInfo
        arow.Cells.Add(acell)
        goalDetailTable.Rows.Add(arow)
    End Sub

    Function helpCreateRow(ByVal values As List(Of String)) As TableRow
        Dim acell As New TableCell
        Dim arow As New TableRow

        For Each v In values
            acell = New TableCell
            acell.Text = v
            arow.Cells.Add(acell)
        Next
        Return arow
    End Function

    Function getEmailTrackData() As emailtrack
        Dim eData As New emailtrack
        Dim rNumber As Integer = 0

        If Request.QueryString.AllKeys.Contains("r") Then
            rNumber = Convert.ToInt64(Request.QueryString("r"))
        ElseIf Request.QueryString.AllKeys.Contains("delemail") Then
            rNumber = Convert.ToInt64(Request.QueryString("delemail"))
        End If
        If rNumber > 0 Then
            eData = (From b In db.emailtracks
                     Where b.refNumber = rNumber
                     Select b).FirstOrDefault
            Return eData
        Else : Return Nothing
        End If
    End Function

    Sub buttonProcedures(ByVal eData As emailtrack,
                         ByVal decision As Integer)
        'vAct - Verify Activity
        'cGoal - Complete Goal
        'sGoal - Suspend Goal
        'rGoal - Restart Goal
        'rgSupport - Request Support for a Goal

        Dim eType As String = ""

        If Not eData Is Nothing Then
            eType = eData.type.ToString.Trim
            If Request.QueryString.AllKeys.Contains("delemail") Then
                unsubscribe(eData.email)
            ElseIf eType = "vAct" Then
                verifyEmail(eData, decision)
            ElseIf eType = "rgSupport" Then
                support(eData, decision)
            ElseIf eType = "cGoal" Then
                insertResponse(eData)
            ElseIf eType = "sGoal" Then
                insertResponse(eData)
            ElseIf eType = "rGoal" Then
                insertResponse(eData)
            End If
        End If
        'redirect()
    End Sub

    Sub verifyEmail(ByVal eData As emailtrack, _
                    ByVal decision As Integer)
        Dim actNumber As Integer = -1
        Dim supportEmail As String = ""
        Dim theAct As New activity
        Dim theResponse As New activityResponse
        Dim mainSupport As New goalSupport
        Dim supInfo As New supporter
        Dim sMember As New Membershipdb
        Dim sUser As New User

        actNumber = eData.actNum
        supportEmail = eData.email
        theAct = (From a In db.activities
                Where a.actNumber = eData.actNum
                Select a).FirstOrDefault
        mainSupport = (From s In db.goalSupports
                            Where s.goalNum = eData.goalNum And s.sEmail = eData.email
                            Select s).FirstOrDefault
        If mainSupport IsNot Nothing Then
            supInfo = (From s In db.supporters
                        Where s.supportEmail = mainSupport.sEmail
                        Select s).FirstOrDefault
            sMember = (From sm In db.Membershipdbs
                            Where sm.Email.Contains(supInfo.supportEmail)
                            Select sm).FirstOrDefault
            If sMember IsNot Nothing Then
                sUser = (From su In db.Users
                            Where su.UserId = sMember.UserId
                            Select su).FirstOrDefault
            End If

            If supInfo IsNot Nothing Then
                theAct.status = "Complete"
                theResponse = New activityResponse
                theResponse.actNumber = theAct.actNumber
                theResponse.sNumber = supInfo.supportNumber
                theResponse.pfResult = decision
                If decision = 1 Then
                    theResponse.response = "Completed "
                    If supMsgText.Text.Length > 0 And
                       checkWhiteSpace(supMsgText.Text) = False Then
                        theResponse.response &= supMsgText.Text
                    End If
                Else
                    theResponse.response = "Incomplete "
                    If supMsgText.Text.Length > 0 And
                       checkWhiteSpace(supMsgText.Text) = False Then
                        theResponse.response &= supMsgText.Text
                    End If
                End If
                theResponse.goalNumber = theAct.goalNumber
                theResponse.type = eData.type
                theResponse.rDate = Today.Date.ToShortDateString
                If sUser IsNot Nothing Then
                    theResponse.userName = sUser.UserName
                Else
                    theResponse.userName = supInfo.supportEmail
                End If
                db.activityResponses.InsertOnSubmit(theResponse)
                eData.processed = 1
                db.SubmitChanges()
            End If
        End If
    End Sub

    Sub unsubscribe(ByVal emailAddress As String)
        Dim unsub As New unsubscribe
        Dim rGoalSupport As IQueryable(Of goalSupport)
        Dim rSupport As IQueryable(Of supporter)

        If emailAddress.Contains("@") Then
            rGoalSupport = From rg In db.goalSupports
                           Where rg.sEmail = emailAddress
            rSupport = From rs In db.supporters
                        Where rs.supportEmail = emailAddress
            For Each rg In rGoalSupport
                db.goalSupports.DeleteOnSubmit(rg)
            Next
            For Each rs In rSupport
                db.supporters.DeleteOnSubmit(rs)
            Next
            unsub.email = emailAddress
            db.unsubscribes.InsertOnSubmit(unsub)
            db.SubmitChanges()
        End If
    End Sub

    Sub support(ByVal eData As emailtrack,
                ByVal decision As Integer)
        Dim theUser As New User
        Dim theGoal As New goal
        Dim theMembershipdb As New Membershipdb
        Dim checkUnsub As New unsubscribe
        Dim sResponse As New activityResponse
        Dim aaSupport As New supporter
        Dim agSupport As New goalSupport
        Dim checkExistingUser As New supporter
        Dim checkExistingGoalSupport As New goalSupport

        theGoal = (From g In db.goals
                    Where g.goalNumber = eData.goalNum
                    Select g).FirstOrDefault
        theUser = (From s In db.Users
                    Where s.UserName = theGoal.userName
                    Select s).SingleOrDefault
        theMembershipdb = (From m In db.Membershipdbs
                            Where m.UserId = theUser.UserId
                            Select m).SingleOrDefault
        If decision = 1 Then
            eData.processed = 1
            checkUnsub = (From em In db.unsubscribes
                            Where em.email = eData.email
                            Select em).FirstOrDefault
            If checkUnsub IsNot Nothing Then
                db.unsubscribes.DeleteOnSubmit(checkUnsub)
            Else
                checkExistingUser = (From s In db.supporters
                                        Where s.supportEmail.Contains(eData.email) And s.userName.Contains(theUser.UserName)
                                        Select s).FirstOrDefault
                If checkExistingUser Is Nothing Then
                    aaSupport = New supporter
                    aaSupport.supportEmail = eData.email
                    aaSupport.userName = theUser.UserName
                    db.supporters.InsertOnSubmit(aaSupport)
                    db.SubmitChanges()
                    If checkWhiteSpace(supMsgText.Text) = True Then
                        sendAcceptSupportEmail(eData, theMembershipdb.Email(), False)
                    Else
                        sResponse = New activityResponse
                        sResponse.goalNumber = eData.goalNum
                        sResponse.response = supMsgText.Text
                        sResponse.sNumber = (From s In db.supporters
                                                Where s.supportEmail = eData.email
                                                Select s.supportNumber).FirstOrDefault
                        sResponse.pfResult = 1
                        sResponse.type = "vaResponse"
                        db.activityResponses.InsertOnSubmit(sResponse)
                        sendAcceptSupportEmail(eData, theMembershipdb.Email(), False)
                    End If
                End If
                If eData.type.Contains("rgSupport") Then
                    checkExistingGoalSupport = (From gs In db.goalSupports
                                                    Where gs.goalNum = eData.goalNum And gs.sEmail.Contains(eData.email)
                                                    Select gs).FirstOrDefault
                    If checkExistingGoalSupport Is Nothing Then
                        agSupport.goalNum = eData.goalNum
                        agSupport.sEmail = eData.email
                        db.goalSupports.InsertOnSubmit(agSupport)
                        db.SubmitChanges()
                        If checkWhiteSpace(supMsgText.Text) = True Then
                            sendAcceptSupportEmail(eData, theMembershipdb.Email(), True)
                        Else
                            sResponse = New activityResponse
                            sResponse.goalNumber = eData.goalNum
                            sResponse.response = supMsgText.Text
                            sResponse.sNumber = (From s In db.supporters
                                                    Where s.supportEmail = eData.email
                                                    Select s.supportNumber).FirstOrDefault
                            sResponse.pfResult = 1
                            sResponse.type = "vgResponse"
                            db.activityResponses.InsertOnSubmit(sResponse)
                            sendAcceptSupportEmail(eData, theMembershipdb.Email(), True)
                        End If
                    End If
                End If
            End If
        ElseIf decision = -1 Then
            eData.processed = -1
            sendRejectSupportEmail(eData, theMembershipdb.Email())
        End If
        db.SubmitChanges()
    End Sub

    Sub sendAcceptSupportEmail(ByVal eData As emailtrack,
                        ByVal uEmail As String,
                        ByVal sType As Boolean)
        'sType = false then its account support
        'sType = true then its goal Support
        Dim MailMsg As New MailMessage
        Dim theGoal As New goal

        theGoal = (From g In db.goals
                    Where g.goalNumber = eData.goalNum
                    Select g).FirstOrDefault
        If mailModule.inUnsubscribes(uEmail) = False Then
            MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(uEmail))
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True
            MailMsg.BodyEncoding = Encoding.Default
            MailMsg.Subject = "Sweet Goals - Accept Support Request"
            If sType = False Then
                MailMsg.Body = "Sweet!, " & eData.email & " wants to support you!"
            Else
                MailMsg.Body = "Sweet!, " & eData.email & " wants to support your goal " & theGoal.goalTitle
            End If

            mailModule.setMailServer(MailMsg)
        End If
    End Sub

    Sub sendRejectSupportEmail(ByVal eData As emailtrack, _
                               ByVal uEmail As String)
        Dim MailMsg As New MailMessage
        Dim theGoal As New goal

        theGoal = (From g In db.goals
                    Where g.goalNumber = eData.goalNum
                    Select g).FirstOrDefault
        If mailModule.inUnsubscribes(uEmail) = False Then
            MailMsg = createRejectSupportEmail(eData, uEmail)
            mailModule.setMailServer(MailMsg)
        End If
    End Sub

    Function createRejectSupportEmail(ByVal eData As emailtrack,
                                       ByVal uEmail As String) As MailMessage
        Dim MailMsg As New MailMessage
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As New  _
            StreamReader(System.Web.HttpContext.Current.Server.MapPath("../EmailTemplates/declineSupport.html"))
        Dim outMsgBody As String = ""
        Dim thegoal As goal = Nothing

        thegoal = (From tg In db.goals
                        Where tg.goalNumber = eData.goalNum
                        Select tg).FirstOrDefault

        currentMember = getMemberInformation(thegoal.userName)
        If currentMember IsNot Nothing Then
            MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(uEmail))
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True
            MailMsg.Subject = "Sweet Goals - Rejected Support Request for Goal " & thegoal.goalTitle
            outMsgBody = goalSupportForm.ReadToEnd()
            outMsgBody = outMsgBody.Replace("[goalNumber]", thegoal.goalNumber)
            outMsgBody = outMsgBody.Replace("[goalTitle]", thegoal.goalTitle)
            outMsgBody = outMsgBody.Replace("[supporterEmailAddress]", eData.email)
            MailMsg.Body = outMsgBody
        End If
        Return MailMsg
    End Function

    Sub insertResponse(ByVal eData As emailtrack)
        Dim theSupport As New supporter
        Dim theResponse As New activityResponse

        theSupport = (From s In db.supporters
                    Where s.supportEmail = eData.email
                    Select s).FirstOrDefault

        If Not theSupport Is Nothing And _
            checkWhiteSpace(supMsgText.Text) = False Then
            theResponse.actNumber = -1
            theResponse.sNumber = theSupport.supportNumber
            theResponse.pfResult = 1
            theResponse.goalNumber = eData.goalNum
            theResponse.response = supMsgText.Text
            db.activityResponses.InsertOnSubmit(theResponse)
            db.SubmitChanges()
        End If

    End Sub

    Function checkWhiteSpace(ByVal checkString As String) As Boolean
        Dim spaceCount As Integer = 0

        spaceCount = checkString.ToString.Count - checkString.ToString.Replace(" ", "").Count
        If spaceCount = checkString.Length Then
            Return True
        Else : Return False
        End If
    End Function

    Function goalTotalTime(ByVal goalNumber As Integer) As String
        Dim db As New sgdataDataContext
        Dim currentGoalActs As IQueryable(Of activity)
        Dim timeDiff As TimeSpan
        Dim days As Integer = 0
        Dim hours As Integer = 0
        Dim minutes As Integer = 0

        currentGoalActs = From acts In db.activities
                          Where acts.goalNumber = goalNumber
        For Each goalActs In currentGoalActs
            Try
                timeDiff += TimeSpan.Parse(goalActs.timeDiff)
            Catch ex As Exception
            End Try
        Next
        days = timeDiff.Days
        hours = timeDiff.Hours
        minutes = timeDiff.Minutes

        Return days & " Days, " & hours & " Hrs, " & minutes & " Min"
    End Function

    Sub redirect()
        If UserLoggedIn() Then
            Response.Redirect("../Account/Login.aspx")
        Else
            Response.Redirect("~/Default.aspx")
        End If
    End Sub

    Sub activityResponseSetup()
        If checkResponse() = False Then
            supMsgText.Visible = True
            yesButton.Visible = True
            If Convert.ToInt32(Request.QueryString("confirm")) = 1 Then
                msgLabel.Text = "Thanks for the Support. Want to leave a supportive message? Like..."
            Else : msgLabel.Text = "What was the problem? "
            End If
        Else : msgLabel.Text = "Already have that one"
        End If
    End Sub

    Function checkResponse() As Boolean
        'returns true if response is found

        Dim findResponse As Integer = -1
        Dim theAct As New activity
        Dim theSupport As New supporter
        Dim actNumber As Integer = -1
        Dim supportNumber As Integer = -1

        actNumber = Convert.ToInt32(Request.QueryString("actNumber"))
        supportNumber = Convert.ToInt32(Request.QueryString("email"))
        theAct = (From a In db.activities
                Where a.actNumber = actNumber
                Select a).FirstOrDefault
        theSupport = (From s In db.supporters
                    Where s.supportNumber = supportNumber
                    Select s).FirstOrDefault

        findResponse = (From m In db.activityResponses _
                       Where m.actNumber = theAct.actNumber _
                       And m.sNumber = theSupport.supportNumber _
                       Select m.responseNumber).Count()
        If findResponse > 0 Then
            Return True
        Else : Return False
        End If
    End Function
End Class
