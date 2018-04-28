Imports System.Net.Http
Imports System.Net.NetworkInformation
Imports System.Security.Cryptography
Imports System.Text

Namespace Helpers
    Public MustInherit Class NetHelperBase
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
        Public Shared Function GetMD5(input As String) As String
            Dim result As String
            Using md5Hash As MD5 = MD5.Create()
                Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input))
                Dim sBuilder As New StringBuilder()
                Dim i As Integer
                For i = 0 To data.Length - 1
                    sBuilder.Append(data(i).ToString("x2"))
                Next i
                result = sBuilder.ToString()
            End Using
            Return result
        End Function
    End Class
    Public Interface IConnect
        Function ConnectAsync() As Task(Of String)
        Function LogOutAsync() As Task(Of String)
        Function GetFluxAsync() As Task(Of (Response As String, ErrorMessage As String))
    End Interface
End Namespace
