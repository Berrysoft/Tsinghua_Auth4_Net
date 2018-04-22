Class Application
    Private Const logPath As String = "log.xml"
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)
        Dim log As XDocument
        Try
            log = XDocument.Load(logPath)
            Dim culture As String = log.<user>.<language>.Value
            If culture IsNot Nothing Then
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(culture)
            End If
        Catch ex As Exception
            Dim state As NetState
            GetConnectableState(state)
            log =
                <?xml version="1.0" encoding="utf-8"?>
                <user>
                    <name></name>
                    <password></password>
                    <state><%= state.ToString() %></state>
                </user>
        End Try
        Dim mainWindow As New MainWindow()
        mainWindow.Show(log, logPath)
    End Sub
    Private Async Sub GetConnectableState(state As NetState)
        state = Await GetConnectableStateAsync()
    End Sub
    Private Async Function GetConnectableStateAsync() As Task(Of NetState)
        If Await NetHelperBase.CanConnect(Auth4Helper.HostName) Then
            Return NetState.Auth4
        Else
            Return NetState.Net
        End If
    End Function
    Public Sub Activate()
        CType(MainWindow, MainWindow).ShowFromMinimized()
    End Sub
End Class
