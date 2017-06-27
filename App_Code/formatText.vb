Imports Microsoft.VisualBasic

Public Module formatText

    Public Function insertLinkBreaks(ByVal desc As String) As String
        Dim i As Integer = 0
        Dim linkText As String = ""
        Dim startLinkIndex As Integer = 0
        Dim endLinkIndex As Integer = 0
        Dim links As New List(Of String)

        startLinkIndex = desc.IndexOf(vbCrLf, startLinkIndex)
        While (startLinkIndex <> -1 And endLinkIndex <> desc.Length)
            desc = desc.Replace(vbCrLf, " </br>")
            startLinkIndex = desc.IndexOf(vbCrLf, i + 1)
        End While

        Return desc
    End Function

    Function findHyperLinks(ByVal text As String) As List(Of String)
        Dim modText As List(Of String)

        modText = findHyperLinksBoth(text, "http://")
        modText.AddRange(findHyperLinksBoth(text, "https://"))
        Return modText
    End Function

    Function findHyperLinksBoth(ByVal uneditedText As String,
                                ByVal startString As String) As List(Of String)
        Dim i As Integer = 0
        Dim linkText As String = ""
        Dim startLinkIndex As Integer = 0
        Dim endLinkIndex As Integer = 0
        Dim links As New List(Of String)
        Dim secure As Boolean = False

        startLinkIndex = uneditedText.IndexOf(startString, i)
        While (startLinkIndex <> -1 And endLinkIndex <> uneditedText.Length)
            endLinkIndex = uneditedText.IndexOf(" ", startLinkIndex)
            If endLinkIndex = -1 Then
                endLinkIndex = uneditedText.Length
            End If
            linkText = uneditedText.Substring(startLinkIndex, endLinkIndex - startLinkIndex)
            links.Add(linkText)
            linkText = ""
            i = endLinkIndex
            startLinkIndex = uneditedText.IndexOf(startString, i)
        End While
        Return links
    End Function

    Function insertHyperLinks(ByVal uneditedText As String) As String
        Dim links As New List(Of String)

        links = findHyperLinks(uneditedText)
        For Each l In links
            uneditedText = uneditedText.Replace(l, createHyperLink(l))
        Next
        Return uneditedText
    End Function

    Function createHyperLink(ByVal textLink As String) As String
        Dim link As New HyperLink

        link.NavigateUrl = textLink
        link.Target = "_blank"
        link.Text = textLink

        Return "<a class='link' href='" & textLink & " ' target='_blank'>" & textLink & "</a>"
    End Function
End Module
