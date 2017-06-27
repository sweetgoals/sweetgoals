Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml
Imports System.Globalization
Imports System.Collections.Specialized
Imports System.Net.Mail
Imports System.Drawing.Image

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class spursService
     Inherits System.Web.Services.WebService
    Dim mycom As New SqlCommand()
    Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=sweetgoals; User ID=dashpar; Password='mathsucks1';")

    <WebMethod()> _
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()> _
    Public Function createAccount(ByVal username As String, _
                                  ByVal password As String, _
                                  ByVal email As String) As Boolean

        Dim mycom As New SqlCommand()
        Dim mystr As String = ""
        Dim nameString = "username, pass, email"
        Dim valueString = "@userName, @pass, @email"
        Dim result As Integer
        Dim userNumber As Integer = 0
        Dim userExists As Integer = 0
        Dim i As Integer = 0

        mycom.CommandText = "SELECT COUNT(*) FROM bobpar.userList where userName='" & username & "'"
        mycom.Connection = con
        con.Open()
        userNumber = mycom.ExecuteScalar()
        con.Close()
        If userNumber = 0 Then
            mycom.Parameters.AddWithValue("@username", username.ToLower)
            mycom.Parameters.AddWithValue("@pass", password)
            mycom.Parameters.AddWithValue("@email", email)
            mycom.Connection = con
            con.Open()
            mycom.CommandText = "INSERT INTO bobpar.userList(" & nameString & ") VALUES (" & valueString & ")"
            result = mycom.ExecuteNonQuery()
            con.Close()
            If result = 1 Then
                Return True
            Else : Return False
            End If
        Else : Return False
        End If
    End Function

    <WebMethod()> _
    Sub getSupportResponses(ByVal user As String, _
                         ByVal pass As String, _
                         ByVal actNum As Integer, _
                         ByRef supports As String, _
                         ByRef responses As String)
        Dim listsupport As New List(Of String)
        Dim listresponse As New List(Of String)
        supports = ""
        responses = ""
        Dim myreader As SqlDataReader
        If verifyUserPass(user, pass) = True Then
            mycom.Parameters.Clear()
            mycom.Connection = con
            mycom.Parameters.AddWithValue("@actNumber", actNum)
            con.Open()
            mycom.CommandType = CommandType.StoredProcedure
            mycom.CommandText = "dbo.getActivityResponse"
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                supports &= myreader("supportName") & "___"
                responses &= myreader("response") & "___"
            End While
            con.Close()
        End If
        'If supports.Length = 0 Then
        '    supports = "none"
        '    responses = "none"
        'End If
    End Sub

    <WebMethod()> _
    Function checkUserAvail(ByVal user As String) As Boolean
        Dim result As Integer = 0

        mycom.Parameters.AddWithValue("@username", user)
        mycom.CommandText = "SELECT COUNT('username') from bobpar.userList WHERE username=@username"
        mycom.Connection = con
        con.Open()
        result = mycom.ExecuteScalar()
        con.Close()

        If result > 0 Then
            Return False
        Else : Return True
        End If
    End Function

    <WebMethod()> _
    Function checkUserEmail(ByVal email As String) As Boolean
        Dim result As Integer = 0

        mycom.Parameters.AddWithValue("@email", email)
        mycom.CommandText = "SELECT COUNT('email') from bobpar.userList WHERE email=@email"
        mycom.Connection = con
        con.Open()
        result = mycom.ExecuteScalar()
        con.Close()

        If result > 0 Then
            Return False
        Else : Return True
        End If
    End Function

    <WebMethod()> _
    Sub checkUser(ByVal user As String, _
                  ByVal pass As String, _
                  ByVal email As String, _
                  ByRef userVerify As Boolean, _
                  ByRef passVerify As Boolean, _
                  ByRef emailVerify As Boolean)
        Dim result As Integer = 0
        userVerify = False
        passVerify = False
        emailVerify = False

        mycom.Parameters.AddWithValue("@userName", user)
        mycom.CommandText = "SELECT COUNT('userName') from bobpar.userList WHERE userName=@userName"
        mycom.Connection = con
        con.Open()
        result = mycom.ExecuteScalar()
        con.Close()
        If result > 0 Then
            userVerify = True
        End If
        If userVerify = True Then
            mycom.Parameters.AddWithValue("@pass", pass)
            mycom.CommandText = "SELECT COUNT('userName,pass') from bobpar.userList WHERE userName=@userName AND pass=@pass"
            mycom.Connection = con
            con.Open()
            result = mycom.ExecuteScalar()
            con.Close()
            If result > 0 Then
                passVerify = True
            End If
        End If

        If (userVerify = True And passVerify = True) Then
            mycom.Parameters.AddWithValue("@email", email)
            mycom.CommandText = "SELECT COUNT('email') from bobpar.userList WHERE userName=@userName AND pass=@pass AND email=@email"
            mycom.Connection = con
            con.Open()
            result = mycom.ExecuteScalar()
            con.Close()
            If result > 0 Then
                emailVerify = True
            End If
        End If

    End Sub

    <WebMethod()> _
    Function verifyUserPass(ByVal user As String, _
                       ByVal pass As String) As Boolean
        Dim result As Integer = 0
        Dim userVerify As Boolean = False
        Dim passVerify As Boolean = False

        mycom.Parameters.AddWithValue("@username", user)
        mycom.CommandText = "SELECT COUNT('userName') from bobpar.userList WHERE userName=@userName"
        mycom.Connection = con
        con.Open()
        result = mycom.ExecuteScalar()
        con.Close()
        If result > 0 Then
            userVerify = True
        End If
        If userVerify = True Then
            mycom.Parameters.AddWithValue("@pass", pass)
            mycom.CommandText = "SELECT COUNT('username,pass') from bobpar.userList WHERE userName=@userName AND pass=@pass"
            mycom.Connection = con
            con.Open()
            result = mycom.ExecuteScalar()
            con.Close()
            If result > 0 Then
                passVerify = True
            End If
        End If
        If userVerify = True And passVerify = True Then
            Return True
        Else : Return False
        End If
    End Function

    <WebMethod()> _
    Public Function createGoal(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal goalTitle As String, _
                        ByVal goalDueDate As String, _
                        ByVal scheduleDays As String, _
                        ByVal timeLength As String, _
                        ByVal timeUnit As String, _
                        ByVal goalDesc As String) As String
        Dim result As Boolean
        Dim goalNumber As Integer = 0
        Dim userExists As Integer = 0
        Dim nameString = "userName, goalTitle, goalDueDate, scheduleDays, timeLength, timeUnit, goalDesc"
        Dim valueString = "@username, @goalTitle, @goalDueDate, @scheduleDays, @timeLength, @timeUnit, @goalDesc"
        Dim i As Integer = 0
        Dim hastitle As Integer = -1

        If verifyUserPass(userName, password) = True Then
            mycom.CommandText = "SELECT COUNT(*) FROM bobpar.goals where goalTitle='" & goalTitle & "' and userName='" & userName & "'"
            mycom.Connection = con
            con.Open()
            hastitle = mycom.ExecuteScalar()
            con.Close()
            If hastitle = 0 Then
                mycom.Parameters.Clear()
                mycom.Parameters.AddWithValue("@userName", userName.ToLower)
                mycom.Parameters.AddWithValue("@goalTitle", goalTitle)
                mycom.Parameters.AddWithValue("@goalDueDate", goalDueDate)
                mycom.Parameters.AddWithValue("@scheduleDays", scheduleDays)
                mycom.Parameters.AddWithValue("@timeLength", timeLength)
                mycom.Parameters.AddWithValue("@timeUnit", timeUnit)
                mycom.Parameters.AddWithValue("@goalDesc", goalDesc)
                mycom.Connection = con
                con.Open()
                mycom.CommandText = "INSERT INTO bobpar.goals(" & nameString & ") VALUES (" & valueString & ")"
                result = mycom.ExecuteNonQuery()
                con.Close()
            End If
            If result = True Then
                Return "Goal Created"
            Else : Return "Goal Not Created"
            End If
        Else : Return "Invalid User/Password"
        End If
    End Function

    <WebMethod()> _
    Sub getGoal(ByVal user As String, _
                ByVal pass As String, _
                ByVal goalTitle As String, _
                ByRef goalNumber As String, _
                ByRef goalDueDate As String, _
                ByRef scheduleDays As String, _
                ByRef timeLength As String, _
                ByRef timeUnit As String)

        Dim myreader As SqlDataReader
        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT goalTitle, goalNumber, goalDueDate, scheduleDays, timeLength, timeUnit from bobpar.goals where " + _
                                    "bobpar.goals.userName='" & user & "' AND bobpar.goals.goalTitle='" & goalTitle & "'"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                goalNumber = myreader("goalNumber")
                goalDueDate = myreader("goalDueDate")
                scheduleDays = myreader("scheduleDays")
                timeLength = myreader("timeLength")
                timeUnit = myreader("timeUnit")
            End While
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Public Function goalSummary(ByVal user As String, _
                                ByVal pass As String) As String
        'Dim goalList As String = ""
        Dim myreader As SqlDataReader
        Dim goalList As String = ""

        If verifyUserPass(user, pass) = True Then
            'mycom.CommandText = "SELECT bobpar.goals.goalTitle from bobpar.userList INNER JOIN bobpar.goals" +
            '                    " on bobpar.userList.userNumber=bobpar.goals.usernumber" +
            '                    " where bobpar.userList.userName='" & user & "'"
            mycom.CommandText = "SELECT goalTitle FROM bobpar.goals WHERE (username='" & user & "')"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                goalList += myreader("goalTitle") + "___"
            End While
            con.Close()
        End If
        If goalList <> "" Then
            Return goalList
        Else : Return "Add Some Goals :-)"
        End If
    End Function

    <WebMethod()> _
    Public Sub writeActivity(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal goalNumber As Integer, _
                        ByVal actTitle As String, _
                        ByVal adesc As String, _
                        ByVal startTime As String, _
                        ByVal actDate As String)


        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@userName", userName.ToLower)
            mycom.Parameters.AddWithValue("@goalNumber", goalNumber)
            mycom.Parameters.AddWithValue("@actTitle", actTitle)
            mycom.Parameters.AddWithValue("@desc", adesc)
            mycom.Parameters.AddWithValue("@startTime", startTime)
            mycom.Parameters.AddWithValue("@actDate", actDate)

            mycom.Connection = con
            con.Open()
            mycom.CommandType = CommandType.StoredProcedure
            mycom.CommandText = "dbo.insertActivityItem"
            mycom.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Public Function listActivity(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal goalTitle As String) As String

        Dim myreader As SqlDataReader
        Dim activityList As String = ""

        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@userName", userName.ToLower)
            mycom.Parameters.AddWithValue("@goalTitle", goalTitle)
            mycom.Connection = con
            con.Open()
            mycom.CommandType = CommandType.StoredProcedure
            mycom.CommandText = "dbo.listActivity"
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                activityList += Convert.ToString(myreader("actNumber")) + "@" + myreader("actTitle") + "___"
            End While
            con.Close()
        End If
        If activityList <> "" Then
            Return activityList
        Else : Return "No Activities"
        End If

    End Function

    <WebMethod()> _
    Public Sub finishActivity(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal actNumber As String, _
                        ByVal good As String, _
                        ByVal bad As String, _
                        ByVal stopTime As String, _
                        ByVal timeDiff As String)
        Dim nameString = "actNumber, good, bad, stopTime, timeDiff"
        Dim valueString = "@actNumber, @good, @bad, @stopTime, @timeDiff"
        Dim pullNames As String() = {"goalNumber", "actDesc", "actDate", "startTime"}
        Dim pullDesc As String() = {"Goal Number:", "Activity Description:", "Activity Date:", "Start Time:"}
        Dim goalNumber As Integer = 0
        Dim callSupportString As String = ""
        Dim supporters As String = ""
        Dim supportList() As String = Nothing
        Dim emailList As List(Of String) = Nothing
        Dim supportNumber As List(Of Integer) = Nothing
        Dim supportNumberStr As String = ""
        Dim fromEmail As String = "thebigM@yobbers.com"
        Dim toEmail As String = "" '"apchampagne@gmail.com"
        Dim MailMsg As MailMessage
        Dim myreader As SqlDataReader
        Dim result As Integer = 0
        Dim values As List(Of String) = Nothing
        Dim imgList As List(Of String) = Nothing
        Dim unconfirmedList As String = ""

        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@actNumber", actNumber)
            mycom.Parameters.AddWithValue("@good", good)
            mycom.Parameters.AddWithValue("@bad", bad)
            mycom.Parameters.AddWithValue("@stopTime", stopTime)
            mycom.Parameters.AddWithValue("@timeDiff", timeDiff)
            con.Open()
            mycom.CommandText = "UPDATE bobpar.activities SET good=@good, bad=@bad, stopTime=@stoptime, " + _
                                "timeDiff=@timediff WHERE actNumber=@actNumber"
            result = mycom.ExecuteNonQuery()
            con.Close()

            values = New List(Of String)
            mycom.Parameters.Clear()
            mycom.CommandText = "SELECT goalNumber,actDesc,actDate,startTime FROM bobpar.activities where actNumber='" & actNumber & "'"
            con.Open()
            myreader = mycom.ExecuteReader
            While myreader.Read()
                For i = 0 To 3 Step 1
                    values.Add(pullDesc(i) & myreader(i))
                Next
                'values.Add("Goal Number:" & myreader("goalnumber"))
                'values.Add("Activity Description:" & myreader("actDesc"))
                'values.Add("Activity Date:" & myreader("actDate"))
                'values.Add("Start Time:" & myreader("startTime"))
            End While
            con.Close()
            mycom.Parameters.Clear()
            mycom.CommandText = "SELECT pictureLocation,picDesc FROM sweetgoals.bobpar.activityPictures where actNumber='" & actNumber & "'"
            con.Open()
            myreader = mycom.ExecuteReader
            imgList = New List(Of String)
            While myreader.Read()
                imgList.Add(myreader("pictureLocation") & "___" & myreader("picDesc"))
            End While
            con.Close()

            mycom.Parameters.Clear()
            mycom.CommandText = "SELECT supporters FROM sweetgoals.bobpar.goals where goalNumber='" & values(0).Split(":")(1) & "'"
            con.Open()
            myreader = mycom.ExecuteReader
            While myreader.Read()
                supporters = myreader("supporters")
            End While
            con.Close()
            supportList = supporters.Split(New String() {"___"}, StringSplitOptions.RemoveEmptyEntries)
            callSupportString = "SELECT supportNumber, supportEmail FROM sweetgoals.bobpar.supporters where "
            For i = 0 To supportList.Count - 2 Step 1
                callSupportString += "supportName='" & supportList(i) & "' or "
            Next
            callSupportString += "supportName='" & supportList.Last & "'"
            emailList = New List(Of String)
            supportNumber = New List(Of Integer)
            mycom.CommandText = callSupportString
            con.Open()
            myreader = mycom.ExecuteReader
            While myreader.Read()
                emailList.Add(myreader("supportEmail"))
                supportNumber.Add(myreader("supportNumber"))
            End While
            con.Close()
            'Smtpclient to send the mail message
            Dim SmtpMail As New SmtpClient
            '***** use this when coming from letstrend.com **********
            'SmtpMail.Host = "smtp.yobbers.com"

            '************* use this when testing locally *************
            SmtpMail.Host = "smtp.cox.net"
            Dim netcred As New NetworkCredential("thebigM@yobbers.com", "Ihateyou123")
            SmtpMail.Credentials = netcred
            Dim j As Integer = 0
            Dim imgStrings() As String

            For i = 0 To emailList.Count - 1 Step 1
                unconfirmedList &= emailList(i) & "___"
                MailMsg = New MailMessage(New MailAddress(fromEmail.Trim()), New MailAddress(emailList(i).Trim()))
                MailMsg.BodyEncoding = Encoding.Default
                MailMsg.Subject = "Activity Verification for " & userName
                MailMsg.Body = "Hi " & supportList(i) & ",<br> " & emailList(i) & "<br><br><br>"
                MailMsg.Body &= "<center>"
                For j = 0 To values.Count - 1 Step 1
                    MailMsg.Body &= values(j) & "<br>"
                Next
                MailMsg.Body &= "Stop Time:" & stopTime & "<br>"
                MailMsg.Body &= "Elapsed Time:" & timeDiff & "<br>"
                MailMsg.Body &= "Good News:" & good & "<br>"
                MailMsg.Body &= "Bad News:" & bad & "<br>"
                For j = 0 To imgList.Count - 1 Step 1
                    imgStrings = imgList(j).Split(New String() {"___"}, StringSplitOptions.RemoveEmptyEntries)
                    MailMsg.Body &= "<img src='" & imgStrings(0) & "' width='600' height='400'/> <br>"
                    MailMsg.Body &= "Description: " & imgStrings(1) & " <br>"
                Next
                '<input id="Button8" type="button" value="button" /><br />
                MailMsg.Body &= "<a href='http://www.yobbers.com/confirm.aspx?confirm=1&actNumber=" & actNumber & _
                                "&email=" & supportNumber(i) & "'>Yes</a> <br />"
                MailMsg.Body &= "<a href='http://www.yobbers.com/confirm.aspx?confirm=0&actNumber=" & actNumber & _
                                "&email=" & supportNumber(i) & "'>no</a> <br />"
                MailMsg.Body &= "Please Verify activity was completed for " & userName & " <br>"
                MailMsg.Body &= "By reviewing data sent in this email and clicking on the 'Verified' Button at the bottom <br></center>"
                MailMsg.Priority = MailPriority.Normal
                MailMsg.IsBodyHtml = True
                SmtpMail.Send(MailMsg)
            Next
            supportNumberStr = ""
            For i = 0 To supportNumber.Count - 1 Step 1
                supportNumberStr &= supportNumber(i) & "___"
            Next
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@unconfirmed", supportNumberStr)
            mycom.Parameters.AddWithValue("@actNumber", actNumber)
            mycom.CommandText = "UPDATE sweetgoals.bobpar.activities SET unconfirmed=@unconfirmed, supportConfirmed='No' WHERE actNumber=@actNumber"
            con.Open()
            result = mycom.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Public Sub processConfirmEmail(ByVal actNumber As Integer, _
                                   ByVal sEmail As String)
        Dim myreader As SqlDataReader
        Dim result As Integer = 0
        Dim unconfirmed, supportConfirmed As List(Of String)
        Dim supportConfirmedStr As String = ""
        Dim i As Integer = 0

        unconfirmed = New List(Of String)
        supportConfirmed = New List(Of String)
        mycom.Parameters.Clear()
        mycom.CommandText = "SELECT unconfirmed,confirmed from sweetgoals.bobpar.activities where actNumber='" & actNumber & "'"
        mycom.Connection = con
        con.Open()
        myreader = mycom.ExecuteReader()
        While myreader.Read()
            unconfirmed = myreader("unconfirmed").ToString.Split(New String() {"___"}, StringSplitOptions.RemoveEmptyEntries).ToList
            supportConfirmedStr = myreader("confirmed")
            supportConfirmed = supportConfirmedStr.Split(New String() {"___"}, StringSplitOptions.RemoveEmptyEntries).ToList
        End While
        con.Close()
        If unconfirmed.Contains(sEmail) And (Not supportConfirmed.Contains(sEmail)) Then
            supportConfirmedStr &= sEmail & "___"

            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@confirmed", supportConfirmedStr)
            mycom.Parameters.AddWithValue("@actNumber", actNumber)
            con.Open()
            mycom.CommandText = "UPDATE sweetgoals.bobpar.activities SET confirmed@confirmed WHERE actNumber=@actNumber"
            result = mycom.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Public Sub activitySummary(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal actNumber As Integer, _
                        ByRef values As String, _
                        ByRef picLocs As String)

        Dim myreader As SqlDataReader
        Dim acttime As String = ""
        Dim timeUnit As String = ""
        Dim minuteDiff As Long = 0
        Dim hourDiff As Long = 0
        Dim i As Integer = 0
        Dim columnsFront As String() = {"<actTitle>", "<actDesc>", "<good>", "<bad>", "<supportConfirmed>", "<confirmed>", "<actDate>", "<startTime>", "<stopTime>", "<timeDiff>", "<userName>"}
        Dim columnsBack As String() = {"</actTitle>", "</actDesc>", "</good>", "</bad>", "</supportConfirmed>", "</confirmed>", "</actDate>", "</startTime>", "</stopTime>", "</timeDiff>", "</userName>"}
        'Dim workTime As String = ""
        acttime = ""
        timeUnit = ""

        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.CommandText = "SELECT actTitle,actDesc,good,bad,supportConfirmed,confirmed,actDate,startTime, stopTime, timeDiff, userName " + _
                                "from sweetgoals.bobpar.activities where actNumber='" & actNumber & "'"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                For i = 0 To 9 Step 1
                    values += columnsFront(i) & myreader(i) & columnsBack(i)
                Next
            End While
            con.Close()

            mycom.Parameters.Clear()
            mycom.CommandText = "SELECT pictureNumber, picDesc from sweetgoals.bobpar.activityPictures where actNumber='" & actNumber & "'"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                picLocs += Convert.ToString(myreader("pictureNumber")) + "@" + myreader("picDesc") + "___"
            End While
            con.Close()
            If picLocs = "" Then
                picLocs = "No Pictures"
            End If
        End If
    End Sub

    <WebMethod()> _
    Function createSupporter(ByVal user As String, _
                        ByVal pass As String, _
                        ByVal supportName As String, _
                        ByVal supportEmail As String) As String
        Dim nameString = "userName, supportName, supportEmail"
        Dim valueString = "@username, @supportName, @supportEmail"
        Dim supportNumber = 0
        Dim result As Boolean
        Dim myreader As SqlDataReader
        Dim sqlName As String = ""

        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportName FROM bobpar.supporters WHERE (username='" & user & "' AND supportName='" & supportName & "')"
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                sqlName = myreader("supportName")
            End While
            con.Close()
            If sqlName.Length = 0 Then
                mycom.CommandText = "SELECT TOP 1 supportNumber FROM bobpar.supporters ORDER BY supportNumber DESC"
                mycom.Connection = con
                con.Open()
                supportNumber = mycom.ExecuteScalar() + 1
                con.Close()

                mycom.Parameters.Clear()
                mycom.Parameters.AddWithValue("@userName", user.ToLower)
                mycom.Parameters.AddWithValue("@supportName", supportName)
                mycom.Parameters.AddWithValue("@supportEmail", supportEmail)

                mycom.Connection = con
                con.Open()
                mycom.CommandText = "INSERT INTO bobpar.supporters(" & nameString & ") VALUES (" & valueString & ")"
                result = mycom.ExecuteNonQuery()
                con.Close()
                Return "Created"
            Else : Return "Duplicate"
            End If
        Else : Return "Bad User"
        End If
    End Function

    <WebMethod()> _
    Function listSupporters(ByVal user As String, _
                            ByVal pass As String) As String
        Dim myreader As SqlDataReader
        Dim supportList As String = ""

        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportName FROM bobpar.supporters WHERE (username='" & user & "')"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                supportList += myreader("supportName") + "___"
            End While
            con.Close()
        End If
        If supportList <> "" Then
            Return supportList
        Else : Return "Add Some Support :-)"
        End If
    End Function

    <WebMethod()> _
    Function listGoalSupporters(ByVal user As String, _
                                ByVal pass As String, _
                                ByVal goalTitle As String) As String
        Dim myreader As SqlDataReader
        Dim supportList As String = ""

        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supporters FROM bobpar.goals WHERE (username='" & user & "' AND goalTitle='" & goalTitle & "')"
            mycom.Connection = con
            con.Open()
            Try
                myreader = mycom.ExecuteReader()
                While myreader.Read()
                    supportList = myreader("supporters")
                End While
            Catch ex As Exception
                supportList = ""
            End Try
            con.Close()
        End If
        If supportList <> "" Then
            Return supportList
        Else : Return "Add Some Support :-)"
        End If
    End Function

    <WebMethod()> _
    Function getSupportEmail(ByVal user As String, _
                             ByVal pass As String, _
                             ByVal supportName As String) As String
        Dim result As Integer = 0
        Dim myreader As SqlDataReader
        Dim supportEmail As String = ""
        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportEmail FROM bobpar.supporters WHERE (userName='" & user & "' AND supportName='" & supportName & "')"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                supportEmail = myreader("supportEmail")
            End While
            con.Close()
        End If
        If supportEmail <> "" Then
            Return supportEmail
        Else : Return ""
        End If
    End Function

    <WebMethod()> _
    Function updateSupport(ByVal user As String, _
                      ByVal pass As String, _
                      ByVal oldSupportName As String, _
                      ByVal newSupportName As String, _
                      ByVal newSupportEmail As String) As String

        Dim result As Integer = 0
        Dim supportEmail As String = ""
        Dim sqlName As String = ""
        Dim myreader As SqlDataReader
        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportName FROM bobpar.supporters WHERE (username='" & user & "' AND supportName='" & newSupportName & "')"
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                sqlName = myreader("supportName")
            End While
            con.Close()
            If sqlName.Length = 0 Then
                mycom.Parameters.Clear()
                mycom.Parameters.AddWithValue("@userName", user)
                mycom.Parameters.AddWithValue("@oldSupportName", oldSupportName)
                mycom.Parameters.AddWithValue("@newSupportName", newSupportName)
                mycom.Parameters.AddWithValue("@newSupportEmail", newSupportEmail)

                mycom.CommandText = "UPDATE bobpar.supporters SET supportName=@newSupportName, supportEmail=@newSupportEmail " + _
                                    "WHERE userName=@userName and supportName=@oldSupportName"
                mycom.Connection = con
                con.Open()
                result = mycom.ExecuteNonQuery()
                con.Close()
                Return "Modified"
            Else : Return "Duplicate"
            End If
        Else : Return "Bad User"
        End If
    End Function

    <WebMethod()> _
    Sub addGoalSupport(ByVal user As String, _
                        ByVal pass As String, _
                        ByVal goalTitle As String, _
                        ByVal goalSup As String)
        Dim nameString = "userName, goalTitle, supporters"
        Dim valueString = "@username, @goalTitle, @supporters"
        Dim result As Integer = 0

        If verifyUserPass(user, pass) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@userName", user.ToLower)
            mycom.Parameters.AddWithValue("@goalTitle", goalTitle)
            mycom.Parameters.AddWithValue("@supporters", goalSup)

            con.Open()
            mycom.CommandText = "UPDATE bobpar.goals SET supporters=@supporters WHERE userName=@userName AND goalTitle=@goalTitle"
            result = mycom.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Sub sendImage(ByVal user As String, _
                  ByVal pass As String, _
                  ByVal actNumber As Integer, _
                  ByVal picDesc As String, _
                  ByVal myImage As Byte())

        Dim FilePath As String = Server.MapPath("~/actpic/")
        Dim picLoc As String = ""
        Dim savePicNum As Integer = 0
        Dim seqNum As Integer = 0
        Dim nameString = "actNumber, pictureLocation, picDesc"
        Dim valueString = "@actNumber, @pictureLocation, @picDesc"
        Dim result As Integer = 0

        If verifyUserPass(user, pass) Then
            mycom.CommandText = "SELECT COUNT(*) FROM bobpar.activityPictures"
            mycom.Connection = con
            con.Open()
            savePicNum = mycom.ExecuteScalar() + 1
            con.Close()
            picLoc = "http://www.yobbers.com/actpic/" & "pictureNumber" & savePicNum & "actNumber" & actNumber & ".jpg"

            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@actNumber", actNumber)
            mycom.Parameters.AddWithValue("@pictureLocation", picLoc)
            mycom.Parameters.AddWithValue("@picDesc", picDesc)
            mycom.Connection = con
            con.Open()
            mycom.CommandText = "INSERT INTO bobpar.activityPictures(" & nameString & ") VALUES (" & valueString & ")"
            result = mycom.ExecuteNonQuery()
            con.Close()
            System.IO.File.WriteAllBytes(FilePath & "pictureNumber" & savePicNum & "actNumber" & actNumber & ".jpg", myImage)
        End If
    End Sub

    <WebMethod()> _
    Function getPicture(ByVal user As String, _
                        ByVal pass As String, _
                        ByVal picNumber As Integer) As String

        Dim picLoc As String = ""
        Dim myreader As SqlDataReader

        If verifyUserPass(user, pass) Then
            mycom.CommandText = "SELECT pictureLocation FROM bobpar.activityPictures where pictureNumber='" & picNumber & "'"
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                picLoc = myreader("pictureLocation")
            End While
            con.Close()
        End If
        If picLoc <> "" Then
            Return picLoc
        Else : Return ""
        End If
    End Function
End Class

'If stoptime <> "" Then
'minuteDiff = DateDiff(DateInterval.Minute, Convert.ToDateTime(starttime), Convert.ToDateTime(stoptime))
'hourDiff = DateDiff(DateInterval.Hour, Convert.ToDateTime(starttime), Convert.ToDateTime(stoptime))
'If minuteDiff >= 60 Then
'    hourDiff = minuteDiff \ 60
'    minuteDiff = minuteDiff Mod 60
'    If minuteDiff = 0 Then
'        workTime = hourDiff.ToString + ":00"
'    ElseIf minuteDiff < 10 Then
'        workTime = hourDiff.ToString + ":0" + minuteDiff.ToString
'    Else
'        workTime = hourDiff.ToString + ":" + minuteDiff.ToString
'    End If
'ElseIf minuteDiff < 10 Then
'    workTime = "0:0" + minuteDiff.ToString
'Else : workTime = "0:" + minuteDiff.ToString
'End If
'Else
'stoptime = "BAD"
'End If
