Public Class PrintResponseModel
    Private Response As Boolean
    Private Glosa As String

    Public Sub setResponse(response As Boolean)
        Me.Response = response
    End Sub
    Public Sub setGlosa(glosa As String)
        Me.Glosa = glosa
    End Sub

    Public Function getResponse() As Boolean
        Return Me.Response
    End Function

    Public Function getGlosa() As String
        Return Me.Glosa
    End Function

End Class
