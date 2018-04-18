Class Application
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)
        Dim mainWindow As New MainWindow()
        mainWindow.Show()
    End Sub
    Public Sub Activate()
        CType(MainWindow, MainWindow).ShowFromMinimized()
    End Sub
End Class
