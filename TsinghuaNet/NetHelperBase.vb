Imports System.Net.Http
Imports System.Net.NetworkInformation
Imports System.Text

MustInherit Class NetHelperBase
    Private Shared ReadOnly Client As New HttpClient()
    Public ReadOnly Property Username As String
    Public ReadOnly Property Password As String
    Public Sub New(username As String, password As String)
        Me.Username = username
        Me.Password = password
    End Sub
    Public Shared Async Function CanConnect(hostName As String) As Task(Of Boolean)
        Dim ping As New Ping
        Dim reply As PingReply = Await ping.SendPingAsync(hostName)
        If reply.Status = IPStatus.Success Then
            Return True
        End If
        Return False
    End Function
    Public Shared Async Function PostAsync(url As String, data As String) As Task(Of (Response As String, ErrorMessage As String))
        Try
            Using content As New StringContent(If(data, String.Empty), Encoding.ASCII, "application/x-www-form-urlencoded")
                Using response As HttpResponseMessage = Await Client.PostAsync(url, content)
                    Dim res As String = Await response.Content.ReadAsStringAsync()
                    Return (res, Nothing)
                End Using
            End Using
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
    End Function
    Public Shared Async Function GetAsync(url As String) As Task(Of (Response As String, ErrorMessage As String))
        Try
            Dim res As String = Await Client.GetStringAsync(url)
            Return (res, Nothing)
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
    End Function
End Class

Interface IConnect
    Function ConnectAsync() As Task(Of String)
    Function LogOutAsync() As Task(Of String)
    Function GetFluxAsync() As Task(Of (Response As String, ErrorMessage As String))
End Interface