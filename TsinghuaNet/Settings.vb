Imports System.Globalization
Imports TsinghuaNet.Helpers

Class Settings
    Private Const logPath As String = "log.xml"
    Public Sub New()
        InitLog()
    End Sub
    Private Async Sub InitLog()
        Try
            Dim logFile As XDocument = XDocument.Load(logPath)
            Username = If(logFile.<user>.<name>.Value, String.Empty)
            Password = If(logFile.<user>.<password>.Value, String.Empty)
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
        If Await NetHelperBase.CanConnect(Auth4Helper.HostName) Then
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
                <password><%= Password %></password>
                <state><%= State.ToString() %></state>
            </user>
        If Language IsNot Nothing Then
            log.Element("user").Add(New XElement("language", Language.Name))
        End If
        log.Save(logPath)
    End Sub
    Public Property Username As String
    Public Property Password As String
    Public Property State As NetState
    Public Property Language As CultureInfo
End Class
