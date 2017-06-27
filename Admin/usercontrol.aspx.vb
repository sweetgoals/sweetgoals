Imports System.Net
Imports System.Net.Mail
Imports System.IO

Partial Class userControl_usercontrol
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        displayUserTable()
    End Sub

    Function checkAdminUser() As Boolean
        Dim userMembership = Membership.GetUser(HttpContext.Current.User.Identity.Name)

        If userMembership IsNot Nothing And
           HttpContext.Current.User.Identity.Name.Contains("dblake") And
           HttpContext.Current.User.Identity.IsAuthenticated = True Then
            Return True
        Else : Return False
        End If
    End Function

    Sub displayUserTable()

        If checkAdminUser() Then
            If Request.QueryString("lock") IsNot Nothing Then
                lockUser(Request.QueryString("lock"))
            ElseIf Request.QueryString("unLock") IsNot Nothing Then
                unLockUser(Request.QueryString("unLock"))
            End If
            createUserTable()
            displayPictureTable()
        Else : Response.Redirect("~/Account/Login.aspx")
        End If
    End Sub

    Sub displayPictureTable()
        Dim db As New sgdataDataContext
        Dim newPictures As IQueryable(Of activityPicture) = Nothing
        Dim picStats As List(Of String)
        Dim thegoal As New goal

        picStats = New List(Of String)(New String() {"Date", "Goal Number", "User Name", "link"})
        pictureTable.Controls.Add(pictureRow(picStats))

        newPictures = From np In db.activityPictures
                        Where np.date IsNot Nothing
                        Order By Convert.ToDateTime(np.date) Ascending
        For Each newPic In newPictures
            thegoal = (From tg In db.goals
                        Where tg.goalNumber = newPic.goalNumber
                        Select tg).FirstOrDefault

            picStats = New List(Of String)(New String() {newPic.date, newPic.goalNumber, thegoal.userName,
                                                         newPic.pictureLocation})
            pictureTable.Controls.Add(pictureRow(picStats))
        Next
    End Sub

    Function pictureRow(ByVal userStats As List(Of String)) As TableRow
        Dim arow As New TableRow
        Dim acell As New TableCell
        Dim picLink As New HyperLink
        Dim userStat As String = ""

        For Each userStat In userStats
            acell = New TableCell
            acell.Text = userStat
            If userStat.Contains("~") Then
                picLink.Text = userStat
                picLink.NavigateUrl = userStat
                picLink.Target = "_blank"
                acell.Controls.Add(picLink)
            End If
            arow.Cells.Add(acell)
        Next
        Return arow
    End Function

    Sub createUserTable()
        Dim db As New sgdataDataContext
        Dim userStats As New List(Of String)
        Dim userList As IQueryable(Of User) = Nothing
        Dim userInfo As New User
        Dim userMembership As New Membershipdb
        Dim goalCount As String = ""

        'setup the good users table
        userStats = New List(Of String)(New String() {"User Name", "Email Address", "Approved", "Create Date",
                                                     "Last Activity", " # Goals", "Lock"})
        userTable.Controls.Add(createTableRow(userStats, "Lock"))

        'setup the locked out users
        userStats = New List(Of String)(New String() {"User Name", "Email Address", "Approved", "Create Date",
                                                      "Last Activity", " # Goals", "UnLock"})
        userLockedTable.Controls.Add(createTableRow(userStats, "Unlock"))

        userList = From u In db.Users
                   Order By Convert.ToDateTime(u.LastActivityDate) Ascending

        For Each userInfo In userList
            goalCount = (From gc In db.goals
                            Where gc.userName.Contains(userInfo.UserName)).Count()
            userMembership = (From um In db.Membershipdbs
                                Where um.UserId = userInfo.UserId
                                Select um).FirstOrDefault
            If userMembership.IsLockedOut = False Then
                userStats = New List(Of String)(New String() {userInfo.UserName,
                                                              userMembership.Email,
                                                              userMembership.IsApproved.ToString,
                                                              userMembership.CreateDate.ToShortDateString,
                                                              userInfo.LastActivityDate.ToShortDateString,
                                                              goalCount})
                userTable.Controls.Add(createTableRow(userStats, "lock"))
            Else
                userStats = New List(Of String)(New String() {userInfo.UserName,
                                                              userMembership.Email,
                                                              userMembership.IsApproved.ToString,
                                                              userMembership.CreateDate.ToShortDateString,
                                                              userInfo.LastActivityDate.ToShortDateString,
                                                              goalCount})
                userLockedTable.Controls.Add(createTableRow(userStats, "unLock"))
            End If
        Next
    End Sub

    Function createTableRow(ByVal userStats As List(Of String),
                            ByVal lockOption As String) As TableRow
        Dim arow As New TableRow
        Dim acell As New TableCell
        Dim userStat As String = ""

        For Each userStat In userStats
            acell = New TableCell
            acell.Text = userStat
            arow.Cells.Add(acell)
        Next
        arow.Cells.Add(createLockCell(userStats(0), lockOption))
        Return arow
    End Function

    Function createLockCell(ByVal userName As String,
                            ByVal lockOption As String) As TableCell
        Dim lockUserCell As New TableCell
        Dim link As New HyperLink
        Dim userInfo As New User
        Dim db As New sgdataDataContext

        userInfo = (From ui In db.Users
                        Where ui.UserName.Contains(userName)
                        Select ui).FirstOrDefault
        If userInfo IsNot Nothing Then
            link.Text = userName
            If lockOption.Contains("lock") Then
                link.NavigateUrl = "usercontrol.aspx?lock=" & userName
            Else
                link.NavigateUrl = "usercontrol.aspx?unLock=" & userName
            End If
            lockUserCell.Controls.Add(link)
        End If
        Return lockUserCell
    End Function

    Sub lockUser(ByVal userName As String)
        Dim db As New sgdataDataContext
        Dim lockUserCell As New TableCell
        Dim link As New LinkButton
        Dim userMembership As New Membershipdb
        Dim userInfo As New User

        userInfo = (From ui In db.Users
                        Where ui.UserName.Contains(userName)
                        Select ui).FirstOrDefault
        If userInfo IsNot Nothing Then
            userMembership = (From um In db.Membershipdbs
                                    Where um.UserId = userInfo.UserId
                                    Select um).FirstOrDefault
            If userMembership IsNot Nothing Then
                userMembership.IsLockedOut = True
                db.SubmitChanges()
            End If
        End If
    End Sub

    Sub unLockUser(ByVal userName As String)
        Dim db As New sgdataDataContext
        Dim lockUserCell As New TableCell
        Dim link As New LinkButton
        Dim userMembership As New Membershipdb
        Dim userInfo As New User

        userInfo = (From ui In db.Users
                        Where ui.UserName.Contains(userName)
                        Select ui).FirstOrDefault
        If userInfo IsNot Nothing Then
            userMembership = (From um In db.Membershipdbs
                                    Where um.UserId = userInfo.UserId
                                    Select um).FirstOrDefault
            If userMembership IsNot Nothing Then
                userMembership.IsLockedOut = False
                db.SubmitChanges()
            End If
        End If
    End Sub
End Class
