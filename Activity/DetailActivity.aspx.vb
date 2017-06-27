Imports System.IO
Imports System.Data
Imports System.Net.Mail
Imports System.Drawing.Image
Imports System.Drawing.Bitmap


Partial Class actdetail
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

#Region "Page: Page_PreRender, Page_Load, Page_LoadComplete "
    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        ViewState("checkRefresh") = Session("checkRefresh")
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session("checkRefresh") = Server.UrlDecode(System.DateTime.Now.ToString)
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Request.QueryString.HasKeys Then
            displayAct()
        End If
    End Sub
#End Region

#Region "Buttons: createActButton_Click, deleteActButton_Click, editActButton_Click, picDeleteButton_Click, "

    Protected Sub createActButton_Click(sender As Object, e As EventArgs) Handles createActButton.Click
        Dim refAct As New activity
        Dim theAct As New activity
        Dim theGoal As New goal
        Dim goalNumber As Integer = -1
        Dim i As Integer = 0
        Dim createdActivityList As IQueryable(Of activity)
        Dim userType As String = ""

        Try
            goalNumber = Convert.ToInt32(Request.QueryString("gNum"))
        Catch ex As Exception
            kickUser()
        End Try
        userType = goalModule.checkGoalNumber(goalNumber, userName)
        If userType.Length > 0 Then
            theGoal = (From g In db.goals
                       Where g.goalNumber = goalNumber
                       Select g).SingleOrDefault
            theAct.goalNumber = theGoal.goalNumber
            If userType.Contains("Supporter") Then
                theAct.status = "Suggested"
                theAct.sequence = (From g In db.activities
                                   Where g.goalNumber = goalNumber And g.status = "Suggested"
                                   Select g).Count
                Session("achs") = achievementModule.suggestActivity(userName)
            Else : theAct.status = "Created"
                createdActivityList = From cal In db.activities
                                      Where cal.goalNumber = goalNumber And cal.sequence <> -1 And
                                            cal.status.Contains("Created")
                                      Order By cal.sequence Ascending
                theAct.startTime = ""
                theAct.sequence = 1
                i = 2
                For Each cal In createdActivityList
                    cal.sequence = i
                    db.SubmitChanges()
                    i += 1
                Next
                Session("achs") = achievementModule.createActivity(userName)
            End If
            theAct.actDate = Today.Date.ToShortDateString
            theAct.actTitle = userName()
            db.activities.InsertOnSubmit(theAct)
            db.SubmitChanges()

            refAct = (From ra In db.activities
                      Where ra.actTitle.Contains(userName())
                      Select ra).FirstOrDefault
            If refAct IsNot Nothing Then
                refAct.actTitle = "custom"
                parseTextControlData(refAct.actNumber)
            End If
            If Request.QueryString("goalTypeList") = "suggested" Then
                Response.Redirect("ListActivity.aspx?gNum=" & goalNumber &
                                  "&activityTypeList=created&goalTypeList=suggested")
            Else : Response.Redirect("ListActivity.aspx?gNum=" & goalNumber & "&activityTypeList=created")
            End If
        Else
            kickUser()
        End If
    End Sub

    Protected Sub deleteActButton_Click(sender As Object, e As EventArgs) Handles deleteActButton.Click
        Dim act As New activity
        Dim activityTextControl As List(Of activityTextControl) = New List(Of activityTextControl)
        Dim theGoal As New goal
        Dim actNum As Integer = -1
        Dim goalNum As Integer = -1

        getGoalActNumbers(goalNum, actNum)
        If goalNum <> -1 And actNum <> -1 Then
            theGoal = (From g In db.goals
                       Where g.goalNumber = goalNum
                       Select g).FirstOrDefault
            act = (From a In db.activities
                   Where a.actNumber = actNum
                   Select a).FirstOrDefault
            activityTextControl = (From atc In db.activityTextControls
                                   Where atc.actnum.Contains(actNum)).ToList()

            If activityTextControl.Count > 0 Then
                db.activityTextControls.DeleteAllOnSubmit(activityTextControl)
            End If
            db.activities.DeleteOnSubmit(act)

            db.SubmitChanges()
            Response.Redirect("ListActivity.aspx?gNum=" & goalNum & "&activityTypeList=created")
        End If
    End Sub

    Protected Sub editActButton_Click(sender As Object, e As EventArgs) Handles editActButton.Click
        Dim goalNum As Integer = 0
        Dim actNum As Integer = 0
        Dim theGoal As New goal
        Dim act As New activity
        Dim userType As String = ""

        getGoalActNumbers(goalNum, actNum)
        If goalNum <> -1 And actNum <> -1 Then
            theGoal = (From g In db.goals
                       Where g.goalNumber = goalNum
                       Select g).FirstOrDefault
            act = (From a In db.activities
                   Where a.actNumber = actNum
                   Select a).FirstOrDefault
            userType = goalModule.checkGoalNumber(theGoal.goalNumber, userName)
            If userType.Contains("User") Then
                act.status = "Edit"
                activityEditEnableHidden.Value = True
                db.SubmitChanges()
            End If
        End If
    End Sub

    Protected Sub picDeleteButton_Click(sender As Object, e As EventArgs) Handles picDeleteButton.Click
        Dim userType As String = ""
        Dim theGoal As New goal
        Dim actNum As Integer = -1
        Dim goalNum As Integer = -1
        Dim pictureNumber As Integer = -1
        Dim actPic As activityPicture = Nothing
        Dim serverPath As String = ""
        Dim midSize640x480Path As String = ""
        Dim thumbNailPath As String = ""
        Dim picFile As String = ""

        pictureNumber = pictureNumberHidden.Value.Replace("FeaturedContent_share", "")
        serverPath = Server.MapPath("../pictures/actpic/")
        midSize640x480Path = Server.MapPath("../pictures/midsize/")
        thumbNailPath = Server.MapPath("../pictures/thumbnail/")

        actPic = (From ap In db.activityPictures
                  Where ap.pictureNumber = pictureNumber
                  Select ap).FirstOrDefault

        If actPic IsNot Nothing Then
            getGoalActNumbers(goalNum, actNum)
            userType = goalModule.checkGoalNumber(goalNum, userName)
            If userType.Contains("User") Then
                theGoal = (From g In db.goals
                           Where g.goalNumber = goalNum
                           Select g).FirstOrDefault
                If theGoal.goalNumber = actPic.goalNumber Then
                    parseTextControlData(actNum.ToString)
                    picFile = actPic.pictureLocation.Replace("../pictures/actpic/", "")
                    Try
                        If System.IO.File.Exists(serverPath & picFile) = True Then
                            System.IO.File.Delete(serverPath & picFile)
                        End If
                        If System.IO.File.Exists(midSize640x480Path & picFile) = True Then
                            System.IO.File.Delete(midSize640x480Path & picFile)
                        End If
                        If System.IO.File.Exists(thumbNailPath & picFile) = True Then
                            System.IO.File.Delete(thumbNailPath & picFile)
                        End If
                        db.activityPictures.DeleteOnSubmit(actPic)
                        db.SubmitChanges()
                    Catch ex As Exception

                    End Try
                Else
                    Response.Redirect("~/Default.aspx")
                End If
            End If
        End If
    End Sub
#End Region

#Region "Buttons: startActButton_Click, stopActButton_Click, submitCommentButton_Click,submitPicButton_Click"
    Protected Sub startActButton_Click(sender As Object, e As EventArgs) Handles startActButton.Click
        Dim act As New activity
        Dim actNum As Integer = -1
        Dim goalNum As Integer = -1
        Dim theGoal As New goal
        Dim customFields As New activityField
        Dim customFieldsString() As String = {}
        Dim customItem As New activityFieldData
        Dim customText As String = ""
        Dim j As Integer = 0
        Dim refAct As New activity

        getGoalActNumbers(goalNum, actNum)
        If goalNum <> -1 Then
            theGoal = (From g In db.goals
                       Where g.goalNumber = goalNum
                       Select g).FirstOrDefault
            If actNum > 0 Then
                act = (From a In db.activities
                       Where a.actNumber = actNum
                       Select a).FirstOrDefault
                act.startTime = TimeOfDay.TimeOfDay.ToString
                act.actDate = Today.Date.ToShortDateString
                act.status = "Start"
                act.actTitle = userName()
                Session("achs") = achievementModule.startActivity(userName)
                db.SubmitChanges()
                refAct = (From ra In db.activities
                          Where ra.actTitle.Contains(userName())
                          Select ra).FirstOrDefault
                If refAct IsNot Nothing Then
                    refAct.actTitle = "custom"
                    parseTextControlData(refAct.actNumber)
                End If
                Response.Redirect("ListActivity.aspx?gNum=" & goalNum & "&activityTypeList=completed")
            ElseIf startNewActivity(theGoal) Then
                Response.Redirect("ListActivity.aspx?gNum=" & goalNum & "&activityTypeList=completed")
            End If
        End If
    End Sub

    Protected Sub stopActButton_Click(sender As Object, e As EventArgs) Handles stopActButton.Click,
                                                                            stopActButtonDialog.Click
        Dim act As New activity
        Dim theGoal As New goal
        Dim actNum As Integer = -1
        Dim goalNum As Integer = -1

        getGoalActNumbers(goalNum, actNum)
        If goalNum > 0 Then
            theGoal = (From g In db.goals
                       Where g.goalNumber = goalNum
                       Select g).FirstOrDefault
            If actNum > 0 Then
                act = (From a In db.activities
                       Where a.actNumber = actNum
                       Select a).FirstOrDefault
                stopActivity(theGoal, act)
            ElseIf startNewActivity(theGoal) Then
                stopActivity(theGoal)
            End If
            Response.Redirect("ListActivity.aspx?gNum=" & goalNum)
        End If
        kickUser()
    End Sub



    Protected Sub submitCommentButton_Click(sender As Object, e As EventArgs) Handles commentButton.Click
        Dim goalNum As Long = 0
        Dim actNum As Long = 0
        Dim theGoal As New goal
        Dim act As New activity
        Dim actResponse As New activityResponse
        Dim checkSpaces As String = ""

        'makes it so comments can't be blank or just have spaces in them
        If commentInsert.Text.Replace(" ", "").Length > 0 Then
            getGoalActNumbers(goalNum, actNum)
            If goalNum <> -1 And actNum <> -1 Then
                theGoal = (From g In db.goals
                           Where g.goalNumber = goalNum
                           Select g).FirstOrDefault
                act = (From a In db.activities
                       Where a.actNumber = actNum
                       Select a).FirstOrDefault
                actResponse.actNumber = act.actNumber
                actResponse.userName = User.Identity.Name
                actResponse.goalNumber = theGoal.goalNumber
                actResponse.response = Server.HtmlEncode(commentInsert.Text)
                actResponse.rDate = Today.Date.ToShortDateString
                db.activityResponses.InsertOnSubmit(actResponse)
                db.SubmitChanges()
            End If
        End If
        commentInsert.Text = ""
    End Sub

    Protected Sub submitPicButton_Click(sender As Object, e As EventArgs) Handles submitPicButton.Click
        Dim actNum As Integer = -1
        Dim gNum As Long = -1
        Dim achTitle As String = ""

        If ViewState("checkRefresh").ToString = Session("checkRefresh") Then
            Session("checkRefresh") = DateTime.Now.ToString
            getGoalActNumbers(gNum, actNum)
            If (actNum <> -1) And (gNum <> -1) Then
                If savePictures(picFile, actNum, gNum) Then
                    parseTextControlData(actNum.ToString)
                    achTitle = achievementModule.pictureActivity(HttpContext.Current.User.Identity.Name)
                    If achTitle.Length > 0 Then
                        ClientScript.RegisterStartupScript(Me.GetType(), "PopUp", "showAchievementPopup('" & achTitle & "');",
                                                           True)
                    End If
                End If
            End If
        Else
            picFile = New FileUpload
        End If
    End Sub

#End Region

    <System.Web.Services.WebMethod()>
    Public Shared Function getPicSettings(ByVal id As String) As String
        Dim shareSettings As String = ""
        Dim picNumber As String = ""
        Dim db As New sgdataDataContext
        Dim actPic As activityPicture = Nothing

        picNumber = id.Replace("FeaturedContent_share", "")
        actPic = (From ap In db.activityPictures
                  Where picNumber = ap.pictureNumber
                  Select ap).FirstOrDefault

        If actPic IsNot Nothing Then
            shareSettings = actPic.shareSetting
        End If
        Return shareSettings
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Sub savePictureSettings(ByVal sendData As String)
        Dim db As New sgdataDataContext
        Dim picNumber As Long = -1
        Dim actPicture As activityPicture = Nothing
        Dim splitSendData() As String

        splitSendData = sendData.Split(",")
        picNumber = Convert.ToDouble(splitSendData(0).Replace("FeaturedContent_share", ""))
        actPicture = (From ap In db.activityPictures
                      Where ap.pictureNumber = picNumber
                      Select ap).FirstOrDefault
        If actPicture IsNot Nothing Then
            actPicture.shareSetting = splitSendData(1)
            db.SubmitChanges()
        End If
    End Sub

    Sub backRevPicture(ByVal pi As activityPicture)
        Dim serverPath As String = ""
        Dim midSize640x480Path As String = ""
        Dim thumbNailPath As String = ""
        Dim fileName As String = ""
        Dim midSizeWidth As Integer = 620
        Dim midSizeHeight As Integer = 480

        serverPath = Server.MapPath("../pictures/actpic/")
        midSize640x480Path = Server.MapPath("../pictures/midsize/")
        thumbNailPath = Server.MapPath("../pictures/thumbnail/")
        fileName = pi.pictureLocation.Substring(pi.pictureLocation.IndexOf("picnum"),
                                                pi.pictureLocation.Length - pi.pictureLocation.IndexOf("picnum"))

        If pi.pictureLocation.Contains("../actpic/") Or
           pi.pictureLocation.Contains("~/actpic/") Then
            pi.pictureLocation = "../pictures/actpic/" & fileName
            db.SubmitChanges()

            Dim originalPic As New Drawing.Bitmap(FromFile(Server.MapPath(pi.pictureLocation)))
            Dim midSize As New Drawing.Bitmap(originalPic, 620, 480)
            Dim thumbNail As New Drawing.Bitmap(originalPic, 50, 50)
            midSize.Save(Path.Combine(midSize640x480Path & fileName))
            thumbNail.Save(Path.Combine(thumbNailPath & fileName))
        ElseIf File.Exists(midSize640x480Path & fileName) = False Then
            Dim originalPic As New Drawing.Bitmap(FromFile(Server.MapPath(pi.pictureLocation)))
            If originalPic.Width < 620 Then
                midSizeWidth = originalPic.Width
            End If
            If originalPic.Height < 480 Then
                midSizeHeight = originalPic.Height
            End If

            Dim midSize As New Drawing.Bitmap(originalPic, midSizeWidth, midSizeHeight)
            Dim thumbNail As New Drawing.Bitmap(originalPic, 50, 50)
            midSize.Save(Path.Combine(midSize640x480Path & fileName))
            thumbNail.Save(Path.Combine(thumbNailPath & fileName))
        End If
    End Sub

    Function calcTimeDiff(ByVal theAct As activity) As String
        Dim tempTime As String = ""
        Dim stopTime As String = ""
        Dim timeDiff As String = ""

        stopTime = TimeOfDay.TimeOfDay.ToString()
        tempTime = (Convert.ToDateTime(stopTime) - Convert.ToDateTime(theAct.startTime)).ToString()
        If tempTime.Contains("-") Then
            tempTime = (Convert.ToDateTime(theAct.startTime) - Convert.ToDateTime(TimeOfDay.TimeOfDay.ToString)).ToString
            timeDiff = (Convert.ToDateTime("23:59:59".ToString) - Convert.ToDateTime(tempTime)).ToString
        Else
            timeDiff = (Convert.ToDateTime(stopTime) - Convert.ToDateTime(theAct.startTime)).ToString()
        End If
        Return timeDiff
    End Function


    Function createActivityDetailItem(ByVal actNumber As String,
                                      ByVal itemClass As String,
                                      ByVal itemTitle As String,
                                      ByVal itemData As String,
                                      ByVal itemId As String,
                                      Optional ByVal custom As Boolean = False) As HtmlGenericControl
        Dim dItem As HtmlGenericControl = New HtmlGenericControl

        dItem.TagName = "Div"
        dItem.ID = itemId
        dItem.Attributes.Add("Class", itemClass)
        dItem.Attributes.Add("Title", itemTitle)
        If custom = True Then
            dItem.Controls.Add(createTextBoxItem(actNumber, itemTitle, itemData))
        Else
            dItem.InnerHtml = itemData.Replace(vbCrLf, "</br>")
        End If
        Return dItem
    End Function

    Function createActivityReviewEmail(ByVal thegoal As goal,
                                      ByVal act As activity,
                                      ByVal theSupporterEmail As String,
                                      ByVal rNum As Long) As MailMessage
        Dim MailMsg As New MailMessage
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As _
                New StreamReader(System.Web.HttpContext.Current.Server.
                                 MapPath("../EmailTemplates/actReview.html"))
        Dim outMsgBody As String = ""

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"),
                                  New MailAddress(theSupporterEmail))
        currentMember = getMemberInformation(thegoal.userName)
        If currentMember IsNot Nothing Then
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True
            MailMsg.Subject = "Sweet Goals - Request Activity Review For"
            MailMsg.Subject = MailMsg.Subject.Replace(vbNewLine, "")
            outMsgBody = goalSupportForm.ReadToEnd()
            outMsgBody = outMsgBody.Replace("[currentGoalTitle]", thegoal.goalTitle)
            outMsgBody = outMsgBody.Replace("[activityTitle]", act.actTitle)
            outMsgBody = outMsgBody.Replace("[currentUserEmailAddress]", currentMember.Email)
            outMsgBody = outMsgBody.Replace("[currentGoalUsername]", thegoal.userName)
            outMsgBody = outMsgBody.Replace("[currentGoalDesc]", thegoal.goalDesc)
            outMsgBody = outMsgBody.Replace("[activityDescription]", act.actDesc)
            outMsgBody = outMsgBody.Replace("[rNumber]", rNum)
            outMsgBody = outMsgBody.Replace("[supporterEmailAddress]", theSupporterEmail)
            MailMsg.Body = outMsgBody
        End If
        Return MailMsg
    End Function

    Function createCommentItem(ByVal actNumber As Integer,
                               ByVal commentItem As activityResponse) As HtmlGenericControl
        '<div id="actCommentsItem<act number>" Class="actComments">
        '   <div class="nameDateComment">
        '       <span class="nameComment"> </span>
        '       <span class="dateComment"> </span>
        '   </div>
        '   <div id="sComment<act number>" class="commentItem">
        '   </div>
        '</div>

        Dim actCommentsDiv As New HtmlGenericControl
        Dim nameDateCommentDiv As New HtmlGenericControl
        Dim nameCommentSpan As New HtmlGenericControl
        Dim dateCommentSpan As New HtmlGenericControl
        Dim sCommentDiv As New HtmlGenericControl
        Dim uSupport As New supporter
        Dim sMember As New Membershipdb
        Dim uName As New User

        uSupport = (From se In db.supporters
                    Where se.supportNumber = commentItem.sNumber
                    Select se).FirstOrDefault
        Try
            sMember = (From sm In db.Membershipdbs
                       Where sm.Email.Contains(uSupport.supportEmail)
                       Select sm).FirstOrDefault
        Catch ex As Exception
            sMember = Nothing
        End Try

        actCommentsDiv.TagName = "Div"
        actCommentsDiv.ID = "actCommentsItem_" & actNumber
        actCommentsDiv.Attributes.Add("Class", "actComments")

        nameDateCommentDiv.TagName = "Div"
        nameDateCommentDiv.ID = "nameDateComment_" & actNumber
        nameDateCommentDiv.Attributes.Add("Class", "nameDate")

        nameCommentSpan.TagName = "Span"
        If commentItem.userName IsNot Nothing Then
            nameCommentSpan.Attributes.Add("Class", "name")
            nameCommentSpan.InnerHtml = commentItem.userName
        ElseIf sMember IsNot Nothing Then
            uName = (From u In db.Users
                     Where u.UserId = sMember.UserId
                     Select u).FirstOrDefault
            nameCommentSpan.Attributes.Add("Class", "nameComment")
            nameCommentSpan.InnerHtml = commentItem.userName
        Else
            nameCommentSpan.Attributes.Add("Class", "nameComment")
            nameCommentSpan.InnerHtml = uSupport.supportEmail
        End If
        dateCommentSpan.TagName = "Span"
        dateCommentSpan.Attributes.Add("Class", "dateComment")
        If commentItem.rDate IsNot Nothing Then
            dateCommentSpan.InnerHtml = commentItem.rDate
        End If
        sCommentDiv.TagName = "Div"
        sCommentDiv.ID = "sComment" & commentItem.responseNumber
        sCommentDiv.Attributes.Add("Class", "commentItem")
        sCommentDiv.InnerHtml = commentItem.response.Replace(vbCrLf, "<br>")

        nameDateCommentDiv.Controls.Add(nameCommentSpan)
        nameDateCommentDiv.Controls.Add(dateCommentSpan)
        actCommentsDiv.Controls.Add(nameDateCommentDiv)
        actCommentsDiv.Controls.Add(sCommentDiv)

        Return actCommentsDiv
    End Function

    Function createMainPictureDiv(ByVal thegoal As goal,
                                  ByVal pic As activityPicture) As HtmlGenericControl
        Dim midSize640x480Path As String = ""
        Dim mainPictureDiv As New HtmlGenericControl
        Dim picture As New Image
        Dim fileName As String = ""

        fileName = pic.pictureLocation.Substring(pic.pictureLocation.IndexOf("picnum"),
                                                pic.pictureLocation.Length -
                                                pic.pictureLocation.IndexOf("picnum"))
        midSize640x480Path = Server.MapPath("../pictures/midsize/")

        mainPictureDiv.TagName = "Div"
        mainPictureDiv.ID = "mainPicture"
        mainPictureDiv.Attributes.Add("class", "mainPicture")
        mainPictureDiv.ClientIDMode = UI.ClientIDMode.Static

        picture = New Image
        picture.ID = "mainImg"
        picture.Attributes.Add("class", "addPicIcon")

        picture.Attributes.Add("src", pic.pictureLocation.Replace("actpic", "midsize") & "?" &
                               DateTime.Now.Ticks.ToString())
        picture.ClientIDMode = UI.ClientIDMode.Static
        If checkPicPermissions(pic, thegoal) = True Then
            mainPictureDiv.Controls.Add(picture)
        End If
        Return mainPictureDiv
    End Function

    Function createPictureCollectionDiv(ByVal thegoal As goal,
                                        ByVal pics As List(Of activityPicture)) As HtmlGenericControl
        Dim picSelect As New HtmlGenericControl("Div")
        Dim eachPic As New HtmlGenericControl("Div")
        Dim picSettingDiv As New HtmlGenericControl("Div")
        Dim pictureDiv As New HtmlGenericControl("div")
        Dim picture As New Image
        Dim shareSettings As New Image
        Dim i As Integer = 0

        picSelect.ID = "picSelect"
        picSelect.ClientIDMode = UI.ClientIDMode.Static
        Dim rowi As New HtmlGenericControl("Div")

        For Each pi In pics
            backRevPicture(pi)
            If checkPicPermissions(pi, thegoal) = True Then
                eachPic = New HtmlGenericControl("span")
                eachPic.ClientIDMode = UI.ClientIDMode.Static
                eachPic.Attributes.Add("class", "eachPic")

                pictureDiv = New HtmlGenericControl("Div")
                pictureDiv.Attributes.Add("class", "pictureDiv")

                picture = New Image
                picture.Attributes.Add("class", "picLine")
                picture.Attributes.Add("src", pi.pictureLocation.Replace("actpic", "thumbnail") & "?" &
                                       DateTime.Now.Ticks.ToString())
                pictureDiv.Controls.Add(picture)
                eachPic.Controls.Add(pictureDiv)

                If UserLoggedIn() Then
                    shareSettings = New Image
                    shareSettings.ID = "share" & pi.pictureNumber
                    shareSettings.ImageUrl = "../Images/gear.png"
                    shareSettings.CssClass = "sharePic"
                    shareSettings.Attributes.Add("shareSetting", pi.shareSetting)

                    picSettingDiv = New HtmlGenericControl("div")
                    picSettingDiv.Attributes.Add("class", "picSettingDiv")
                    picSettingDiv.Controls.Add(shareSettings)
                    eachPic.Controls.Add(picSettingDiv)
                End If
                rowi.Controls.Add(eachPic)
                i += 1
                If i Mod 5 = 0 Then
                    picSelect.Controls.Add(rowi)
                    rowi = New HtmlGenericControl("div")
                End If
            End If
        Next
        picSelect.Controls.Add(rowi)
        Return picSelect
    End Function

    Function createPicturePanel(ByVal thegoal As goal,
                                ByVal pics As List(Of activityPicture)) As HtmlGenericControl
        Dim pItems As New HtmlGenericControl
        Dim i As Integer = -1
        Dim validPermission As Boolean = False

        pItems.TagName = "Div"
        pItems.ID = "picItems"
        If pics.Count > 0 Then
            While ((validPermission = False) And (i < pics.Count - 1))
                i += 1
                validPermission = checkPicPermissions(pics(i), thegoal)
            End While
            pItems.Controls.Add(createMainPictureDiv(thegoal, pics(i)))
            pItems.Controls.Add(createPictureCollectionDiv(thegoal, pics))
        End If
        Return pItems
    End Function

    Function createSpan(ByVal sId As String,
                        ByVal sClass As String,
                        ByVal sTitle As String,
                        ByVal sData As String) As HtmlGenericControl
        Dim cSpan As New HtmlGenericControl

        cSpan.TagName = "Span"
        cSpan.ID = sId
        cSpan.Attributes.Add("Class", sClass)
        cSpan.Attributes.Add("Title", sTitle)
        cSpan.InnerHtml = sData
        Return cSpan
    End Function

    Function createTextBoxItem(ByVal actNumber As String,
                               ByVal itemTitle As String,
                               ByVal itemData As String) As TextBox
        Dim tbox As TextBox = New TextBox
        Dim blah As String = ""

        tbox.ID = itemTitle
        tbox.Rows = 2
        tbox.Columns = 20
        tbox.Attributes.Add("Class", "textboxes")
        tbox.Attributes.Add("Placeholder", itemTitle)
        tbox.ValidateRequestMode = UI.ValidateRequestMode.Disabled
        tbox.Text = itemData.Replace(vbCrLf, "</br>")
        Return tbox
    End Function

    Sub displayAct()
        Dim act As New activity
        Dim theGoal As New goal
        Dim actNum As Integer = -1
        Dim goalNum As Integer = -1

        getGoalActNumbers(goalNum, actNum)
        previousActivity.Attributes.Add("style", "display:none")
        nextActivity.Attributes.Add("style", "display:none")

        If goalNum <> -1 Then
            theGoal = (From g In db.goals
                       Where g.goalNumber = goalNum
                       Select g).FirstOrDefault
            If actNum > 0 Then
                act = (From a In db.activities
                       Where a.actNumber = actNum
                       Select a).FirstOrDefault
                loadMetaData(theGoal, act)
                setPrevNextLinks(theGoal, act)
                stateOptions(theGoal, act)
                displayGoalItem(theGoal, act)
            Else
                setCreateState()
                displayGoalItem(theGoal)
            End If
        End If
    End Sub

#Region "State functions"
    'Functions in Region:
    'checkChangeState
    'setChangeState
    'checkCompleteState
    'setCompleteState
    'checkStartState
    'setStartState
    'setCreateState
    'setPublicState
    Sub stateOptions(ByVal theGoal As goal,
                  ByVal act As activity)
        If checkChangeState(act.status) Then
            setChangestate()
        ElseIf checkCompleteState(act.status) Then
            setCompleteState(theGoal, act)
        ElseIf checkStartState(theGoal.status, act.status) Then
            setStartState()
        Else setCreateState()
        End If
    End Sub
    Function checkChangeState(ByVal status As String) As Boolean
        If status.Contains("Start") Or
           status.Contains("Edit") Then
            Return True
        Else Return False
        End If
    End Function

    Sub setChangestate()
        stopActButton.Style.Item("visibility") = "visible"
        startSection.Visible = False
        editActButton.Visible = False
        endSection.Visible = True
    End Sub

    Function checkCompleteState(ByVal status As String) As Boolean
        If status.Contains("Complete") Then
            Return True
        Else Return False
        End If
    End Function

    Sub setCompleteState(ByVal theGoal As goal,
                         ByVal act As activity)
        startSection.Visible = True
        editActButton.Visible = True
        endSection.Visible = False
        setPrevNextLinks(theGoal, act)
        displayActivityComments(theGoal, act)
    End Sub

    Function checkStartState(ByVal goalStatus As String,
                              ByVal actStatus As String) As Boolean
        If goalStatus.Contains("working") And
           (actStatus.Contains("Created") Or actStatus.Contains("Suggested")) And
           goalModule.checkStartActivity().Length = 0 Then
            Return True
        End If
        Return False
    End Function

    Sub setStartState()
        deleteActButton.Visible = True
        startActButton.Visible = True
    End Sub

    Sub setCreateState()
        endSection.Visible = False
        editActButton.Visible = False
        createActButton.Visible = True
        startActButton.Visible = True
    End Sub
#End Region

#Region "Metadata routines: loadMetaData, loadStandardMetaData, loadFacebookMetaData, loadTwitterMetaData, loadMetaTag"

    Sub loadMetaData(ByVal theGoal As goal,
                     ByVal act As activity)
        Dim imageLink As String = ""

        loadStandardMetaData(theGoal, act)
        imageLink = loadFacebookMetaData(act)
        loadTwitterMetaData(imageLink)
    End Sub

    Sub loadStandardMetaData(ByVal theGoal As goal,
                             ByVal act As activity)
        Dim actList As List(Of activityTextControl)
        Dim actListItem As New activityTextControl

        actList = (From ali In db.activityTextControls
                   Where ali.actnum = act.actNumber
                   Select ali).ToList

        actListItem = (From ali In actList
                       Where ali.listed.Contains("List")
                       Select ali).FirstOrDefault
        If Not actListItem Is Nothing Then
            Page.Title = theGoal.goalTitle & " - " & actListItem.title & " - " & actListItem.text
            Page.MetaDescription = actListItem.text
        Else
            Page.Title = theGoal.goalTitle
            Page.MetaDescription = theGoal.goalTitle
        End If
        If Page.Title.Length > 60 Then
            Page.Title = Page.Title.Substring(0, 60)
        End If

        If Page.MetaDescription.Length > 160 Then
            Page.MetaDescription = Page.MetaDescription.Substring(0, 160)
        End If

        For Each al In actList
            Page.MetaKeywords &= al.title & ", "
        Next
    End Sub

    Function loadFacebookMetaData(ByVal act As activity) As String
        Dim actPic As New activityPicture
        Dim head As HtmlHead = Page.Header
        Dim imageLink As String = ""
        Dim metaTag As New HtmlMeta

        head.Controls.Add(loadMetaTag("og:Title", Page.Title))
        head.Controls.Add(loadMetaTag("og:type", "Website"))
        head.Controls.Add(loadMetaTag("og:url", HttpContext.Current.Request.Url.AbsoluteUri))

        metaTag.Attributes.Add("name", "og:image")
        actPic = (From ap In db.activityPictures
                  Where ap.actNumber = act.actNumber
                  Select ap).FirstOrDefault
        If actPic IsNot Nothing Then
            imageLink = actPic.pictureLocation
            imageLink = imageLink.Replace("../", "")
            imageLink = "http://www.sweetgoals.com/" & imageLink
            metaTag.Attributes.Add("content", imageLink)
        Else
            imageLink = "http://www.sweetgoals.com/images/logo.png"
            metaTag.Attributes.Add("content", imageLink)
        End If
        head.Controls.Add(metaTag)
        Return imageLink
    End Function

    Sub loadTwitterMetaData(ByVal imageLink As String)
        Dim head As HtmlHead = Page.Header

        head.Controls.Add(loadMetaTag("twitter:card", "Summary"))
        head.Controls.Add(loadMetaTag("twitter:site", "@SweetGoals"))
        head.Controls.Add(loadMetaTag("twitter:title", Page.Title))
        head.Controls.Add(loadMetaTag("twitter:description", Page.MetaDescription))
        head.Controls.Add(loadMetaTag("twitter:url", HttpContext.Current.Request.Url.AbsoluteUri))
        head.Controls.Add(loadMetaTag("twitter:image:src", imageLink))
    End Sub

    Function loadMetaTag(ByVal metaProperty As String,
                         ByVal metaContent As String) As HtmlMeta
        Dim metaTag As New HtmlMeta

        metaTag.Attributes.Add("name", metaProperty)
        metaTag.Attributes.Add("content", metaContent)

        Return metaTag
    End Function

#End Region

    Sub displayActivityComments(ByVal thegoal As goal,
                                ByVal theAct As activity)
        Dim comments As IQueryable(Of activityResponse)
        Dim actComments As New HtmlGenericControl

        comments = (From c In db.activityResponses
                    Where c.actNumber = theAct.actNumber
                    Order By c.responseNumber Descending)
        If UserLoggedIn() = False And
            comments.Count = 0 Then
            commentGroup.Visible = False
        ElseIf UserLoggedIn() = False And
            comments.Count > 0 Then
            aComments.Visible = False
            For Each com In comments
                actComments.Controls.Add(createCommentItem(theAct.actNumber, com))
            Next
            actCommentPanel.Controls.Add(actComments)
        ElseIf UserLoggedIn() = True And
               comments.Count > 0 Then
            For Each com In comments
                actComments.Controls.Add(createCommentItem(theAct.actNumber, com))
            Next
            actCommentPanel.Controls.Add(actComments)
        End If
    End Sub

    Sub displayGoalItem(ByVal thegoal As goal,
                        Optional ByVal theAct As activity = Nothing)
        '<div id="actDetailItem<act number>" Class="activityDetail">
        '   <div id="actTitle<act number>" class="actTitle">
        '   </div>
        '   <div id="actTimes<act number>" class="actTimes">
        '       <span class="startTime"> start time </span>
        '       <span class="startTime"> End time </span>
        '       <span class="startTime"> Duration time </span>
        '       <span class="startTime"> Date </span>
        '   </div>
        '</div>

        Dim actPics As List(Of activityPicture)
        Dim actDetailItem As New HtmlGenericControl
        Dim divTimes As New HtmlGenericControl
        Dim activityTextControls As List(Of activityTextControl) = New List(Of activityTextControl)
        Dim activityListTextControl As New activityTextControl
        Dim previousAct As New activity
        Dim atcServerData As String = ""
        Dim goalActs As List(Of activity) = New List(Of activity)

        Session("goalUserName") = thegoal.userName
        If theAct Is Nothing Then
            commentGroup.Visible = False
            goalTitleDiv.InnerText = "Create Activity For " & Server.HtmlDecode(thegoal.goalTitle)
            activityStatusDiv.InnerText = "By " & thegoal.userName & " On " & Today.Date.ToShortDateString
            actStartTime.InnerText = ""
            actCompleteTime.InnerText = ""
            actDifferenceTime.InnerText = ""
            textControlServerData.Value = ""
            goalActs = (From ga In db.activities
                        Where ga.goalNumber = thegoal.goalNumber
                        Order By ga.actNumber).ToList()
            If goalActs.Count > 0 Then
                previousAct = (From pa In db.activities
                               Where pa.actNumber = goalActs(goalActs.Count - 1).actNumber
                               Select pa).FirstOrDefault
                If previousAct IsNot Nothing Then
                    activityTextControls = (From atc In db.activityTextControls
                                            Where (atc.actnum.Contains(previousAct.actNumber) And
                                                       atc.listed.Contains("Activity"))
                                            Order By atc.Id Ascending).ToList()
                    activityListTextControl = (From atc In db.activityTextControls
                                               Where (atc.actnum.Contains(previousAct.actNumber) And
                                                           atc.listed.Contains("List"))
                                               Select atc).FirstOrDefault
                    If activityListTextControl IsNot Nothing Then
                        activityTextControls.Insert(0, activityListTextControl)
                        If activityTextControls.Count > 0 Then
                            For Each actTextCon In activityTextControls
                                atcServerData &= actTextCon.title & "<>"
                                atcServerData &= actTextCon.listed & "<>"
                                atcServerData &= "<>"
                            Next
                            textControlServerData.Value = atcServerData
                        End If
                    End If
                End If
            End If
        Else
            textControlBackRev(theAct)
            If (theAct.stopTime Is Nothing) And (theAct.status.Contains("start")) Then
                theAct.timeDiff = calcTimeDiff(theAct)
                theAct.stopTime = TimeOfDay.TimeOfDay.ToString
            End If
            actDetailItem.TagName = "Div"
            actDetailItem.ID = "actDetailItem1_" & theAct.actNumber
            actDetailItem.Attributes.Add("class", "activityDetail")
            actPics = (From ap In db.activityPictures
                       Where ap.actNumber = theAct.actNumber).ToList
            If actPics.Count > 0 Then
                picDisplayPanel.Attributes.Clear()
                picDisplayPanel.Controls.Add(createPicturePanel(thegoal, actPics))
            Else picDisplayPanel.Attributes.Add("style", "display:none")
            End If
            actDetailItem.Controls.Add(createActivityDetailItem(thegoal.goalNumber, "goalTitle", "Goal Title",
                                                                thegoal.goalTitle, "goalTitle"))
            divTimes.TagName = "Div"
            divTimes.Attributes.Add("Class", "actTimes")
            If (theAct.stopTime Is Nothing) And (theAct.status.Contains("Start")) Then
                theAct.timeDiff = calcTimeDiff(theAct)
                theAct.stopTime = TimeOfDay.TimeOfDay.ToString
                divTimes.Controls.Add(createSpan("Start", "times", "Start", theAct.startTime))
                divTimes.Controls.Add(createSpan("End", "times", "End", theAct.stopTime))
                divTimes.Controls.Add(createSpan("Difference", "times", "Difference", theAct.timeDiff))
                divTimes.Controls.Add(createSpan("Date_Completed", "times", "Date Completed", theAct.actDate))
            ElseIf (theAct.stopTime IsNot Nothing) And (theAct.status.Contains("Complete")) Then
                divTimes.Controls.Add(createSpan("Start", "times", "Start", theAct.startTime))
                divTimes.Controls.Add(createSpan("End", "times", "End", theAct.stopTime))
                divTimes.Controls.Add(createSpan("Difference", "times", "Difference", theAct.timeDiff))
                divTimes.Controls.Add(createSpan("Date_Completed", "times", "Date Completed", theAct.actDate))
            Else
                divTimes.Controls.Add(createSpan("Date_Created", "timeSingle", "Date Created", theAct.actDate))
            End If
            actDetailItem.Controls.Add(divTimes)
            activityTextControls = (From atc In db.activityTextControls
                                    Where (atc.actnum.Contains(theAct.actNumber) And atc.listed.Contains("Activity"))
                                    Order By atc.Id Ascending).ToList()
            activityListTextControl = (From atc In db.activityTextControls
                                       Where (atc.actnum.Contains(theAct.actNumber) And atc.listed.Contains("List"))
                                       Select atc).FirstOrDefault
            activityTextControls.Insert(0, activityListTextControl)
            If activityTextControls.Count > 0 Then
                For Each actTextCon In activityTextControls
                    atcServerData &= actTextCon.title & "<>"
                    atcServerData &= actTextCon.listed & "<>"
                    If theAct.status.Contains("Complete") Then
                        atcServerData &= formatTextControl(actTextCon.text) & "<>"
                    Else
                        atcServerData &= actTextCon.text & "<>"
                    End If
                Next
                textControlServerData.Value = atcServerData
            End If
            goalTitleDiv.InnerText = Server.HtmlDecode(thegoal.goalTitle)
            Session("actTitle") = Server.HtmlDecode(theAct.actTitle)
            activityStatusDiv.InnerText = "By " & thegoal.userName & " On " & theAct.actDate
            Session("actStatus") = theAct.status
            Session("actDate") = theAct.actDate
            actStartTime.InnerText = theAct.startTime
            actCompleteTime.InnerText = theAct.stopTime
            actDifferenceTime.InnerText = theAct.timeDiff
        End If
    End Sub

    Sub setPrevNextLinks(ByVal thegoal As goal,
                         ByVal theAct As activity)
        Dim goalActs As List(Of activity)
        goalActs = (From act In db.activities
                    Where (act.goalNumber = thegoal.goalNumber) And
                         (act.status.Contains("Complete") Or
                          act.status.Contains("start") Or
                          act.status.Contains("Edit"))
                    Order By Convert.ToDateTime(act.actDate) Descending,
                                Convert.ToDateTime(act.startTime) Descending).ToList()
        If goalActs.Count > 0 Then
            setPreviousActivityLink(goalActs, theAct.actNumber)
            setNextActivityLink(goalActs, theAct.actNumber)
        End If
    End Sub

    Sub setPreviousActivityLink(ByVal goalActs As List(Of activity),
                                ByVal actNumber As Integer)
        Dim i As Integer = 0
        Dim setLink As Boolean = False
        Dim setCreateState As Boolean = False

        For i = 0 To goalActs.Count - 1 Step 1
            If (i < goalActs.Count - 1) And
               (goalActs(i).actNumber = actNumber) Then
                previousActivity.PostBackUrl =
                    Request.Url.AbsoluteUri.Split("?")(0) & "?anum=" & goalActs(i + 1).actNumber &
                    "&gNum=" & goalActs(i + 1).goalNumber
                previousActivity.Attributes.Clear()
                setLink = True
                Exit For
            End If
        Next
        If setLink = False Then
            previousActivity.Attributes.Add("style", "display:none")
        End If
    End Sub

    Sub setNextActivityLink(ByVal goalActs As List(Of activity),
                                ByVal actNumber As Integer)
        Dim i As Integer = 0
        Dim setLink As Boolean = False

        For i = 0 To goalActs.Count - 1 Step 1
            If (goalActs(i).actNumber = actNumber) And
                (i > 0) Then
                nextActivity.PostBackUrl =
                    Request.Url.AbsoluteUri.Split("?")(0) & "?anum=" & goalActs(i - 1).actNumber &
                    "&gNum=" & goalActs(i - 1).goalNumber
                nextActivity.Attributes.Clear()
                setLink = True
                Exit For
            End If
        Next
        If setLink = False Then
            nextActivity.Attributes.Add("style", "display:none")
        End If
    End Sub

    Function formatTextControl(ByVal text As String) As String
        Dim formattedText As String = ""

        formattedText = formatText.insertLinkBreaks(text)
        formattedText = formatText.insertHyperLinks(formattedText)
        Return formattedText
    End Function

    Sub getGoalActNumbers(Optional ByRef gnum As Integer = -1,
                          Optional ByRef anum As Integer = -1)
        Dim goalNumber As Integer = -1
        Dim checkUser As String = ""

        Try
            goalNumber = Convert.ToInt32(Request.QueryString("gNum"))
            anum = Convert.ToInt32(Request.QueryString("anum"))
        Catch ex As Exception
        End Try
        checkUser = goalModule.checkGoalNumber(goalNumber, userName)
        If checkUser.Length > 0 Then
            gnum = goalNumber
        Else
            Response.Redirect("~/Default.aspx")
        End If
    End Sub

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

    Sub kickUser()
        FormsAuthentication.SignOut()
        Response.Redirect("~/Account/Login.aspx")
    End Sub

    Sub parseTextControlData(ByVal actNumber As String)
        Dim dbTextControl As New activityTextControl
        Dim sTextControlItem As String = ""
        Dim sItem() As String = Nothing
        Dim singleTextControlItem() As String = Nothing
        Dim needToSubmitChanges As Boolean = False
        Dim toDeleteTextControls As List(Of activityTextControl) = New List(Of activityTextControl)

        toDeleteTextControls = (From dtc In db.activityTextControls
                                Where dtc.actnum.Contains(actNumber)).ToList
        If toDeleteTextControls.Count > 0 Then
            For Each tdtc In toDeleteTextControls
                db.activityTextControls.DeleteOnSubmit(tdtc)
                db.SubmitChanges()
            Next
        End If

        singleTextControlItem = Split(textControlDataHidden.Value, "[control]", , CompareMethod.Text)
        For Each sTextControlItem In singleTextControlItem
            If sTextControlItem.Length > 0 Then
                dbTextControl = New activityTextControl
                sItem = Split(sTextControlItem, "[break]", , CompareMethod.Text)
                dbTextControl.actnum = actNumber
                dbTextControl.listed = sItem(1)
                dbTextControl.title = sItem(0)
                dbTextControl.text = sItem(2)
                needToSubmitChanges = True
                db.activityTextControls.InsertOnSubmit(dbTextControl)
            End If
        Next
        If needToSubmitChanges = True Then
            db.SubmitChanges()
        End If
    End Sub

    Function savePictures(ByVal picFile As FileUpload,
                          ByVal actNum As Integer,
                          ByVal gNum As Long) As Boolean
        Dim thePic As New activityPicture
        Dim picUploaded As Boolean = False
        Dim midSizeWidth As Integer = 620
        Dim midSizeHeight As Integer = 480
        Dim picNum As Integer = 0
        Dim picPath As String = ""
        Dim ext As String = ""
        Dim serverPath As String = ""
        Dim midSize640x480Path As String = ""
        Dim thumbNailPath As String = ""
        Dim acceptedExtensions As List(Of String) = Nothing

        acceptedExtensions = New List(Of String)(New String() {".jpg", ".png", ".gif",
                                                               ".bmp", ".tif", ".tiff",
                                                               ".jpeg", ".img"})
        serverPath = Server.MapPath("../pictures/actpic/")
        midSize640x480Path = Server.MapPath("../pictures/midsize/")
        thumbNailPath = Server.MapPath("../pictures/thumbnail/")

        If picFile.HasFiles And picFile.PostedFiles.Count < 20 Then
            For Each uploadedFile In picFile.PostedFiles
                picPath = Path.GetFileName(uploadedFile.FileName)
                picPath = picPath.Replace(" ", "_")
                ext = Path.GetExtension(picPath).ToLower
                If acceptedExtensions.IndexOf(ext) > -1 Then
                    picNum = (From a In db.activityPictures
                              Order By a.pictureNumber
                                Descending Select a).FirstOrDefault.pictureNumber + 1
                    thePic = New activityPicture
                    thePic.picDesc = ""
                    thePic.date = Today.Date.ToShortDateString
                    thePic.pictureLocation = "../pictures/actpic/picnum" & picNum & "actnum" & actNum & ext
                    thePic.actNumber = actNum
                    thePic.goalNumber = gNum
                    thePic.shareSetting = setPicShareSetting(picShareAll.SelectedValue.ToString)
                    uploadedFile.SaveAs(Path.Combine(serverPath & "picnum" & picNum & "actnum" & actNum & ext))

                    Dim originalPic As New Drawing.Bitmap(Path.Combine(serverPath & "picnum" & picNum &
                                                                       "actnum" & actNum & ext))

                    If originalPic.Width < 620 Then
                        midSizeWidth = originalPic.Width
                    End If
                    If originalPic.Height < 480 Then
                        midSizeHeight = originalPic.Height
                    End If

                    Dim midSize As New Drawing.Bitmap(originalPic, midSizeWidth, midSizeHeight)
                    Dim thumbNail As New Drawing.Bitmap(originalPic, 50, 50)

                    midSize.Save(Path.Combine(midSize640x480Path & "picnum" & picNum & "actnum" & actNum & ext))
                    thumbNail.Save(Path.Combine(thumbNailPath & "picnum" & picNum & "actnum" & actNum & ext))

                    db.activityPictures.InsertOnSubmit(thePic)
                    db.SubmitChanges()
                    picUploaded = True
                    picDisplayPanel.Attributes.Add("style", "display:inline-block")
                End If
            Next
        Else : ErrorLabel.Visible = True
            ErrorLabel.Text = "To Many files at once. Only 20 at once."
            ErrorLabel.ForeColor = Drawing.Color.Red
        End If
        picFile = New FileUpload
        Return picUploaded
    End Function

    Sub sendSupportEmail(ByVal goalNum As Integer,
                         ByVal actNum As Integer)
        Dim act As New activity
        Dim thegoal As New goal
        Dim MailMsg As New MailMessage
        Dim rNumber As Integer = 0
        Dim dbSupporters As IQueryable(Of goalSupport)

        thegoal = (From g In db.goals
                   Where g.goalNumber = goalNum
                   Select g).FirstOrDefault
        act = (From a In db.activities
               Where a.actNumber = actNum
               Select a).FirstOrDefault
        dbSupporters = From s In db.goalSupports
                       Where s.goalNum = thegoal.goalNumber
                       Order By s.number Ascending
        For Each dSupportEmail In dbSupporters
            If mailModule.inUnsubscribes(dSupportEmail.sEmail) = False Then
                rNumber = mailModule.createEmailTrack(goalNum, actNum, dSupportEmail.sEmail, "vAct")
                MailMsg = createActivityReviewEmail(thegoal, act, dSupportEmail.sEmail, rNumber)
                mailModule.setMailServer(MailMsg)
            End If
        Next
    End Sub

    Function setPicShareSetting(ByVal shareSetting As String) As String
        If shareSetting.Contains("Share Settings") Then
            Return "Private"
        Else : Return shareSetting
        End If
    End Function

    Function startNewActivity(ByVal thegoal As goal) As Boolean
        Dim theAct As New activity
        Dim refAct As New activity
        Dim userType As String = ""

        userType = goalModule.checkGoalNumber(thegoal.goalNumber, userName)
        If userType.Length > 0 Then
            theAct.goalNumber = thegoal.goalNumber
            If userType.Contains("Supporter") Then
                theAct.status = "Suggested"
                theAct.sequence = (From g In db.activities
                                   Where g.goalNumber = thegoal.goalNumber And g.status = "Suggested"
                                   Select g).Count
                Session("achs") = achievementModule.suggestActivity(userName)
            Else
                theAct.status = "Start"
                theAct.startTime = TimeOfDay.TimeOfDay.ToString
                theAct.sequence = -1
                Session("achs") = achievementModule.startActivity(userName)
                Session("achs") &= ", " & achievementModule.createActivity(userName)
            End If
            theAct.actDate = Today.Date.ToShortDateString
            theAct.actTitle = userName()
            db.activities.InsertOnSubmit(theAct)
            db.SubmitChanges()

            refAct = (From ra In db.activities
                      Where ra.actTitle.Contains(userName())
                      Select ra).FirstOrDefault
            If refAct IsNot Nothing Then
                refAct.actTitle = "custom"
                parseTextControlData(refAct.actNumber)
            End If
            Return True
        End If
        Return False
    End Function

    Sub stopActivity(ByVal theGoal As goal,
                     Optional ByVal act As activity = Nothing)
        Dim refAct As activity = New activity
        Dim startTimeSpan As TimeSpan = New TimeSpan
        Dim endTimeSpan As TimeSpan = New TimeSpan
        Dim restOfDay As TimeSpan = New TimeSpan
        Dim partOfNewDay As TimeSpan = New TimeSpan
        Dim manualTime As TimeSpan = New TimeSpan

        If act Is Nothing Then
            act = New activity
            act = (From a In db.activities
                   Where a.goalNumber = theGoal.goalNumber And
                         a.status.Contains("Start")
                   Select a).FirstOrDefault
        End If
        startTimeSpan = Convert.ToDateTime(act.startTime).TimeOfDay
        If manualBox.Text.Length > 0 Then
            manualTime = Convert.ToDateTime(manualBox.Text).TimeOfDay
            If (startTimeSpan + manualTime).Days > 0 Then
                partOfNewDay = (startTimeSpan + manualTime).Subtract(New TimeSpan(1, 0, 0, 0))
                act.timeDiff = manualTime.ToString("hh\:mm\:ss")
                act.stopTime = partOfNewDay.ToString("hh\:mm\:ss")
            Else
                act.stopTime = (startTimeSpan + manualTime).ToString("hh\:mm\:ss")
                act.timeDiff = manualTime.ToString("hh\:mm\:ss")
            End If
        ElseIf act.stopTime Is Nothing Then
            endTimeSpan = Now.TimeOfDay
            If startTimeSpan > endTimeSpan Then
                partOfNewDay = endTimeSpan
                restOfDay = New TimeSpan(1, 0, 0, 0).Subtract(startTimeSpan)
                act.timeDiff = (partOfNewDay + restOfDay).ToString("hh\:mm\:ss")
                act.stopTime = endTimeSpan.ToString("hh\:mm\:ss")
            Else
                act.timeDiff = (endTimeSpan - startTimeSpan).ToString("hh\:mm\:ss")
                act.stopTime = Now.TimeOfDay.ToString("hh\:mm\:ss")
            End If
        End If

        act.status = "Complete"
        act.actTitle = userName()
        'Session("achs") = achievementModule.completeActivity(userName)
        db.SubmitChanges()

        refAct = (From ra In db.activities
                  Where ra.actTitle.Contains(userName())
                  Select ra).FirstOrDefault
        If refAct IsNot Nothing Then
            refAct.actTitle = "custom"
            parseTextControlData(refAct.actNumber)
        End If
        activityEditEnableHidden.Value = False
        'sendSupportEmail(theGoal.goalNumber, act.actNumber)
    End Sub

    Sub textControlBackRev(ByVal theAct As activity)
        Dim checkTextControlTable As activityTextControl

        checkTextControlTable = (From ctct In db.activityTextControls
                                 Where ctct.actnum.Contains(theAct.actNumber)
                                 Select ctct).FirstOrDefault
        If checkTextControlTable Is Nothing Then
            If theAct.customFieldNumber <> 0 Then
                textControlBackRevCustom(theAct)
            Else
                textControlBackRevCheck("Title", theAct.actTitle, theAct.actNumber, "List")
                textControlBackRevCheck("Description", theAct.actDesc, theAct.actNumber, "Activity")
                textControlBackRevCheck("Good", theAct.good, theAct.actNumber, "Activity")
                textControlBackRevCheck("Bad", theAct.bad, theAct.actNumber, "Activity")
            End If
        End If
    End Sub

    Sub textControlBackRevCheck(ByVal actTitle As String,
                                ByVal actText As String,
                                ByVal actNumber As String,
                                ByVal listSetting As String)
        If actText IsNot Nothing Then
            If actText.Length > 0 Then
                textControlCreateItem(actTitle, actText, actNumber, listSetting)
            End If
        End If
    End Sub

    Sub textControlBackRevCustom(ByVal theAct As activity)
        Dim customFieldData As List(Of activityFieldData)
        Dim checkTextControlTable As activityTextControl
        Dim i As Integer = 0

        customFieldData = (From cf In db.activityFieldDatas
                           Where cf.actNum = theAct.actNumber
                           Order By cf.actFieldNum Ascending).ToList
        checkTextControlTable = (From ctct In db.activityTextControls
                                 Where ctct.actnum.Contains(customFieldData(0).actNum)
                                 Select ctct).FirstOrDefault
        If checkTextControlTable Is Nothing Then
            textControlBackRevCheck(customFieldData(0).name, customFieldData(0).data, customFieldData(0).actNum, "List")
            For i = 1 To customFieldData.Count - 1 Step 1
                textControlBackRevCheck(customFieldData(i).name, customFieldData(i).data, customFieldData(i).actNum,
                                        "Activity")
            Next
        End If
    End Sub

    Sub textControlCreateItem(ByVal actTitle As String,
                              ByVal actText As String,
                              ByVal actNumber As String,
                              ByVal listSetting As String)
        Dim textItem As New activityTextControl

        textItem.actnum = actNumber
        textItem.text = actText
        textItem.title = actTitle
        textItem.listed = listSetting
        db.activityTextControls.InsertOnSubmit(textItem)
        db.SubmitChanges()
    End Sub
End Class