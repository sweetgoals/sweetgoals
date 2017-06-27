Imports System.Net
Imports System.Net.Mail
Imports System.IO

Partial Class goaldetail
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            display()
        End If
    End Sub

    Sub activityDisplay(ByVal theGoal As goal,
                    ByRef activeActsList As IQueryable(Of activity))
        Dim createActsList As IQueryable(Of activity)
        Dim suggestedActsList As IQueryable(Of activity)

        If theGoal IsNot Nothing Then
            'Get the list of the different Activities        
            createActsList = From t In db.activities
                                Where t.goalNumber = theGoal.goalNumber And t.sequence >= 0 And t.status = "Created"
                                Order By t.sequence Ascending
            activeActsList = From act In db.activities
                             Where (act.goalNumber = theGoal.goalNumber) And (act.status.Contains("Complete") Or
                                                                               act.status.Contains("start") Or
                                                                               act.status.Contains("Edit"))
                             Order By Convert.ToDateTime(act.actDate) Descending,
                                Convert.ToDateTime(act.startTime) Descending
            suggestedActsList = From act In db.activities
                                    Where act.goalNumber = theGoal.goalNumber And
                                          act.status.Contains("Suggested")
                                    Order By Convert.ToDateTime(act.actDate) Descending,
                                    Convert.ToDateTime(act.stopTime) Descending

            If Request.QueryString("activityTypeList") IsNot Nothing Then
                If Request.QueryString("activityTypeList").Contains("created") And createActsList.Count > 0 Then
                    If UserLoggedIn() Then
                        displayCreateList(theGoal, createActsList)
                    Else
                        displayAcitivityList(theGoal.goalTitle, "created", createActsList)
                    End If
                ElseIf Request.QueryString("activityTypeList").Contains("completed") And activeActsList.Count > 0 Then
                    displayAcitivityList(theGoal.goalTitle, "completed", activeActsList)
                ElseIf Request.QueryString("activityTypeList").Contains("suggested") And suggestedActsList.Count > 0 Then
                    displayAcitivityList(theGoal.goalTitle, "suggested", suggestedActsList)
                Else
                    displayDefaultActivities(theGoal, createActsList, activeActsList, suggestedActsList)
                End If
            Else 'default show completed activities
                displayDefaultActivities(theGoal, createActsList, activeActsList, suggestedActsList)
            End If
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

    Function createItemDescriptioinSection(ByVal description As String) As HtmlGenericControl
        Dim itemDescSection As New HtmlGenericControl
        Dim itemDescWithLinks As String = ""

        itemDescSection.TagName = "Div"
        itemDescSection.Attributes.Add("class", "itemDescSection")

        itemDescSection.InnerHtml = formatText.insertLinkBreaks(description)
        Return itemDescSection
    End Function


    Function createItemDiv(ByVal act As activity,
                           ByVal listIndex As Integer) As HtmlAnchor
        Dim listActivityTextControl As activityTextControl
        Dim customFieldData As List(Of activityFieldData)
        Dim itemAnchor As New HtmlAnchor
        Dim itemDiv As New HtmlGenericControl
        Dim actTimeDiff As String = ""
        Dim maxFieldsToDisplay As Integer = 1
        Dim loadedTitle As Boolean = False

        itemAnchor.Attributes.Add("Class", "itemTitleActTitle")
        itemAnchor.ClientIDMode = UI.ClientIDMode.Static
        listActivityTextControl = (From atc In db.activityTextControls
                               Where atc.actnum = act.actNumber And atc.listed.Contains("List")
                               Select atc).FirstOrDefault

        itemAnchor.ID = "a_" & act.actNumber
        itemDiv.TagName = "div"

        If act.status.Contains("Start") Or act.status.Contains("Edit") Then
            itemDiv.Attributes.Add("class", "itemStart")
            Session("gcl") = "Complete"
        Else
            itemDiv.Attributes.Add("class", "item")
        End If
        If act.timeDiff = "" And act.startTime <> "" Then
            actTimeDiff = calcTimeDiff(act)
        Else
            actTimeDiff = act.timeDiff
        End If
        If act.customFieldNumber IsNot Nothing Then
            customFieldData = (From cf In db.activityFieldDatas
                                    Where cf.actNum = act.actNumber
                                    Order By cf.actNum Descending).ToList
            If customFieldData.Count < 2 Then
                maxFieldsToDisplay = 0
            End If
            For i = 0 To maxFieldsToDisplay Step 1 ' Each cfd In customFieldData
                If loadedTitle = False Then
                    itemDiv.Controls.Add(createItemTitleSectionDiv(listIndex, act, actTimeDiff, customFieldData(i).data))
                    loadedTitle = True
                ElseIf customFieldData(i).data IsNot Nothing Then
                    itemAnchor.HRef = "../Activity/DetailActivity.aspx?anum=" & act.actNumber & "&gNum=" & act.goalNumber
                    itemDiv.Controls.Add(createItemDescriptioinSection(customFieldData(i).data))
                End If
            Next
        ElseIf listActivityTextControl Is Nothing Then
            itemAnchor.HRef = "../Activity/DetailActivity.aspx?anum=" & act.actNumber & "&gNum=" & act.goalNumber
            itemDiv.Controls.Add(createItemTitleSectionDiv(listIndex, act, actTimeDiff))
            If act.actDesc IsNot Nothing Then
                itemDiv.Controls.Add(createItemDescriptioinSection(act.actDesc))
            End If
        Else
            itemAnchor.HRef = "../Activity/DetailActivity.aspx?anum=" & act.actNumber & "&gNum=" & act.goalNumber
            itemDiv.Controls.Add(createItemTitleSectionDiv(listIndex, act, actTimeDiff, listActivityTextControl.title))
            itemDiv.Controls.Add(createItemDescriptioinSection(listActivityTextControl.text))
        End If
        itemAnchor.Controls.Add(itemDiv)
        Return itemAnchor
    End Function

    Function createItemTitleSectionDiv(ByVal listIndex As String,
                                       ByVal theAct As activity,
                                       ByVal timeDiff As String,
                                       Optional ByVal customTitle As String = Nothing) As HtmlGenericControl
        Dim itemTitleSection As New HtmlGenericControl
        Dim actNumSpan As New HtmlGenericControl
        Dim actTimeSpan As New HtmlGenericControl
        Dim actDateSpan As New HtmlGenericControl
        Dim actTitleSpan As New HtmlGenericControl

        itemTitleSection.TagName = "Div"
        itemTitleSection.Attributes.Add("class", "itemTitleSection")

        actNumSpan.TagName = "span"
        actNumSpan.Attributes.Add("class", "itemTitleActNum")
        actNumSpan.InnerHtml = listIndex

        actTitleSpan.TagName = "span"
        If customTitle IsNot Nothing Then
            actTitleSpan.InnerHtml = customTitle.Replace(vbCrLf, "<br>")
        Else
            actTitleSpan.InnerHtml = theAct.actTitle
        End If

        actTimeSpan.TagName = "span"
        actTimeSpan.Attributes.Add("class", "itemTitleActTime")
        actTimeSpan.InnerHtml = timeDiff

        actDateSpan.TagName = "span"
        actDateSpan.Attributes.Add("class", "itemTitleActDate")
        actDateSpan.InnerHtml = theAct.actDate

        itemTitleSection.Controls.Add(actNumSpan)
        itemTitleSection.Controls.Add(actTitleSpan)
        itemTitleSection.Controls.Add(actTimeSpan)
        itemTitleSection.Controls.Add(actDateSpan)

        Return itemTitleSection
    End Function

    Function createSpan(ByVal text As String,
                        ByVal sClass As String,
                        ByVal title As String) As HtmlGenericControl
        Dim cSpan As New HtmlGenericControl

        cSpan.TagName = "span"
        cSpan.Attributes.Add("class", sClass)
        cSpan.InnerText = text
        Return cSpan
    End Function

    Sub display()
        Dim displayActivities As Boolean = False
        Dim currentGoal As goal = Nothing
        Dim userGoals As List(Of goal) = Nothing
        Dim actCount As Integer = 0
        Dim currentGoalActCount As Integer = 0
        Dim i As Integer = 0
        Dim activeActsList As IQueryable(Of activity) = Nothing
        Dim userPermissions As String = ""

        If Request.QueryString("gNum") IsNot Nothing Then
            currentGoal = (From cg In db.goals
                                Where cg.goalNumber = Request.QueryString("gNum")
                                Select cg).FirstOrDefault
        End If
        If currentGoal IsNot Nothing Then
            If UserLoggedIn() And
                currentGoal.userName.Contains(userName) Then
                Session.Remove("newuser")
                userGoals = (From ug In db.goals
                                Where ug.userName.Contains(userName)
                                Select ug).ToList
                While actCount < 10 And i < userGoals.Count
                    actCount += (From act In db.activities
                                    Where act.goalNumber = userGoals(i).goalNumber
                                    Select act).Count()
                    i += 1
                End While
                If actCount = 10 Then
                    Session("newuser") = "yes"
                End If
                currentGoalActCount = (From ca In db.activities
                                            Where ca.goalNumber = currentGoal.goalNumber
                                            Select ca).Count()
                If currentGoalActCount = 0 Then
                    Response.Redirect("DetailActivity.aspx?gNum=" & currentGoal.goalNumber)
                End If
            End If
            If currentGoal.public.Contains("no") Then
                If UserLoggedIn() Then
                    userPermissions = checkGoalNumber(currentGoal.goalNumber, userName)
                    If Not userPermissions.Contains("Public") Then
                        displayActivities = True
                    End If
                End If
            Else
                displayActivities = True
            End If
        End If
        If displayActivities = True Then
            activityDisplay(currentGoal, activeActsList)
            loadMetaData(currentGoal)
            goalSummaryDisplay(currentGoal, activeActsList)
            ' supportDisplay(currentGoal)
        Else
            Response.Redirect("~/Default.aspx")
        End If
    End Sub

    Sub displayAcitivityList(ByVal listTitle As String,
                             ByVal listType As String,
                             ByVal actList As IQueryable(Of activity))
        Dim i As Integer = actList.Count

        For Each act In actList
            goalDetailPanel.Controls.Add(createItemDiv(act, i))
            i -= 1
        Next
    End Sub

    Sub displayCreateList(ByVal theGoal As goal,
                          ByVal actList As IQueryable(Of activity))
        Dim i As Integer = 1

        'goalTitleLabel.Text = theGoal.goalTitle & " Created Activities"
        sortableTable.Style.Add("display", "inline-block")
        sortable.Style.Add("display", "inline-block")
        If userName.Length > 0 And
           theGoal.status.Contains("working") And
           theGoal.userName.ToLower.Contains(userName) Then
            orderButton.Visible = True
            sortable.Style.Add("cursor", "move")
        Else : orderButton.Visible = False
        End If
        For Each act In actList
            sortable.Controls.Add(createItemDiv(act, i))
            i += 1
        Next
    End Sub

    Sub displayDefaultActivities(ByVal tGoal As goal,
                                 ByVal cList As IQueryable(Of activity),
                                 ByVal aList As IQueryable(Of activity),
                                 ByVal sList As IQueryable(Of activity))
        If aList.Count > 0 Then
            displayAcitivityList(tGoal.goalTitle, "completed", aList)
        ElseIf cList.Count > 0 Then
            displayCreateList(tGoal, cList)
        ElseIf sList.Count > 0 Then
            displayAcitivityList(tGoal.goalTitle, "suggested", sList)
        Else
            'goalTitleLabel.Text = "There are no Activities for: " & tGoal.goalTitle
        End If
    End Sub

    Sub goalSummaryDisplay(ByVal currentGoal As goal,
                           ByVal activeActsList As IQueryable(Of activity))
        Dim todayActList As IQueryable(Of activity)
        Dim activityListType As String = ""

        activityListType = "Completed"
        If currentGoal IsNot Nothing Then
            If Request.QueryString("activityTypeList") IsNot Nothing Then
                If Request.QueryString("activityTypeList").Contains("created") Then
                    activityListType = "Created"
                End If
            End If

            goalDetailSection.Controls.Add(createSpan(currentGoal.goalTitle & " " & activityListType & " Activities",
                                                      "goalDetailSpan goalTitleSpan",
                                                      "Goal Title"))

            goalDetailSection.Controls.Add(createSpan(currentGoal.goalDesc,
                                                      "goalDetailSpan goalDescriptionSpan",
                                                      "Description"))
            '1 hour every 1 day
            goalDetailSection.Controls.Add(createSpan(currentGoal.timeLength & " " & currentGoal.timeUnit & " Every " &
                                                      currentGoal.frequency & " " & currentGoal.frequencyUnit,
                                                      "goalDetailSpan",
                                                      "Time Allocated"))
            goalDetailSection.Controls.Add(createSpan("To Be Completed By " & currentGoal.goalDueDate,
                                                      "goalDetailSpan",
                                                      "Due Date"))

            If activeActsList IsNot Nothing Then
                todayActList = (From m In activeActsList
                                    Where m.goalNumber = currentGoal.goalNumber And
                                        m.actDate = Today.Date.ToShortDateString And
                                        (m.status.Contains("Complete") Or m.status.Contains("Start")))
                goalDetailSection.Controls.Add(createSpan("Total time so far is " & goalTotalTime(activeActsList),
                                                          "goalDetailSpan",
                                                          "Total Time"))
            End If
        End If
    End Sub

    Function goalTotalTime(ByVal actList As IQueryable(Of activity)) As String
        Dim timeDiff As TimeSpan
        Dim days As Integer = 0
        Dim hours As Integer = 0
        Dim minutes As Integer = 0

        For Each goalActs In actList
            If (goalActs.status.Contains("Complete") Or _
                    goalActs.status.Contains("Start")) Then
                If goalActs.timeDiff IsNot Nothing Then
                    timeDiff += TimeSpan.Parse(goalActs.timeDiff)
                Else : timeDiff += TimeSpan.Parse(calcTimeDiff(goalActs))
                End If
            End If
        Next
        days = timeDiff.Days
        hours = timeDiff.Hours
        minutes = timeDiff.Minutes
        If days = 0 Then
            Return hours & " Hrs, " & minutes & " Min"
        Else : Return days & " Days, " & hours & " Hrs, " & minutes & " Min"
        End If
    End Function

    Sub loadMetaData(ByVal theGoal As goal)

        loadStandardMetaData(theGoal)
        loadFacebookMetaData()
        loadTwitterMetaData()
    End Sub

    Sub loadStandardMetaData(ByVal theGoal As goal)
        Dim acts As List(Of activity)
        Dim actListItem As New activityTextControl
        Dim actListTitles As New List(Of String)

        Page.Title = theGoal.goalTitle & " - " & theGoal.goalDesc
        If Page.Title.Length > 60 Then
            Page.Title = Page.Title.Substring(0, 60)
        End If

        Page.MetaDescription = theGoal.goalTitle & " - " & theGoal.goalDesc
        If Page.MetaDescription.Length > 160 Then
            Page.MetaDescription = Page.MetaDescription.Substring(0, 160)
        End If

        acts = (From a In db.activities
                    Where a.goalNumber = theGoal.goalNumber
                    Select a).ToList

        For Each a In acts
            actListItem = (From ali In db.activityTextControls
                                Where ali.actnum = a.actNumber And ali.listed.Contains("List")
                                Select ali).FirstOrDefault
            If actListItem IsNot Nothing Then
                If Not actListTitles.Contains(actListItem.title) Then
                    actListTitles.Add(actListItem.title)
                End If
            Else : actListTitles.Add(a.actTitle)
            End If
        Next
        For Each alt In actListTitles
            Page.MetaKeywords &= alt & ", "
        Next
    End Sub

    Sub loadFacebookMetaData()
        Dim head As HtmlHead = Page.Header
        Dim metaTag As New HtmlMeta

        head.Controls.Add(loadMetaTag("og:Title", Page.Title))
        head.Controls.Add(loadMetaTag("og:type", "Website"))
        head.Controls.Add(loadMetaTag("og:url", HttpContext.Current.Request.Url.AbsoluteUri))
        head.Controls.Add(loadMetaTag("og:image", "http://www.sweetgoals.com/images/logo.png"))
    End Sub

    Sub loadTwitterMetaData()
        Dim head As HtmlHead = Page.Header

        head.Controls.Add(loadMetaTag("twitter:card", "Summary"))
        head.Controls.Add(loadMetaTag("twitter:site", "@SweetGoals"))
        head.Controls.Add(loadMetaTag("twitter:title", Page.Title))
        head.Controls.Add(loadMetaTag("twitter:description", Page.MetaDescription))
        head.Controls.Add(loadMetaTag("twitter:url", HttpContext.Current.Request.Url.AbsoluteUri))
        head.Controls.Add(loadMetaTag("twitter:image:src", "http://www.sweetgoals.com/images/logo.png"))
    End Sub

    Function loadMetaTag(ByVal metaProperty As String,
                         ByVal metaContent As String) As HtmlMeta
        Dim metaTag As New HtmlMeta

        metaTag.Attributes.Add("name", metaProperty)
        metaTag.Attributes.Add("content", metaContent)

        Return metaTag
    End Function

    'to be used later
    Sub supportDisplay(ByVal currentGoal As goal)
        Dim theSupports As IQueryable(Of goalSupport)
        Dim arow As New TableRow

        If UserLoggedIn() Then
            If currentGoal IsNot Nothing Then
                theSupports = From gs In db.goalSupports
                              Where currentGoal.goalNumber = gs.goalNum
                If theSupports.Count > 0 Then
                    'supportTable.Visible = True
                    For Each s In theSupports
                        arow = New TableRow
                        If mailModule.inUnsubscribes(s.sEmail) = False Then
                            arow.Cells.Add(createCell(s.sEmail))
                            'supportTable.Rows.Add(arow)
                        End If
                    Next
                End If
            End If
        End If
    End Sub

    'to be used later
    Function createCell(cellText As String) As TableCell
        Dim acell As New TableCell

        acell.Text = cellText
        Return acell
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Sub reOrderList(ByVal input As String)
        Dim findId As New activity
        Dim replaceId As New activity
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim listI As New List(Of Integer)
        Dim tempArr As New List(Of Integer)
        Dim db As New sgdataDataContext
        Dim tempStr As String = ""
        Dim tempStrSplit() As String

        tempStr = input.Replace("a_", "")
        tempStrSplit = tempStr.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        listI = New List(Of Integer)
        For Each bl In tempStrSplit
            listI.Add(Convert.ToInt32(bl))
        Next
        For i = 0 To listI.Count - 1 Step 1
            replaceId = New activity
            replaceId = (From f In db.activities
                         Where f.actNumber = listI(i)
                         Select f).FirstOrDefault
            'return the index of where act number is 
            tempArr.Add(findIndex(replaceId.actNumber, listI))
        Next
        For i = 0 To listI.Count - 1 Step 1
            replaceId = New activity
            replaceId = (From f In db.activities
                         Where f.actNumber = listI(i)
                         Select f).FirstOrDefault
            replaceId.sequence = tempArr(i)
            db.SubmitChanges()
        Next
    End Sub

    Public Shared Function findIndex(ByVal tofind As Integer,
                   ByVal arr As List(Of Integer)) As Integer
        Dim found As Integer = -1
        Dim i As Integer = 0

        While found = -1
            If tofind = arr(i) Then
                found = i
            End If
            i += 1
        End While
        'array starts at zero, but the database starts at 1 so have to add one.
        Return found + 1
    End Function
End Class