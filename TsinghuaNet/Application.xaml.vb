Class Application
    Implements ISingleInstanceApp
    Private log As Settings
    Public Sub New()
        InitializeComponent()
        log = New Settings()
    End Sub
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)
        Dim cul = log.Language
        If cul IsNot Nothing Then
            If cul.LCID = &H7F Then
                cul = DefaultCultureInfo
            End If
            Threading.Thread.CurrentThread.CurrentUICulture = cul
        End If
        My.Windows.MainWindow.Show(log)
    End Sub
    Public Function SignalExternalCommandLineArgs(args As IList(Of String)) As Boolean Implements ISingleInstanceApp.SignalExternalCommandLineArgs
        My.Windows.MainWindow.ShowFromMinimized()
        Return True
    End Function
End Class
