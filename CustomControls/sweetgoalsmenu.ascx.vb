Imports System.Net
Imports System.Net.Mail
Imports System.IO

Partial Class sweetgoalsmenu
    Inherits System.Web.UI.UserControl

    Dim db As New sgdataDataContext

    Class MenuItem
        Public title As String
        Public anchorLink As String
        Public cssClass As String

        Public Sub New()
            title = Nothing
            anchorLink = Nothing
            cssClass = Nothing
        End Sub

        Public Sub New(ByVal pTitle As String)
            title = pTitle
        End Sub

        Public Sub New(ByVal pTitle As String,
                       ByVal pAnchorLink As String)
            title = pTitle
            anchorLink = pAnchorLink
        End Sub

        Public Sub New(ByVal ptitle As String,
                       ByVal panchorLink As String,
                       ByVal pcssClass As String)
            title = ptitle
            anchorLink = panchorLink
            cssClass = pcssClass
        End Sub

    End Class

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim currentGoal As goal = Nothing

        If Request.QueryString("gNum") IsNot Nothing Then
            currentGoal = (From gn In db.goals
                                Where gn.goalNumber = Request.QueryString("gNum")
                                Select gn).FirstOrDefault
        End If
        goalMenuFunctions(currentGoal)
        createGoalMenu(currentGoal)
        createActivityMenu(currentGoal)
        'createSupportMenu(currentGoal)
        'createAchievementsMenu()
        createAccountMenu()
        'createAboutMenu()
        createDonateMenu()

    End Sub

    Sub goalMenuFunctions(ByVal currentGoal As goal)
        If userName.Length > 0 And
           UserLoggedIn() = True And
           currentGoal IsNot Nothing Then
            If currentGoal.userName.ToLower.Contains(userName) Then
                If Request.QueryString("goalChangeState") IsNot Nothing Then
                    If Request.QueryString("goalChangeState").Contains("complete") Then
                        completeGoal(currentGoal)
                    ElseIf Request.QueryString("goalChangeState").Contains("delete") Then
                        deleteGoal(currentGoal)
                    ElseIf Request.QueryString("goalChangeState").Contains("restart") Then
                        restartGoal(currentGoal)
                    ElseIf Request.QueryString("goalChangeState").Contains("private") Then
                        privateGoal(currentGoal)
                    ElseIf Request.QueryString("goalChangeState").Contains("public") Then
                        publishGoal(currentGoal)
                    End If
                End If
            End If
        End If
    End Sub

    Sub completeGoal(ByVal currentGoal As goal)
        Dim MailMsg As New MailMessage
        Dim rNumber As Long = -1
        Dim achTitle As String = ""
        'Dim dbSupporters As IQueryable(Of goalSupport)
        Dim checkStartedActivity As Integer = 0

        checkStartedActivity = (From csa In db.activities
                                        Where csa.status.Contains("Start") And csa.goalNumber = currentGoal.goalNumber
                                        Select csa).Count()
        If currentGoal.status.Contains("working") And
           checkStartedActivity = 0 Then
            currentGoal.status = "finished"
            currentGoal.completeDate = Today.Date.ToShortDateString
            db.SubmitChanges()
            'achTitle = achievementModule.completeGoal(userName)
            'If achTitle.Length > 0 Then
            '    'ClientScript.RegisterStartupScript(Me.GetType(), "PopUp", "showAchievementPopup('" & achTitle & "');", True)
            'End If
            'dbSupporters = (From s In db.goalSupports
            '                    Where s.goalNum = currentGoal.goalNumber
            '                    Order By s.number Ascending)
            'For Each dSupportEmail In dbSupporters
            '    If mailModule.inUnsubscribes(dSupportEmail.sEmail) = False Then
            '        rNumber = mailModule.createEmailTrack(currentGoal.goalNumber, -1, dSupportEmail.sEmail, "cGoal")
            '        MailMsg = createCompleteGoalEmail(currentGoal, dSupportEmail.sEmail, rNumber)
            '        setMailServer(MailMsg)
            '    End If
            'Next
            Response.Redirect("../Default.aspx?goalTypeList=completed")
        Else
            signOut()
        End If
    End Sub

    Sub signOut()
        FormsAuthentication.SignOut()
        Response.Redirect("~/Default.aspx")
    End Sub

    Sub deleteGoal(ByVal currentGoal As goal)
        Dim goalActivityCount As Integer = 0

        goalActivityCount = (From gac In db.activities
                                Where gac.goalNumber = currentGoal.goalNumber And
                                      (gac.status.Contains("Complete") Or gac.status.Contains("Start"))
                                Select gac).Count()
        If goalActivityCount = 0 Then
            currentGoal.status = "deleted"
            currentGoal.public = "no"
            db.SubmitChanges()
            Response.Redirect("../Default.aspx")
        Else : signOut()
        End If
    End Sub

    Sub restartGoal(ByVal currentGoal As goal)
        Dim MailMsg As New MailMessage
        Dim rNumber As Long = 0
        'Dim dbSupporters As IQueryable(Of goalSupport)

        If currentGoal.status.Contains("finished") Then
            currentGoal.status = "working"
            currentGoal.completeDate = ""
            db.SubmitChanges()
            'dbSupporters = From s In db.goalSupports
            '                Where s.goalNum = currentGoal.goalNumber
            '                Order By s.number Ascending
            'For Each dSupportEmail In dbSupporters
            '    If mailModule.inUnsubscribes(dSupportEmail.sEmail) = False Then
            '        rNumber = mailModule.createEmailTrack(currentGoal.goalNumber, -1, dSupportEmail.sEmail, "rGoal")
            '        MailMsg = createRestartGoalEmail(currentGoal, dSupportEmail.sEmail, rNumber)
            '        mailModule.setMailServer(MailMsg)
            '    End If
            'Next
            Response.Redirect("../Default.aspx")
        Else : signOut()
        End If
    End Sub

    Sub publishGoal(ByVal currentGoal As goal)
        Dim goalActivityCount As Integer = 0

        goalActivityCount = (From gac In db.activities
                        Where gac.goalNumber = currentGoal.goalNumber And
                              gac.status.Contains("Complete")
                        Select gac).Count()
        If goalActivityCount >= 5 Then
            currentGoal.public = "yes"
            db.SubmitChanges()
            Response.Redirect("../Default.aspx?")
        End If
    End Sub

    Sub privateGoal(ByVal currentGoal As goal)
        currentGoal.public = "no"
        db.SubmitChanges()
        Response.Redirect("../Default.aspx")        
    End Sub

    Sub createGoalMenu(ByVal currentGoal As goal)
        Dim goalMenu As New HtmlGenericControl
        Dim goalReasons As New goalreason
        Dim goalCounts As New List(Of Integer)
        Dim goalMenuItems As New List(Of MenuItem)
        Dim goalActivityCount As Integer = 0
        Dim goalSupportCount As Integer = 0
        Dim activityStartedCount As Integer = 0
        Dim mainItemLi As New HtmlGenericControl
        Dim subMenu As New HtmlGenericControl
        Dim listMenu As New HtmlGenericControl

        goalMenu.TagName = "ul"
        goalMenu.Attributes.Add("class", "mainItem")

        mainItemLi.TagName = "li"

        subMenu.TagName = "ul"
        subMenu.Attributes.Add("class", "subMenu")

        listMenu.TagName = "ul"
        listMenu.Attributes.Add("class", "listUL")

        mainItemLi.Controls.Add(createAnchor("Goals", "~/Default.aspx"))

        If UserLoggedIn() Then
            subMenu.Controls.Add(createMenuLiAnchor("subMenuItem", "Create", "../Goals/CreateGoal.aspx"))
        End If

        If currentGoal IsNot Nothing Then
            If UserLoggedIn() And
               currentGoal.userName.ToLower.Contains(userName) Then
                goalActivityCount = (From gac In db.activities
                                            Where gac.goalNumber = currentGoal.goalNumber And
                                                  gac.status.Contains("Complete")).Count()
                activityStartedCount = (From gac In db.activities
                                            Where gac.goalNumber = currentGoal.goalNumber And
                                                   gac.status.Contains("Start")).Count()
                If currentGoal.status.Contains("working") Then
                    goalSupportCount = (From sc In db.goalSupports
                                                Where sc.goalNum = currentGoal.goalNumber).Count()
                    If goalActivityCount > 0 And activityStartedCount = 0 Then
                        subMenu.Controls.Add(createMenuLiAnchor("subMenuItem",
                                                                "Complete",
                                                                "../Activity/ListActivity.aspx?gNum=" &
                                                                    currentGoal.goalNumber & "&goalChangeState=complete"))
                    ElseIf goalSupportCount = 0 Then
                        subMenu.Controls.Add(createMenuLiAnchor("subMenuItem",
                                                                "Delete",
                                                                "../Activity/ListActivity.aspx?gNum=" &
                                                                    currentGoal.goalNumber & "&goalChangeState=delete"))

                        'subMenu.Controls.Add(createLiAnchor("subMenuItem", createAnchor("Delete",
                        '                                                                "../Activity/ListActivity.aspx?gNum=" &
                        '                                                                currentGoal.goalNumber &
                        '                                                                "&goalChangeState=delete")))
                    End If
                    'If check24HourLapse(currentGoal) = True Then
                    subMenu.Controls.Add(createMenuLiAnchor("subMenuItem",
                                                                "Modify",
                                                                "../Goals/CreateGoal.aspx?gNum=" & currentGoal.goalNumber))

                    'subMenu.Controls.Add(createLiAnchor("subMenuItem", createAnchor("Modify",
                    '                                                                "../Goals/CreateGoal.aspx?gNum=" &
                    '                                                                currentGoal.goalNumber)))
                    'End If
                ElseIf currentGoal.status.Contains("finished") Then
                    subMenu.Controls.Add(createMenuLiAnchor("subMenuItem",
                                                            "Restart",
                                                            "../Activity/ListActivity.aspx?gNum=" &
                                                                currentGoal.goalNumber & "&goalChangeState=restart"))

                    'subMenu.Controls.Add(createLiAnchor("subMenuItem", createAnchor("Restart", "../Activity/ListActivity.aspx?gNum=" &
                    '                                          currentGoal.goalNumber & "&goalChangeState=restart")))
                End If
                If currentGoal.public.Contains("yes") Then
                    subMenu.Controls.Add(createMenuLiAnchor("subMenuItem",
                                                            "Private",
                                                            "../Activity/ListActivity.aspx?gNum=" &
                                                                currentGoal.goalNumber & "&goalChangeState=private"))

                    'subMenu.Controls.Add(createLiAnchor("subMenuItem", createAnchor("Private", "../Activity/ListActivity.aspx?gNum=" &
                    '                                          currentGoal.goalNumber & "&goalChangeState=private", "bold")))

                ElseIf goalActivityCount >= 5 Then
                    subMenu.Controls.Add(createMenuLiAnchor("subMenuItem",
                                                            "Publish",
                                                            "../Activity/ListActivity.aspx?gNum=" &
                                                                currentGoal.goalNumber & "&goalChangeState=public"))

                    'subMenu.Controls.Add(createLiAnchor("subMenuItem", createAnchor("Publish", "../Activity/ListActivity.aspx?gNum=" &
                    '                                          currentGoal.goalNumber & "&goalChangeState=public", "bold")))
                End If
                'subMenu.Controls.Add(createLi("Lists", "subMenuItem"))
                'mainItemLi.Controls.Add(subMenu)
            End If

            '******* DISACTIVATED CODE FOR TIME BEING  NEEDS TO BE REWORKED*****
            'goalReasons = (From gr In db.goalreasons
            '                    Where gr.goalnum = currentGoal.goalNumber
            '                    Select gr).FirstOrDefault
            'If (goalReasons IsNot Nothing) Or (UserLoggedIn() = True) Then
            '    goalMenuItems.Add(New MenuItem("Reasons", "../Goals/ReasonsGoal.aspx?gNum=" & currentGoal.goalNumber))
            'Else
            '    goalMenuItems.Add(New MenuItem("Reasons"))
            'End If
            'goalMenuItems.Add(New MenuItem("Summary", "../Goals/SummaryGoal.aspx?gNum=" & currentGoal.goalNumber))
            '*********************************************************************
        End If
        If UserLoggedIn() Then
            subMenu.Controls.Add(createLi("Lists", "subMenuItem"))
            mainItemLi.Controls.Add(subMenu)

            goalCounts = getGoalCounts()
            listMenu.Controls.Add(createLiElementBasedGoalCount(goalCounts(0),
                                                                "subMenuItem",
                                                                "~/Default.aspx?goalTypeList=working",
                                                                "Working (" & goalCounts(0) & ")"))
            listMenu.Controls.Add(createLiElementBasedGoalCount(goalCounts(1),
                                                                "subMenuItem",
                                                                "~/Default.aspx?goalTypeList=completed",
                                                                "Completed (" & goalCounts(1) & ")"))
            listMenu.Controls.Add(createLiElementBasedGoalCount(goalCounts(2),
                                                                "subMenuItem",
                                                                "~/Default.aspx?goalTypeList=public",
                                                                "Published (" & goalCounts(2) & ")"))
            listMenu.Controls.Add(createLiElementBasedGoalCount(goalCounts(3),
                                                                "subMenuItem",
                                                                "~/Default.aspx?goalTypeList=community",
                                                                "Community (" & goalCounts(3) & ")"))
            subMenu.Controls.Add(listMenu)
        End If
        goalMenu.Controls.Add(mainItemLi)
        menubar.Controls.Add(goalMenu)
    End Sub

    Function createLiElementBasedGoalCount(ByVal goalCount As Integer,
                                           ByVal liClass As String,
                                           ByVal link As String,
                                           ByVal text As String) As HtmlGenericControl
        If goalCount > 0 Then
            Return createMenuLiAnchor(liClass, text, link)
        Else
            Return createLi(text, liClass)
        End If
    End Function

    Function createMenuLiAnchor(ByVal liClass As String,
                                ByVal anchorText As String,
                                ByVal anchorLink As String,
                                Optional ByVal anchorClass As String = "") As HtmlGenericControl
        Return createLiAnchor(liClass, createAnchor(anchorText, anchorLink))
    End Function

    Sub createActivityMenu(ByVal currentGoal As goal)
        Dim ActivityMenu As New HtmlGenericControl
        Dim ActivityMenuItems As New List(Of MenuItem)
        Dim ActivityCounts As List(Of String) = New List(Of String)
        Dim mainItemLi As New HtmlGenericControl
        Dim subMenu As New HtmlGenericControl
        Dim listMenu As New HtmlGenericControl

        ActivityMenu.TagName = "ul"
        ActivityMenu.Attributes.Add("class", "mainItem")

        mainItemLi.TagName = "li"
        subMenu.TagName = "ul"
        subMenu.Attributes.Add("class", "subMenu")

        listMenu.TagName = "ul"
        listMenu.Attributes.Add("class", "listUL")

        If currentGoal IsNot Nothing Then
            mainItemLi.TagName = "li"
            mainItemLi.Controls.Add(createAnchor("Activities",
                                                 "~/Activity/ListActivity.aspx?gNum=" & currentGoal.goalNumber))
            If UserLoggedIn() And
               currentGoal.userName.ToLower.Contains(userName) Then
                subMenu.Controls.Add(createMenuLiAnchor("subMenuItem",
                                                        "Create",
                                                        "../Activity/DetailActivity.aspx?gnum=" & currentGoal.goalNumber))
            End If
            subMenu.Controls.Add(createLi("Lists", "subMenuItem"))


            ActivityCounts = getActivityNumber(currentGoal)
            listMenu.Controls.Add(createLiElementBasedGoalCount(ActivityCounts(0),
                                                                "subMenuItem",
                                                                "../Activity/ListActivity.aspx?gNum=" & currentGoal.goalNumber & "&activityTypeList=created",
                                                                "Created (" & ActivityCounts(0) & ")"))
            listMenu.Controls.Add(createLiElementBasedGoalCount(ActivityCounts(1),
                                                                "subMenuItem",
                                                                "../Activity/ListActivity.aspx?gNum=" & currentGoal.goalNumber & "&activityTypeList=completed",
                                                                "Completed (" & ActivityCounts(1) & ")"))
            subMenu.Controls.Add(listMenu)
            mainItemLi.Controls.Add(subMenu)
            ActivityMenu.Controls.Add(mainItemLi)
        Else
            mainItemLi.Controls.Add(createLi("Activities", "subMenuItem"))
            ActivityMenu.Controls.Add(mainItemLi)
        End If
        menubar.Controls.Add(ActivityMenu)
    End Sub

    Sub createAchievementsMenu()
        Dim achievementMenu As New HtmlGenericControl
        Dim anchor As New HtmlAnchor

        If UserLoggedIn() Then
            achievementMenu.TagName = "Li"
            achievementMenu.Controls.Add(createAnchor("Achievements", "../Achievements/Achievements.aspx"))
            menubar.Controls.Add(achievementMenu)
        Else
            menubar.Controls.Add(createLi("Achievements"))
        End If
    End Sub

    Sub createSupportMenu(ByVal currentGoal As goal)
        Dim supportMenu As New HtmlGenericControl
        Dim anchor As New HtmlAnchor
        Dim supportMenuItems As New List(Of MenuItem)
        Dim inactiveSupportmenu As Boolean = True

        supportMenu.ID = "accountMenu"
        supportMenu.TagName = "Li"
        If UserLoggedIn() = True And
           currentGoal IsNot Nothing Then
            If currentGoal.userName.ToLower.Contains(userName) Then
                inactiveSupportmenu = False
                anchor = createAnchor("Support", "../Support/supporters.aspx?gNum=" & currentGoal.goalNumber)
                supportMenu.Controls.Add(anchor)
                menubar.Controls.Add(supportMenu)
            End If
        End If
        If inactiveSupportmenu = True Then
            menubar.Controls.Add(createLi("Supporters"))
        End If
    End Sub

    Sub createAccountMenu()
        Dim accountMenu As New HtmlGenericControl
        Dim accountMenuItems As New List(Of MenuItem)
        Dim anchor As New HtmlAnchor
        Dim menuItems As New List(Of KeyValuePair(Of String, String))

        Dim mainItemLi As New HtmlGenericControl
        Dim subMenu As New HtmlGenericControl
        Dim listMenu As New HtmlGenericControl

        accountMenu.TagName = "ul"
        accountMenu.Attributes.Add("class", "mainItem")

        mainItemLi.TagName = "li"
        subMenu.TagName = "ul"
        subMenu.Attributes.Add("class", "subMenu")

        listMenu.TagName = "ul"
        listMenu.Attributes.Add("class", "listUL")
        mainItemLi.TagName = "li"
        If UserLoggedIn() = False Then
            mainItemLi.Controls.Add(createAnchor("Log In",
                                                 "~/Account/Login.aspx"))
            'subMenu.Controls.Add(createMenuLiAnchor("subMenuItem", "Login", "~/Account/Login.aspx"))
            subMenu.Controls.Add(createMenuLiAnchor("subMenuItem", "Register", "~/Account/Register.aspx"))
            'anchor = createAnchor("Account", "~/Account/Login.aspx")
            'accountMenu.Controls.Add(anchor)
            'accountMenuItems.Add(New MenuItem("Login", "~/Account/Login.aspx"))
            'accountMenuItems.Add(New MenuItem("Register", "~/Account/Register.aspx"))
        Else
            mainItemLi.Controls.Add(createAnchor(HttpContext.Current.User.Identity.Name,
                                                 "../Account/settings.aspx"))
            subMenu.Controls.Add(createMenuLiAnchor("subMenuItem", "Settings", "../Account/settings.aspx"))
            subMenu.Controls.Add(createMenuLiAnchor("subMenuItem", "Logout", "~/Default.aspx?Logout=Yes"))

            'anchor = createAnchor("Account", "../Account/settings.aspx")
            'accountMenu.Controls.Add(anchor)
            'accountMenuItems.Add(New MenuItem("Settings", "../Account/settings.aspx"))
            'accountMenuItems.Add(New MenuItem("Logout", "~/Default.aspx?Logout=Yes"))
        End If
        mainItemLi.Controls.Add(subMenu)
        accountMenu.Controls.Add(mainItemLi)
        'If accountMenuItems.Count > 0 Then
        '    accountMenu.Controls.Add(createMenu("Account", accountMenuItems))
        'End If
        menubar.Controls.Add(accountMenu)
    End Sub

    Sub createAboutMenu()
        Dim aboutMenu As New HtmlGenericControl
        Dim aboutMenuItems As New HtmlGenericControl
        Dim ulLi As New HtmlGenericControl
        Dim anchor As New HtmlAnchor
        Dim pageName As String = ""
        Dim pagesContainingTours As List(Of String) = Nothing

        pagesContainingTours = New List(Of String)(New String() {"default.aspx", "listactivity.aspx", "reasonsgoal.aspx",
                                                                 "createactivity.aspx", "detailactivity.aspx", "supporters.aspx",
                                                                 "settings.aspx", "creategoal.aspx"})
        Dim mainItemLi As New HtmlGenericControl
        Dim subMenu As New HtmlGenericControl
        Dim listMenu As New HtmlGenericControl

        aboutMenu.TagName = "ul"
        aboutMenu.Attributes.Add("class", "mainItem")

        mainItemLi.TagName = "li"
        subMenu.TagName = "ul"
        subMenu.Attributes.Add("class", "subMenu")

        listMenu.TagName = "ul"
        listMenu.Attributes.Add("class", "listUL")
        mainItemLi.TagName = "li"

        mainItemLi.Controls.Add(createAnchor("About",
                                             "~/Account/about.aspx"))
        subMenu.Controls.Add(createLiAnchor("subMenuItem", createAnchor("Blog", "http://www.sweetgoals.com/blog", "Bold")))
        'subMenu.Controls.Add(createMenuLiAnchor("subMenuItem", "Tutorial", "http://www.sweetgoals.com/blog", "_blank"))
        mainItemLi.Controls.Add(subMenu)
        aboutMenu.Controls.Add(mainItemLi)
        menubar.Controls.Add(aboutMenu)



        'pageName = Path.GetFileName(Request.Url.AbsolutePath).ToLower

        'aboutMenu.TagName = "li"

        'anchor = createAnchor("About", "~/Account/about.aspx")
        'aboutMenu.Controls.Add(anchor)
        'aboutMenuItems.TagName = "ul"
        'aboutMenuItems.ID = "aboutItems"
        'aboutMenuItems.Attributes.Add("class", "subMenu")

        'anchor = New HtmlAnchor
        'ulLi = New HtmlGenericControl

        'anchor = createAnchor("Blog", "http://www.sweetgoals.com/blog", "_blank")

        'ulLi.TagName = "li"
        'ulLi.Attributes.Add("class", "subMenuItem")
        'ulLi.Controls.Add(anchor)
        'aboutMenuItems.Controls.Add(ulLi)

        'If pagesContainingTours.IndexOf(pageName) > -1 Then
        '    anchor = New HtmlAnchor
        '    ulLi = New HtmlGenericControl
        '    anchor = createTutorialAnchor()
        '    ulLi.TagName = "li"
        '    ulLi.Attributes.Add("class", "subMenuItem")
        '    ulLi.Controls.Add(anchor)
        '    aboutMenuItems.Controls.Add(ulLi)
        'End If
        '        aboutMenu.Controls.Add(aboutMenuItems)
    End Sub

    Sub createDonateMenu()
        Dim donateAnchor As New HtmlAnchor
        Dim donateImg As New HtmlImage

        donateImg.Src = "https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif"
        donateImg.Alt = "Donate Now Using Paypal"
        donateImg.Border = "0"

        donateAnchor.Target = "_blank"
        donateAnchor.HRef = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=donate@sweetgoals.com&" &
                            "item_name=Sweet Goals&no_note=0&bn=PP-DonationsBF:btn_donate_LG.gif:NonHostedGuest&" &
                            "currency_code=USD"
        donateAnchor.Controls.Add(donateImg)
        menubar.Controls.Add(donateAnchor)
    End Sub

    Function createMenuListItem(ByVal title As String,
                                    ByVal link As String,
                                    ByVal ActivityNumber As Integer) As MenuItem
        Dim menuListItem As New MenuItem

        menuListItem.cssClass = "left20px"
        If ActivityNumber > 0 Then
            menuListItem.title = title & "(" & ActivityNumber & ")"
            menuListItem.anchorLink = link
        Else
            menuListItem.title = title
        End If
        Return menuListItem
    End Function

    Function getGoalCounts() As List(Of Integer)
        Dim userGoals As IQueryable(Of goal) = Nothing
        Dim userEmail As String = ""
        Dim workingListCount As Integer = 0
        Dim finishedListCount As Integer = 0
        Dim supportingListCount As Integer = 0
        Dim publicListCount As Integer = 0
        Dim communityListCount As Integer = 0
        Dim goalCounts As New List(Of Integer)

        If UserLoggedIn() Then
            userEmail = getUserEmail()
            userGoals = From ug In db.goals
                           Where ug.userName.ToLower.Contains(userName)

            workingListCount = (From wl In userGoals
                                    Where wl.status.Contains("working")
                                    Select wl).Count()
            finishedListCount = (From wl In userGoals
                                    Where wl.status.Contains("finished")
                                    Select wl).Count()
            'supportingListCount = (From wl In db.goalSupports
            '                        Where wl.sEmail.Contains(userEmail)
            '                        Select wl).Count()
            publicListCount = (From wl In userGoals
                                    Where wl.public.Contains("yes")
                                    Select wl).Count
            communityListCount = (From cg In db.goals
                                        Where cg.public.Contains("yes")
                                    Select cg).Count
            goalCounts.Add(workingListCount)
            goalCounts.Add(finishedListCount)
            'goalCounts.Add(supportingListCount)
            goalCounts.Add(publicListCount)
            goalCounts.Add(communityListCount)
            Return goalCounts
        Else
            Return Nothing
        End If
    End Function

    Function getActivityNumber(ByVal currentGoal As goal) As List(Of String)
        Dim goalActivities As IQueryable(Of activity)
        Dim activityCounts As List(Of String) = New List(Of String)
        Dim activityCount As Integer = 0
        Dim status As List(Of String) = New List(Of String)(New String() {"Created", "Complete"})

        goalActivities = From ac In db.activities
                           Where ac.goalNumber = currentGoal.goalNumber
                           Select ac
        For Each stat In status
            activityCount = (From ga In goalActivities
                             Where ga.status.Contains(stat)).Count()
            activityCounts.Add(activityCount)
        Next

        Return activityCounts
    End Function

    Function createAnchor(ByVal text As String,
                          ByVal link As String,
                          Optional target As String = "") As HtmlAnchor
        Dim anchorHtml As New HtmlAnchor

        anchorHtml.Attributes.Add("class", "bold")
        anchorHtml.InnerHtml = text
        anchorHtml.HRef = link
        If target.Length > 0 Then
            anchorHtml.Target = target
        End If
        Return anchorHtml
    End Function

    Function createTutorialAnchor() As HtmlAnchor
        Dim tutorialAnchor As New HtmlAnchor

        tutorialAnchor.Attributes.Add("class", "tutorial")
        tutorialAnchor.InnerHtml = "Tutorial"
        tutorialAnchor.ID = "tutorial"
        Return tutorialAnchor
    End Function

    Function createLi(ByVal text As String,
                      ByVal className As String) As HtmlGenericControl
        Dim liElement As New HtmlGenericControl

        liElement.TagName = "li"
        liElement.InnerHtml = text
        liElement.Attributes.Add("class", className)
        Return liElement
    End Function

    Function createLi(Optional text As String = "") As HtmlGenericControl
        Dim liElement As New HtmlGenericControl

        liElement.TagName = "li"
        liElement.InnerHtml = text
        Return liElement
    End Function

    Function createLiAnchor(ByVal liClass As String,
                            ByVal anchor As HtmlAnchor) As HtmlGenericControl
        Dim liElement As New HtmlGenericControl

        liElement.TagName = "li"
        liElement.Attributes.Add("class", liClass)
        liElement.Controls.Add(anchor)
        Return liElement
    End Function


    Function createMenu(ByVal id As String,
                        ByVal menuItems As List(Of MenuItem)) As HtmlGenericControl
        Dim menuItem As New HtmlGenericControl
        Dim li As New HtmlGenericControl

        menuItem.TagName = "Ul"
        menuItem.Attributes.Add("class", "subMenu")
        menuItem.ID = id
        For Each item In menuItems
            li = New HtmlGenericControl
            li.TagName = "Li"
            If item.cssClass IsNot Nothing Then
                li.Attributes.Add("class", item.cssClass)
            Else
                li.Attributes.Add("class", "subMenuItem")
            End If
            If item.anchorLink IsNot Nothing Then
                li.Controls.Add(createAnchor(item.title, item.anchorLink))
            Else
                li.InnerHtml = item.title
            End If
            menuItem.Controls.Add(li)
        Next
        Return menuItem
    End Function
End Class
