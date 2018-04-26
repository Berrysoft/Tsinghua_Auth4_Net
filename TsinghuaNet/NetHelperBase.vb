Imports System.Net.Http
Imports System.Net.NetworkInformation
Imports System.Text

MustInherit Class NetHelperBase
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
    Public MustOverride Async Function Connect() As Task(Of String)
    Public MustOverride Async Function LogOut() As Task(Of String)
    Public MustOverride Async Function GetFlux() As Task(Of (Response As String, ErrorMessage As String))
    Public Shared Async Function Post(url As String, data As String) As Task(Of (Response As String, ErrorMessage As String))
        Try
            Using content As New StringContent(If(data, String.Empty), Encoding.ASCII, "application/x-www-form-urlencoded")
                Using client As New HttpClient()
                    Using response As HttpResponseMessage = Await client.PostAsync(url, content)
                        Dim res As String = Await response.Content.ReadAsStringAsync()
                        Return (res, Nothing)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
    End Function
    Public Shared Async Function GetData(url As String) As Task(Of (Response As String, ErrorMessage As String))
        Try
            Using client As New HttpClient()
                Using response As HttpResponseMessage = Await client.GetAsync(url)
                    Dim res As String = Await response.Content.ReadAsStringAsync()
                    Return (res, Nothing)
                End Using
            End Using
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
    End Function
End Class