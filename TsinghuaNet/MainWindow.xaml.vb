Imports System.Globalization
Imports System.IO
Imports System.Threading

Class MainWindow
    Private log As XDocument
    Private logPath As String
    Private WithEvents Notify As New Forms.NotifyIcon
    Private getFluxCancellationTokeSource As CancellationTokenSource
    Public Sub New()
        InitializeComponent()
        Me.Icon = Interop.Imaging.CreateBitmapSourceFromHIcon(My.Resources.Logo.Logo.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        Notify.Text = My.Resources.NotifyText
        Notify.Icon = My.Resources.Logo.Logo
        Notify.Visible = True
    End Sub
    Friend Overloads Sub Show(log As XDocument, path As String)
        Me.log = log
        Me.logPath = path
        MyBase.Show()
    End Sub
    Private Async Sub Connect()
        CancelGetFlux()
        Dim helper As NetHelperBase = Model.Helper
        Dim connected As Boolean = False
        SetFlux(My.Resources.Connecting, Nothing, Nothing)
        Dim result As String = Await helper.Connect()
        If result IsNot Nothing Then
            MessageBox.Show(String.Format(My.Resources.ConnectionFailedWithResult, result), My.Resources.ConnectionFailed, MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            connected = True
        End If
        If connected Then GetFlux()
    End Sub
    Private Async Sub LogOut()
        CancelGetFlux()
        Dim helper As NetHelperBase = Model.Helper
        SetFlux(My.Resources.LoggingOut, Nothing, Nothing)
        Dim result As String = Await helper.LogOut()
        If result IsNot Nothing Then
            MessageBox.Show(String.Format(My.Resources.LogOutFailedWithResult, result), My.Resources.LogOutFailed, MessageBoxButton.OK, MessageBoxImage.Error)
        End If
        GetFlux()
    End Sub
    Private Async Sub GetFlux()
        CancelGetFlux()
        getFluxCancellationTokeSource = New CancellationTokenSource()
        Try
            Dim helper As NetHelperBase = Model.Helper
            Await GetFluxInternal(helper, getFluxCancellationTokeSource.Token)
        Catch ex As OperationCanceledException

        End Try
    End Sub
    Private Sub CancelGetFlux()
        getFluxCancellationTokeSource?.Cancel()
        getFluxCancellationTokeSource = Nothing
    End Sub
    Private Async Function GetFluxInternal(helper As NetHelperBase, token As CancellationToken) As Task
        If helper IsNot Nothing Then
            Dim result = Await helper.GetFlux()
            token.ThrowIfCancellationRequested()
            If result.ErrorMessage Is Nothing Then
                Dim r As String() = result.Response.Split(","c)
                If String.IsNullOrWhiteSpace(r(0)) Then
                    SetFlux(My.Resources.Disconnected, Nothing, Nothing)
                Else
                    SetFlux(r(0), CLng(r(6)), TimeSpan.FromSeconds(CLng(r(2)) - CLng(r(1))))
                End If
            Else
                SetFlux(My.Resources.NoNetwork, Nothing, Nothing)
            End If
        End If
    End Function
    Private Sub SetFlux(username As String, flux As Long?, time As TimeSpan?)
        Me.Dispatcher.BeginInvoke(
            Sub()
                Model.LoggedUsername = username
                Model.Flux = flux
                Model.OnlineTime = time
            End Sub)
    End Sub
    Private Sub Model_StateChanged(sender As Object, e As NetState) Handles Model.StateChanged
        Select Case e
            Case NetState.Auth4
                Auth4.IsChecked = True
            Case NetState.Net
                Net.IsChecked = True
        End Select
    End Sub
    Private Sub MainWindow_Loaded() Handles Me.Loaded
        Model.Username = log.<user>.<name>.Value
        Model.Password = log.<user>.<password>.Value
        Dim state As NetState
        If [Enum].TryParse(log.<user>.<state>.Value, state) Then
            Model.State = state
        End If
        GetFlux()
        Dim currentcul As CultureInfo = Thread.CurrentThread.CurrentUICulture
        Dim dirs() As String = Directory.GetDirectories(Directory.GetCurrentDirectory())
        Model.Languages.Add(New CultureInfo(""))
        For i = 0 To dirs.Length - 1
            Try
                Dim d As New DirectoryInfo(dirs(i))
                Dim culture As New CultureInfo(d.Name)
                Model.Languages.Add(culture)
                If CompareCulture(culture, currentcul) Then
                    Model.LanguagesSelectIndex = i + 1
                End If
            Catch ex As CultureNotFoundException

            End Try
        Next
    End Sub
    Private Sub MainWindow_Closed() Handles Me.Closed
        log.<user>.<name>.Value = Model.Username
        log.<user>.<password>.Value = Model.Password
        log.<user>.<state>.Value = Model.State.ToString()
        log.Save(logPath)
        Notify.Visible = False
        Notify.Dispose()
    End Sub
    Private Sub MainWindow_StateChanged(sender As Object, e As EventArgs) Handles Me.StateChanged
        If Me.WindowState = WindowState.Minimized Then
            Me.Hide()
        End If
    End Sub
    Private Sub Auth4_Checked() Handles Auth4.Checked
        Model.State = NetState.Auth4
        Me.GetFlux()
    End Sub
    Private Sub Auth6_Checked() Handles Auth6.Checked
        Model.State = NetState.Auth6
        Me.GetFlux()
    End Sub
    Private Sub Net_Checked() Handles Net.Checked
        Model.State = NetState.Net
        Me.GetFlux()
    End Sub
    Friend Sub ShowFromMinimized()
        Me.Show()
        Me.WindowState = WindowState.Normal
        Me.Activate()
        Me.GetFlux()
    End Sub
    Private Sub Notify_Click(sender As Object, e As EventArgs) Handles Notify.Click
        ShowFromMinimized()
    End Sub
    Private Sub ChangeLanguage()
        log.<user>.<language>.Value = Model.Languages(Model.LanguagesSelectIndex).Name
        MainWindow_Closed()
        Process.Start(Reflection.Assembly.GetExecutingAssembly().Location)
        Me.Close()
    End Sub
    Private Function CompareCulture(base As CultureInfo, current As CultureInfo) As Boolean
        If current.LCID = &H7F Then
            Return base.LCID = &H7F
        End If
        Return base.Name = current.Name OrElse CompareCulture(base, current.Parent)
    End Function
End Class
