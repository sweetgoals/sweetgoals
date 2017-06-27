
Partial Class achList
    Inherits System.Web.UI.Page

    Dim db As New sgdataDataContext
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim achList As IQueryable(Of achievementList)

        achList = From al In db.achievementLists
                    Select al
        For Each a In achList
            completeAchList.Rows.Add(createRow(a.type, a.title, a.desc))
        Next
    End Sub

    Function createRow(ByVal type As String,
                       ByVal title As String,
                       ByVal desc As String) As TableRow
        Dim arow As New TableRow

        arow.Cells.Add(createCell(type))
        arow.Cells.Add(createCell(title))
        arow.Cells.Add(createCell(desc))
        Return arow
    End Function

    Function createCell(ByVal txtString As String) As TableCell
        Dim acell As New TableCell

        acell.Text = txtString
        Return acell
    End Function
End Class
