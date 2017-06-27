
Partial Class achievements
    Inherits System.Web.UI.Page

    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        displayAchs()
    End Sub

    Sub displayAchs()
        Dim userName As String = ""
        Dim achCompleteItem As List(Of achievement) = Nothing
        Dim allAchs As List(Of achievementList) = Nothing


        Dim achListItem As achievementList = Nothing
        Dim i As Integer = 1

        userName = HttpContext.Current.User.Identity.Name
        achCompleteItem = (From a In db.achievements
                        Where a.userName = userName
                        Order By Convert.ToDateTime(a.compDate) Descending).ToList

        allAchs = (From aa In db.achievementLists
                        Select aa).ToList
        i = achCompleteItem.Count
        For Each ai In achCompleteItem
            achListItem = (From aa In allAchs
                            Where aa.title.Contains(ai.title)
                            Select aa).FirstOrDefault
            If achListItem IsNot Nothing Then
                allAchs.Remove(achListItem)
                achives.Controls.Add(createAchCompleteDiv(ai, achListItem.desc, i))
                i -= 1
            End If
        Next

        i = 0
        For Each aa In allAchs
            achives.Controls.Add(createAchDiv(aa, i))
            i += 1
        Next
    End Sub

    Function createAchCompleteDiv(ByVal ai As achievement,
                          ByVal alDesc As String,
                          ByVal i As Integer) As HtmlGenericControl
        '<div id="achMain<i>" Class="achMainCompleteDiv">
        '   <div id="achTitle<i>" class="achTitleCompleteDiv">
        '       <span class="achNumCompleteSpan">activity number</span>
        '       <span class="achTitleCompleteSpan">title</span>
        '       <span class="achDateCompleteSpan">date</span>
        '   </div>
        '   <div id="achDesc<i>" class="achDescDiv">
        '   </div>
        '</div>

        Dim achMainDiv As New HtmlGenericControl("div")
        Dim achTitleDiv As New HtmlGenericControl("div")
        Dim achDescDiv As New HtmlGenericControl("div")

        achMainDiv.ID = "achMain" & i
        achMainDiv.Attributes.Add("class", "achMainCompleteDiv")

        achTitleDiv.ID = "achTitle" & i
        achTitleDiv.Attributes.Add("class", "achTitleCompleteDiv")

        achTitleDiv.Controls.Add(creatAchSpan(i, "achNumCompleteSpan"))
        achTitleDiv.Controls.Add(creatAchSpan(ai.title, "achTitleCompleteSpan"))
        achTitleDiv.Controls.Add(creatAchSpan(ai.compDate, "achDateCompleteSpan"))
        achMainDiv.Controls.Add(achTitleDiv)

        achDescDiv.ID = "achDesc" & i
        achDescDiv.Attributes.Add("class", "achDescCompleteDiv")

        achDescDiv.InnerText = alDesc
        achMainDiv.Controls.Add(achDescDiv)

        Return achMainDiv
    End Function

    Function createAchDiv(ByVal ach As achievementList,
                          ByVal i As Integer) As HtmlGenericControl
        '<div id="achMain<i>" Class="achMainDiv">
        '   <div id="achTitle<i>" class="achTitleDiv">
        '       <span class="achTitle">title</span>
        '   </div>
        '   <div id="achDesc<i>" class="achDescDiv">
        '   </div>
        '</div>

        Dim achMainDiv As New HtmlGenericControl("div")
        Dim achTitleDiv As New HtmlGenericControl("div")
        Dim achDescDiv As New HtmlGenericControl("div")

        achMainDiv.ID = "achMain" & i
        achMainDiv.Attributes.Add("class", "achMainDiv")

        achTitleDiv.ID = "achTitle" & i
        achTitleDiv.Attributes.Add("class", "achTitleDiv")

        achTitleDiv.Controls.Add(creatAchSpan(ach.title, "achTitle"))
        achMainDiv.Controls.Add(achTitleDiv)

        achDescDiv.ID = "achDesc" & i
        achDescDiv.Attributes.Add("class", "achDescDiv")

        achDescDiv.InnerText = ach.desc
        achMainDiv.Controls.Add(achDescDiv)
        Return achMainDiv
    End Function

    Function creatAchSpan(ByVal innerText As String,
                          ByVal alClass As String) As HtmlGenericControl
        '<span class="alClass">innerText</span
        Dim achSpan As New HtmlGenericControl("Span")

        achSpan.ID = "ach" & alClass
        achSpan.Attributes.Add("Class", alClass)
        achSpan.InnerText = innerText

        Return achSpan
    End Function

End Class
