Class Application
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)
        Dim log As XDocument = XDocument.Load("log.xml")
        Dim culture As String = log.<user>.<language>.Value
        If culture IsNot Nothing Then
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(culture)
        End If
        Dim mainWindow As New MainWindow()
        mainWindow.Show()
    End Sub
    Public Sub Activate()
        CType(MainWindow, MainWindow).ShowFromMinimized()
    End Sub
End Class
