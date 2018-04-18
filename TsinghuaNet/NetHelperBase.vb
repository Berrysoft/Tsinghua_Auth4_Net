Imports System.IO
Imports System.Net
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
    Protected Async Function Post(url As String, data As String) As Task(Of (Response As String, ErrorMessage As String))
        Try
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            If data IsNot Nothing Then
                Dim bytes As Byte() = Encoding.ASCII.GetBytes(data)
                request.ContentLength = bytes.LongLength
                Using requestStream As Stream = Await request.GetRequestStreamAsync()
                    requestStream.Write(bytes, 0, bytes.Length)
                    requestStream.Flush()
                    requestStream.Close()
                End Using
            Else
                request.ContentLength = 0L
            End If
            Dim response As HttpWebResponse = Await request.GetResponseAsync()
            Dim lines As String
            Using streamReader As New StreamReader(response.GetResponseStream())
                lines = Await streamReader.ReadToEndAsync()
                streamReader.Close()
            End Using
            Return (lines, Nothing)
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
    End Function
End Class