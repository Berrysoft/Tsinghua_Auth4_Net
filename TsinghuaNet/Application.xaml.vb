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
        ChangeTheme(log.Theme)
        My.Windows.MainWindow.Show(log)
    End Sub
    Private Sub ChangeTheme(theme As Theme)
        Dim rs As New ResourceDictionary
        rs.Source = New Uri(GetXamlFileName(theme), UriKind.Relative)
        Me.Resources.MergedDictionaries.Clear()
        Me.Resources.MergedDictionaries.Add(rs)
    End Sub
    Private Function GetXamlFileName(theme As Theme) As String
        Select Case theme
            Case Theme.Dark
                Return "BlackTheme.xaml"
            Case Else
                Return "WhiteTheme.xaml"
        End Select
    End Function
    Public Function SignalExternalCommandLineArgs(args As IList(Of String)) As Boolean Implements ISingleInstanceApp.SignalExternalCommandLineArgs
        My.Windows.MainWindow.ShowFromMinimized()
        Return True
    End Function
End Class
