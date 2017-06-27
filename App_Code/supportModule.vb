Imports Microsoft.VisualBasic

Public Module supportModule
    Dim db As New sgdataDataContext

    'Public Function createSupportEmailList(ByVal theGoal As goal) As IQueryable
    '    Dim dbSupporters As IQueryable(Of goalSupport)
    '    Dim iSupport As New supporter
    '    Dim userName As String = HttpContext.Current.User.Identity.Name

    '    dbSupporters = From s In db.goalSupports
    '            Where s.goalNum = theGoal.goalNumber
    '            Order By s.number Ascending

    '    For Each dSupportEmail In dbSupporters
    '        iSupport = (From s In db.supporters
    '                    Where s.userName = userName And s.supportEmail = dSupportEmail.sEmail
    '                    Select s).SingleOrDefault
    '        'sEmailList.Add(iSupport.supportEmail)
    '        'sNameList.Add(iSupport.supportName)
    '    Next
    'End Function
End Module
