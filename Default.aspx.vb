
Partial Class GoalGrid
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If UserLoggedIn Then
            displayUserGoals()
        Else
            displayCommunityGoals()
        End If
    End Sub

    Function createGoalLayout(ByVal theGoal As goal,
                              ByVal i As Integer) As HtmlAnchor

        '<href id=goalBox<i> class=goalBox>
        '   <div id=goalDivHoldSpans class=goalDivHoldSpans>
        '       <span id=goalName<i> class=goalName>Goal Name</span>
        '       <span id=lastDate<i> class=lastDate><date last worked on> </span>
        '       <span id=CompleteActs<i> class=completeActs<i><number of completed activities></span>
        '   </div>
        '</href>
        Dim goalLayout As New HtmlAnchor
        Dim goalDivHoldSpans As New HtmlGenericControl
        Dim completeActCount As Integer = 0
        Dim lastDate As String = ""
        Dim goalActs As List(Of activity)
        Dim mostRecentActivity As New activity
        Dim actTitles As New List(Of String)

        goalActs = From ca In db.activities
                   Where ca.goalNumber = theGoal.goalNumber And
                              ca.status.Contains("Complete")
                   Order By Convert.ToDateTime(ca.actDate) Descending.ToList
        completeActCount = goalActs.Count
        goalLayout.HRef = "~/Activity/ListActivity.aspx?gNum=" & theGoal.goalNumber &
                          "&activityTypeList=completed"
        goalLayout.ID = "goalBox" & i
        goalLayout.Attributes.Add("class", "goalBox")

        goalDivHoldSpans.TagName = "Div"
        goalDivHoldSpans.ID = "goalDivHoldSpans" & i
        goalDivHoldSpans.Attributes.Add("class", "goalDivHoldSpans")

        If completeActCount > 0 Then
            mostRecentActivity = goalActs.FirstOrDefault
            lastDate = mostRecentActivity.actDate
            goalDivHoldSpans.Controls.Add(createDivSpan("lastDate", lastDate, i, "Span"))
            goalDivHoldSpans.Controls.Add(createDivSpan("completeActs", completeActCount, i, "Span"))
        Else
            goalDivHoldSpans.Controls.Add(createDivSpan("lastDate", "", i, "Span"))
            goalDivHoldSpans.Controls.Add(createDivSpan("completeActs", "", i, "Span"))

        End If
        goalDivHoldSpans.Controls.Add(createDivSpan("goalName", theGoal.goalTitle, i, "Div"))
        goalLayout.Controls.Add(goalDivHoldSpans)
        Return goalLayout
    End Function

    Function createDivSpan(ByVal idName As String,
                           ByVal propertyName As String,
                           ByVal i As Integer,
                           ByVal tagName As String) As HtmlGenericControl
        Dim spanLayout As New HtmlGenericControl
        Dim lineBreak As New HtmlGenericControl

        'spanLayout.TagName = "div"
        spanLayout.TagName = tagName
        spanLayout.ID = idName & i
        spanLayout.Attributes.Add("class", idName)
        spanLayout.InnerHtml = propertyName
        Return spanLayout
    End Function

    Sub displayCommunityGoals()
        Dim i As Integer = 0
        Dim publicGoals As IQueryable(Of goal) = Nothing
        Dim sortedPublicGoalsList As List(Of goal) = Nothing

        If UserLoggedIn() = False Then
            GoalTypes.Text = "Community Goals"
            publicGoals = findCommunityGoals()
            sortedPublicGoalsList = sortGoalsByDate(publicGoals)
            For Each pg In sortedPublicGoalsList
                i += 1
                main.Controls.Add(createGoalLayout(pg, i))
            Next
        End If
    End Sub

    Sub displayUserGoals()
        Dim i As Integer = 0
        Dim workingListCount As Integer = 0
        Dim finishedListCount As Integer = 0
        Dim supportingListCount As Integer = 0
        Dim userGoals As IQueryable(Of goal) = Nothing
        Dim displayedGoals As IQueryable(Of goal) = Nothing
        Dim displayGoalsByDate As List(Of goal) = Nothing

        Session.Remove("newuser")
        userGoals = From ug In db.goals
                       Where ug.userName = userName()
        If userGoals.Count = 0 Then
            Session("newuser") = "yes"
            Response.Redirect("~/Goals/CreateGoal.aspx")
        End If
        If Request.QueryString("goalTypeList") IsNot Nothing Then
            If Request.QueryString("goalTypeList").Contains("working") Then
                GoalTypes.Text = "Working Goals"
                displayedGoals = findGoalsInCollection("working", userGoals)
            ElseIf Request.QueryString("goalTypeList").Contains("completed") Then
                GoalTypes.Text = "Finished Goals"
                displayedGoals = findGoalsInCollection("finished", userGoals)
            ElseIf Request.QueryString("goalTypeList").Contains("supporting") Then
                GoalTypes.Text = "Supporting Goals"
                displayedGoals = findUserSupportingGoals()
            ElseIf Request.QueryString("goalTypeList").Contains("public") Then
                GoalTypes.Text = "Public Goals"
                displayedGoals = findUserPublicGoals()
            ElseIf Request.QueryString("goalTypeList").Contains("community") Then
                GoalTypes.Text = "Community Goals"
                displayedGoals = findCommunityGoals()
            End If
        Else
            GoalTypes.Text = "Working Goals"
            displayedGoals = findGoalsInCollection("working", userGoals)
            If displayedGoals.Count = 0 Then
                displayedGoals = findGoalsInCollection("finished", userGoals)
            End If
        End If

        If displayedGoals IsNot Nothing Then
            displayGoalsByDate = sortGoalsByDate(displayedGoals)
            For Each dg In displayGoalsByDate
                i += 1
                main.Controls.Add(createGoalLayout(dg, i))
            Next
        End If
    End Sub

    Function sortGoalsByDate(ByRef displayGoal As IQueryable(Of goal)) As List(Of goal)
        Dim i As Integer = 0
        Dim aGoal As goal = New goal
        Dim goalEmptyActList As List(Of goal) = New List(Of goal)
        Dim sortedGoalList As List(Of goal) = New List(Of goal)
        Dim goalActList As List(Of activity) = Nothing
        Dim sumActList As List(Of activity) = New List(Of activity)

        For Each og In displayGoal
            goalActList = (From al In db.activities
                                Where al.goalNumber = og.goalNumber And al.status.Contains("Complete")
                                Order By Convert.ToDateTime(al.actDate) Descending).ToList()
            If goalActList.Count > 0 Then
                sumActList.Add(goalActList(0))
            Else
                goalEmptyActList.Add(og)
            End If
        Next
        sumActList = sumActList.OrderBy(Function(x) Convert.ToDateTime(x.actDate)).ToList()
        For i = sumActList.Count - 1 To 0 Step -1
            aGoal = (From sgl In db.goals
                        Where sgl.goalNumber = sumActList(i).goalNumber
                        Select sgl).FirstOrDefault
            sortedGoalList.Add(aGoal)
        Next
        sortedGoalList.AddRange(goalEmptyActList)
        Return sortedGoalList
    End Function
End Class
