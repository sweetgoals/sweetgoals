Imports System.Collections.Generic
Imports System.Web.UI.WebControls

Partial Class Account_Manage
    Inherits System.Web.UI.Page

    Private successMessageTextValue As String
    Protected Property SuccessMessageText As String
        Get
            Return successMessageTextValue
        End Get
        Private Set(value As String)
            successMessageTextValue = value
        End Set
    End Property

    Private canRemoveExternalLoginsValue As Boolean
    Protected Property CanRemoveExternalLogins As Boolean
        Get
            Return canRemoveExternalLoginsValue
        End Get
        Set(value As Boolean)
            canRemoveExternalLoginsValue = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            ' Render success message
            Dim message = Request.QueryString("m")
            If Not message Is Nothing Then
                ' Strip the query string from action
                Form.Action = ResolveUrl("../default.aspx")

                Select Case message
                    Case "ChangePwdSuccess"
                        SuccessMessageText = "Your password has been changed."
                    Case "SetPwdSuccess"
                        SuccessMessageText = "Your password has been set."
                    Case "RemoveLoginSuccess"
                        SuccessMessageText = "The external login was removed."
                    Case Else
                        SuccessMessageText = String.Empty
                End Select

                successMessage.Visible = Not String.IsNullOrEmpty(SuccessMessageText)
            End If
        End If
    End Sub
End Class
