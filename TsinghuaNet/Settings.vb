Imports System.Globalization
Imports System.Net.NetworkInformation
Imports System.Text
Imports Berrysoft.Tsinghua.Net

Class Settings
    Private Const logPath As String = "log.xml"
    Public Sub New()
        InitLog()
    End Sub
    Private Async Sub InitLog()
        Try
            Dim logFile As XDocument = XDocument.Load(logPath)
            Username = If(logFile.<user>.<name>.Value, String.Empty)
            Dim passwordstr As String = logFile.<user>.<password>.Value
            If passwordstr Is Nothing Then
                Password = String.Empty
            Else
                Password = Encoding.ASCII.GetString(Convert.FromBase64String(passwordstr))
            End If
            Dim statestr As String = logFile.<user>.<state>.Value
            If statestr Is Nothing Then
                State = Await GetConnectableStateAsync()
            Else
                Dim temp As NetState
                If [Enum].TryParse(statestr, temp) Then
                    State = temp
                Else
                    State = Await GetConnectableStateAsync()
                End If
            End If
            Dim moreinf As String = logFile.<user>.<more>.Value
            Dim moreinfResult As Boolean
            If Boolean.TryParse(moreinf, moreinfResult) AndAlso moreinfResult Then
                Me.MoreInf = True
            End If
            Dim lang As String = logFile.<user>.<language>.Value
            If lang IsNot Nothing Then
                Try
                    Language = New CultureInfo(lang)
                Catch ex As Exception

                End Try
            End If
        Catch ex As Exception
            InitBlankLog()
        End Try
    End Sub
    Private Async Sub InitBlankLog()
        Username = String.Empty
        Password = String.Empty
        State = Await GetConnectableStateAsync()
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
    Public Sub Save()
        Dim log As XDocument =
            <?xml version="1.0" encoding="utf-8"?>
            <user>
                <name><%= Username %></name>
                <password><%= Convert.ToBase64String(Encoding.ASCII.GetBytes(Password)) %></password>
                <state><%= State.ToString() %></state>
                <more><%= MoreInf.ToString() %></more>
            </user>
        If Language IsNot Nothing Then
            log.Element("user").Add(New XElement("language", Language.Name))
        End If
        log.Save(logPath)
    End Sub
    Public Property Username As String
    Public Property Password As String
    Public Property State As NetState
    Public Property MoreInf As Boolean
    Public Property Language As CultureInfo
End Class
