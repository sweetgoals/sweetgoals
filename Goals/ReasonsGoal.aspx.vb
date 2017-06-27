
Partial Class goalreasons
    Inherits System.Web.UI.Page
    Dim db As New sgdataDataContext

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim gReason As New goalreason
        Dim goalNumber As Double = 0
        Dim theGoal As New goal
        Dim userType As String = ""
        Dim reasonSplit() As String = Nothing
        Dim i As Integer = 0
        Dim closeImgEnable As Boolean = True
        Dim goalNumberShare As String = ""

        If Request.QueryString("gNum") IsNot Nothing Then
            theGoal = GetGoal(Request.QueryString("gNum"), db)
            Session("gN") = Request.QueryString("gNum")
            If theGoal IsNot Nothing Then
                goalNumberShare = checkGoalNumber(theGoal.goalNumber, userName)
            End If
        End If

        If goalNumberShare.Length > 0 Then
            Session("disableSort") = "0"
            If (Request.QueryString("gNum") IsNot Nothing) And (Not Page.IsPostBack) Then
                goalNumber = Convert.ToInt32(Request.QueryString("gNum"))
                userType = goalModule.checkGoalNumber(goalNumber, userName)
                If userType.Length > 0 Then
                    theGoal = (From g In db.goals
                                        Where g.goalNumber = goalNumber
                                        Select g).FirstOrDefault
                    If (Not theGoal.userName.ToLower.Contains(userName)) Or userName.Length = 0 Then
                        closeImgEnable = False
                        Session("disableSort") = "1"
                        reasonList.Attributes.Add("style", "list-style-type:none;")
                    End If

                    goalReasonLabel.Text = "<h1><u>" & theGoal.goalTitle & " Reasons </u></h1>"
                    gReason = (From gr In db.goalreasons
                                 Where gr.goalnum = goalNumber
                                 Select gr).FirstOrDefault
                    If Not gReason Is Nothing Then
                        reasonSplit = gReason.reason.Split("<")
                        For Each s In reasonSplit
                            If s.Contains("reason>") And s.Length > 8 Then
                                reasonList.Controls.Add(createReason(s.Substring(7), i, closeImgEnable))
                            End If
                        Next
                    End If
                    If userType <> "User" Then
                        TextBoxesGroup.Visible = False
                    End If
                Else
                    Response.Redirect("../Default.aspx")
                End If
            End If
        Else
            Response.Redirect("../Default.aspx")
        End If
    End Sub

    Function createReason(ByVal reason As String,
                          ByVal i As Integer,
                          ByVal closeImgEnable As Boolean) As HtmlGenericControl
        '<li class='reasonClass'/>
        '   <span id="rt_i" class="dSpan"> reason </span>
        '   <img id="rd_i" class="closeImg" src: "Images/closepic.png"></img>
        '</li>

        Dim reasonClass As New HtmlGenericControl("li")
        Dim dspan As New HtmlGenericControl("span")
        Dim closeImg As New HtmlGenericControl("img")

        If closeImgEnable = True Then
            closeImg.ID = "rd_" & i
            closeImg.Attributes.Add("src", "../Images/closepic.png")
            closeImg.Attributes.Add("Class", "closeImg")
            reasonClass.Attributes.Add("Class", "reasonClass")
        Else
            reasonClass.Attributes.Add("Class", "Anonymous")
        End If
        dspan.ID = "rt_" & i
        dspan.Attributes.Add("Class", "dSpan")
        dspan.InnerText = reason

        reasonClass.Controls.Add(closeImg)
        reasonClass.Controls.Add(dspan)

        Return reasonClass
    End Function

    Shared Function reasonTag(ByVal rString As String) As String
        Dim tagString As String = ""

        If rString <> "" Then
            tagString = "<reason>" & rString & "</reason>"
        End If
        Return tagString
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function saveList(ByVal gReasons As String,
                                    ByVal goalNum As String) As String
        Dim db_shared As New sgdataDataContext
        Dim reasonSplit() As String = Nothing
        Dim reasonList As String = ""
        Dim reasonGoal As New goalreason

        reasonSplit = gReasons.Split(New String() {"|||"}, StringSplitOptions.RemoveEmptyEntries)
        For Each rs In reasonSplit
            reasonList &= reasonTag(rs)
        Next
        reasonGoal = (From rg In db_shared.goalreasons
                        Where rg.goalnum = goalNum
                        Select rg).FirstOrDefault
        If reasonGoal IsNot Nothing Then
            reasonGoal.reason = reasonList
        Else
            reasonGoal = New goalreason
            reasonGoal.goalnum = goalNum
            reasonGoal.reason = reasonList
            db_shared.goalreasons.InsertOnSubmit(reasonGoal)
        End If
        db_shared.SubmitChanges()
        Return achievementModule.goalReason(HttpContext.Current.User.Identity.Name)
    End Function
End Class
