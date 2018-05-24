Imports System.Globalization
Imports System.Net.NetworkInformation
Imports System.Text
Imports Berrysoft.Console

<Settings("user")>
Class Settings
    Inherits XmlSettings
    <Settings("name")>
    Public Property Username As String
    <Settings("password")>
    Public Property Password As String
    <Settings("state")>
    Public Property State As NetState
    <Settings("more")>
    Public Property MoreInf As Boolean
    <Settings("language")>
    Public Property Language As CultureInfo
    Private Const logPath As String = "log.xml"
    Public Sub New()
        Username = String.Empty
        Password = String.Empty
        OpenAsync()
    End Sub
    Private Async Sub OpenAsync()
        Try
            Open(logPath)
        Catch ex As Exception
            WriteException(ex)
        End Try
        If State = NetState.Unknown Then
            State = Await GetConnectableStateAsync()
        End If
    End Sub
    Private Async Function GetConnectableStateAsync() As Task(Of NetState)
        Dim ping As New Ping()
        Dim response As PingReply = Await ping.SendPingAsync("auth4.tsinghua.edu.cn")
        If response.Status = IPStatus.Success Then
            Return NetState.Auth4
        Else
            Return NetState.Net
        End If
    End Function
    Public Overloads Sub Save()
        Save(logPath)
    End Sub
    Protected Overrides Function ChangeType(name As String, value As Object, conversionType As Type) As Object
        Select Case name
            Case "password"
                Return Encoding.ASCII.GetString(Convert.FromBase64String(If(value, String.Empty)))
            Case "state"
                Dim temp As NetState
                If [Enum].TryParse(value, temp) Then
                    Return temp
                Else
                    Return NetState.Unknown
                End If
            Case "more"
                Dim moreinfResult As Boolean
                If Boolean.TryParse(value, moreinfResult) AndAlso moreinfResult Then
                    Return True
                Else
                    Return False
                End If
            Case "language"
                Try
                    Return If(value Is Nothing, Nothing, New CultureInfo(value.ToString()))
                Catch ex As Exception
                    WriteException(ex)
                    Return Nothing
                End Try
            Case Else
                Return value
        End Select
    End Function
    Protected Overrides Function ChangeBackType(name As String, value As Object, conversionType As Type) As Object
        Select Case name
            Case "password"
                Return Convert.ToBase64String(Encoding.ASCII.GetBytes(value))
            Case "language"
                Return CType(value, CultureInfo)?.Name
            Case Else
                Return value.ToString()
        End Select
    End Function
End Class
