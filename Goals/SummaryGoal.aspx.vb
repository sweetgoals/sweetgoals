Imports System.Net

Partial Class summary
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim theGoal As New goal
        Dim goalNumberShare As String = ""
        Dim picCount As Integer = 0

        If Request.QueryString("gNum") IsNot Nothing Then
            theGoal = (From tg In db.goals
                            Where tg.goalNumber = Request.QueryString("gNum")
                            Select tg).FirstOrDefault
            If theGoal IsNot Nothing Then
                goalNumberShare = checkGoalNumber(theGoal.goalNumber, userName)
            End If
        End If

        If goalNumberShare.Length > 0 Then
            picCount = displayPictures(theGoal)
            displayStatisticsTable(theGoal, picCount)
        Else
            Response.Redirect("../Default.aspx")
        End If
    End Sub

    Function displayPictures(ByVal theGoal As goal) As Integer
        Dim picture As Image = New Image
        Dim hAnchor As HtmlAnchor = New HtmlAnchor
        Dim pSpan As HtmlGenericControl
        Dim goalNumber As Integer = -1
        Dim pics As IQueryable(Of activityPicture)
        Dim picVisible As Boolean = False
        Dim firstPic As Boolean = False
        Dim picCount As Integer = 0
        pics = (From p In db.activityPictures
                    Where p.goalNumber = theGoal.goalNumber
                    Select p)
        If pics IsNot Nothing Then
            For Each pi In pics
                If checkPicPermissions(pi, theGoal) = True Then
                    picCount += 1
                    picture = New Image
                    hAnchor = New HtmlAnchor
                    pSpan = New HtmlGenericControl

                    pSpan.TagName = "span"
                    pSpan.ID = "pic" & pi.pictureNumber
                    picture.ID = "image" & pi.pictureLocation.Substring(pi.pictureLocation.LastIndexOf("/") + 1,
                                 pi.pictureLocation.LastIndexOf(".") - pi.pictureLocation.LastIndexOf("/") - 1)
                    picture.ImageUrl = pi.pictureLocation

                    picture.Width = 50
                    picture.Height = 50

                    picture.Style.Add("display", "Inline-block")
                    hAnchor.HRef = pi.pictureLocation
                    hAnchor.Attributes.Add("rel", "prettyPhoto[gallery1]")
                    If firstPic = False Then
                        hAnchor.Controls.Add(picture)
                        firstPic = True
                    End If
                    pSpan.Controls.Add(hAnchor)
                    goalPics.Controls.Add(pSpan)
                End If
            Next
        End If
        Return picCount
    End Function

    Sub displayStatisticsTable(ByVal theGoal As goal, ByVal picCount As Integer)
        Dim goalActs As IQueryable(Of activity)
        Dim checkStartAct As Integer = 0
        Dim todayActList As IQueryable(Of activity)
        Dim completedActList As IQueryable(Of activity)
        Dim firstAct As New activity
        Dim lastAct As New activity
        Dim longestAct As New activity

        pageTitle.Text = "<h1><u>" & theGoal.goalTitle & " Summary </u></h1>"
        helpCreateRow("Description", theGoal.goalDesc)
        checkStartAct = (From a In db.activities
                             Where a.goalNumber = theGoal.goalNumber And a.status.Contains("Start")
                             Select a).Count
        helpCreateRow("Time Allocated", theGoal.timeLength & " " & theGoal.timeUnit)
        If theGoal.scheduleDays <> "" Then
            helpCreateRow("Schedule", theGoal.scheduleDays)
        Else
            helpCreateRow("Schedule", theGoal.frequency & "x " & theGoal.frequencyUnit)
        End If
        helpCreateRow("Goal Due Date", theGoal.goalDueDate)
        goalActs = From ga In db.activities
                        Where ga.goalNumber = theGoal.goalNumber
                        Order By Convert.ToDateTime(ga.actDate) Descending
        If goalActs IsNot Nothing Then
            If goalActs.Count > 0 Then
                helpCreateRow("Activities Completed", getActivityCount(theGoal.goalNumber, "Complete", goalActs))
                helpCreateRow("Activities Created", (getActivityCount(theGoal.goalNumber, "Complete", goalActs) +
                                                     getActivityCount(theGoal.goalNumber, "Created", goalActs)))
                helpCreateRow("Activities Suggested", getActivityCount(theGoal.goalNumber, "Suggested", goalActs))
                helpCreateRow("Picture Count", picCount)
                completedActList = From acts In goalActs
                                          Where (acts.status.Contains("Complete") Or acts.status.Contains("Start"))
                                          Order By Convert.ToDateTime(acts.actDate) Descending
                If completedActList.Count > 0 Then
                    todayActList = (From m In completedActList
                                        Where m.goalNumber = theGoal.goalNumber And
                                              m.actDate = Today.Date.ToShortDateString)
                    firstAct = completedActList.AsEnumerable.Last
                    lastAct = completedActList.First()
                    helpCreateRow("First Recorded Activity Date", firstAct.actDate)
                    helpCreateRow("Last Recorded Activity Date", lastAct.actDate)
                    If todayActList.Count > 0 Then
                        helpCreateRow("Today Time", listTime(todayActList))
                    End If
                    longestAct = longestTimeDifferenceActivityTitle(completedActList)
                    helpCreateRow("Longest Activity", longestAct.actTitle)
                    helpCreateRow("Longest Activity Time", longestTimeDifferenceActivityTime(longestAct.timeDiff))
                    helpCreateRow("Total Time", listTime(completedActList))
                    helpCreateRow("Average Time Per Activity", goalAverageTime(theGoal.goalNumber, completedActList))
                    helpCreateRow("Total Time This Week", gaolTimeThisWeek(theGoal.goalNumber, completedActList))
                    helpCreateRow("Average Time Per Week", averageGoalTimePerWeek(firstAct.actDate, lastAct.actDate,
                                                                                  theGoal.goalNumber, completedActList))
                End If
            End If
        End If
    End Sub

    Function gaolTimeThisWeek(ByVal goalNumber As Integer,
                              ByVal actList As IQueryable(Of activity)) As String
        Dim goalWeekTime As New TimeSpan
        Dim firstSundayOfWeek As New DateTime
        Dim weekActivities As IQueryable(Of activity) = Nothing

        firstSundayOfWeek = Today.AddDays(-Today.DayOfWeek)
        weekActivities = (From wa In actList
                                Where Convert.ToDateTime(wa.actDate) >= firstSundayOfWeek
                                Select wa)
        goalWeekTime = calcGoalTotalTime(goalNumber, weekActivities)
        Return formatTimeString(goalWeekTime)
    End Function

    Function averageGoalTimePerWeek(ByVal firstDate As String,
                                    ByVal lastDate As String,
                                    ByVal goalNumber As Integer,
                                    ByVal actList As IQueryable(Of activity)) As String
        Dim numDays As Integer = 0
        Dim numWeeks As Integer = 0
        Dim goalTotalTime As New TimeSpan
        Dim secPerWeek As New Long
        Dim secPerWeekTS As New TimeSpan

        numDays = DateDiff(DateInterval.Day, Convert.ToDateTime(firstDate), Convert.ToDateTime(lastDate))
        numWeeks = numDays / 7
        If numWeeks = 0 Then 'working less than a week
            numWeeks = 1
        End If
        goalTotalTime = calcGoalTotalTime(goalNumber, actList)
        secPerWeek = goalTotalTime.TotalSeconds / numWeeks
        secPerWeekTS = TimeSpan.FromSeconds(secPerWeek)
        Return formatTimeString(secPerWeekTS)
    End Function

    Function longestTimeDifferenceActivityTitle(ByVal cActs As IQueryable(Of activity)) As activity
        Dim longestActTime As String = ""
        Dim theact As activity

        longestActTime = (From a In cActs
                                Select a.timeDiff).Max
        theact = (From a In cActs
                    Where a.timeDiff.Contains(longestActTime)
                    Select a).FirstOrDefault
        Return theact
    End Function

    Function longestTimeDifferenceActivityTime(ByVal maxTime As String) As String
        Dim longTime As New TimeSpan

        longTime = TimeSpan.Parse(maxTime)
        Return formatTimeString(longTime)
    End Function

    Function longestActivityTime(ByVal gn As Integer) As String
        Dim longestActTime As String = ""

        longestActTime = (From a In db.activities
                                Where a.goalNumber = gn
                                Select (Convert.ToDateTime(a.timeDiff).TimeOfDay)).Max.ToString
        Return longestActTime
    End Function


    Function getActivityCount(ByVal goalNumber As Integer,
                              ByVal type As String,
                              ByVal goalActs As IQueryable(Of activity)) As Integer
        Return (From act In goalActs
                    Where act.goalNumber = goalNumber And act.status.Contains(type)
                    Select act).Count
    End Function

    Sub helpCreateRow(ByVal rowTitle As String, _
                  ByVal cellInfo As String)
        Dim arow As New TableRow
        Dim acell As New TableCell

        acell.Text = rowTitle
        arow.Cells.Add(acell)
        acell = New TableCell
        acell.Text = cellInfo
        arow.Cells.Add(acell)
        statisticsTable.Rows.Add(arow)
    End Sub

    Function listTime(ByVal actList As IQueryable(Of activity)) As String
        Dim timeDiff As TimeSpan

        For Each goalActs In actList
            If goalActs.timeDiff IsNot Nothing Then
                timeDiff += TimeSpan.Parse(goalActs.timeDiff)
            Else : timeDiff += TimeSpan.Parse(calcTimeDiff(goalActs))
            End If
        Next
        Return formatTimeString(timeDiff)
    End Function

    Function calcGoalTotalTime(ByVal goalNumber As Integer, _
                           ByVal actList As IQueryable(Of activity)) As TimeSpan
        Dim timeDiff As TimeSpan

        For Each goalActs In actList
            If (goalActs.status.Contains("Complete") Or _
                    goalActs.status.Contains("Start")) Then
                If goalActs.timeDiff IsNot Nothing Then
                    timeDiff += TimeSpan.Parse(goalActs.timeDiff)
                Else : timeDiff += TimeSpan.Parse(calcTimeDiff(goalActs))
                End If
            End If
        Next
        Return timeDiff
    End Function

    Function goalAverageTime(ByVal goalNumber As Integer, _
                             ByVal actList As IQueryable(Of activity)) As String
        Dim totalTime As New TimeSpan
        Dim avgSeconds As Double = 0
        Dim avgSecondsSpan As New TimeSpan

        totalTime = calcGoalTotalTime(goalNumber, actList)
        avgSeconds = totalTime.TotalSeconds / actList.Count
        avgSecondsSpan = TimeSpan.FromSeconds(avgSeconds)
        Return formatTimeString(avgSecondsSpan)
    End Function

    Function formatTimeString(ByVal totalTime As TimeSpan) As String
        If totalTime.Days > 0 Then
            Return totalTime.Days & " Days, " & totalTime.Hours & " Hours, " & totalTime.Minutes & " Minutes, " &
                   totalTime.Seconds & " Seconds "
        ElseIf totalTime.Hours > 0 Then
            Return totalTime.Hours & " Hours, " & totalTime.Minutes & " Minutes, " & totalTime.Seconds & " Seconds "
        Else
            Return totalTime.Minutes & " Minutes, " & totalTime.Seconds & " Seconds "
        End If
    End Function

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
End Class