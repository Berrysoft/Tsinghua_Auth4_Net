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
    Public Shared Function CanConnect(hostName As String) As Boolean
        Dim ping As New Ping
        Dim reply As PingReply = ping.Send(hostName)
        If reply.Status = IPStatus.Success Then
            Return True
        End If
        Return False
    End Function
    Public MustOverride Function Connect() As String
    Public MustOverride Function LogOut() As String
    Public MustOverride Function GetFlux() As (Response As String, ErrorMessage As String)
    Protected Function Post(url As String, data As String) As (Response As String, ErrorMessage As String)
        Try
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            If data IsNot Nothing Then
                Dim bytes As Byte() = Encoding.ASCII.GetBytes(data)
                request.ContentLength = bytes.LongLength
                Using requestStream As Stream = request.GetRequestStream()
                    requestStream.Write(bytes, 0, bytes.Length)
                    requestStream.Flush()
                    requestStream.Close()
                End Using
            Else
                request.ContentLength = 0L
            End If
            Dim response As HttpWebResponse = request.GetResponse()
            Dim lines As String
            Using streamReader As New StreamReader(response.GetResponseStream())
                lines = streamReader.ReadToEnd()
                streamReader.Close()
            End Using
            Return (lines, Nothing)
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
    End Function
End Class
