Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports TsinghuaNet.Helpers

Class MainWindow
    Private log As Settings
    Private WithEvents Notify As New Forms.NotifyIcon
    Private getFluxCancellationTokeSource As CancellationTokenSource
    Public Sub New()
        InitializeComponent()
        Me.Width = My.Resources.Width
        Me.Icon = Interop.Imaging.CreateBitmapSourceFromHIcon(My.Resources.Logo.Logo.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        InitNotify()
    End Sub
    Private Sub InitNotify()
        Notify.Text = My.Resources.NotifyText
        Notify.Icon = My.Resources.Logo.Logo
        Dim menu As New Forms.ContextMenu()
        menu.MenuItems.Add(New Forms.MenuItem(My.Resources.NotifyText, AddressOf ShowFromMinimized))
        menu.MenuItems.Add(New Forms.MenuItem(My.Resources.CloseText, AddressOf Close))
        menu.RightToLeft = If(Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft, Forms.RightToLeft.Yes, Forms.RightToLeft.No)
        Notify.ContextMenu = menu
        Notify.Visible = True
    End Sub
    Friend Overloads Sub Show(log As Settings)
        Me.log = log
        MyBase.Show()
    End Sub
    Private Async Sub Connect()
        CancelGetFlux()
        Dim helper As IConnect = Model.Helper
        Dim connected As Boolean = False
        SetFlux(My.Resources.Connecting)
        Dim result As String = Await helper.ConnectAsync()
        If result IsNot Nothing Then
            MessageBox.Show(String.Format(My.Resources.ConnectionFailedWithResult, result), My.Resources.ConnectionFailed, MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            connected = True
        End If
        If connected Then GetFlux()
    End Sub
    Private Async Sub LogOut()
        CancelGetFlux()
        Dim helper As IConnect = Model.Helper
        SetFlux(My.Resources.LoggingOut)
        Dim result As String = Await helper.LogOutAsync()
        If result IsNot Nothing Then
            MessageBox.Show(String.Format(My.Resources.LogOutFailedWithResult, result), My.Resources.LogOutFailed, MessageBoxButton.OK, MessageBoxImage.Error)
        End If
        GetFlux()
    End Sub
    Private Async Sub LogOutSelected()
        Dim usereg As UseregHelper = Model.UseregHelper
        For Each user In UsersList.SelectedItems
            Await usereg.LogoutAsync(user)
        Next
        GetFlux()
    End Sub
    Private Async Sub GetFlux()
        CancelGetFlux()
        Using getFluxCancellationTokeSource = New CancellationTokenSource()
            Dim helper As IConnect = Model.Helper
            Dim usereg As UseregHelper = Model.UseregHelper
            Await GetFluxInternal(helper, usereg, getFluxCancellationTokeSource.Token)
        End Using
    End Sub
    Private Sub CancelGetFlux()
        getFluxCancellationTokeSource?.Cancel()
    End Sub
    Private Async Function GetFluxInternal(helper As IConnect, usereg As UseregHelper, token As CancellationToken) As Task
        If helper IsNot Nothing Then
            Dim result = Await helper.GetFluxAsync()
            If Not token.IsCancellationRequested Then
                If result.ErrorMessage Is Nothing Then
                    Dim r As String() = result.Response.Split(","c)
                    If String.IsNullOrWhiteSpace(r(0)) Then
                        SetFlux(My.Resources.Disconnected)
                    Else
                        SetFlux(r(0), CLng(r(6)), TimeSpan.FromSeconds(CLng(r(2)) - CLng(r(1))), CDec(r(11)))
                    End If
                Else
                    SetFlux(My.Resources.NoNetwork)
                End If
                If usereg IsNot Nothing Then
                    Dim err = Await usereg.ConnectAsync()
                    If err Is Nothing Then
                        Dim list = Await usereg.GetUsersAsync()
                        SetUsers(list)
                    End If
                End If
            End If
        End If
    End Function
    Private Sub SetFlux(username As String, Optional flux As Long? = Nothing, Optional time As TimeSpan? = Nothing, Optional balance As Decimal? = Nothing)
        Me.Dispatcher.BeginInvoke(
            Sub()
                Model.LoggedUsername = username
                Model.Flux = flux
                Model.OnlineTime = time
                Model.Balance = balance
            End Sub)
    End Sub
    Private Sub SetUsers(source As IEnumerable(Of NetUser))
        Me.Dispatcher.BeginInvoke(
            Sub()
                Model.Users.Clear()
                If source IsNot Nothing Then
                    For Each user In source
                        Model.Users.Add(user)
                    Next
                End If
            End Sub)
    End Sub
    Private Sub Model_StateChanged(sender As Object, e As NetState) Handles Model.StateChanged
        Select Case e
            Case NetState.Auth4
                Auth4.IsChecked = True
            Case NetState.Auth6
                Auth6.IsChecked = True
            Case NetState.Net
                Net.IsChecked = True
        End Select
    End Sub
    Private Sub MainWindow_Loaded() Handles Me.Loaded
        Model.Username = log.Username
        Model.Password = log.Password
        Model.State = log.State
        Model.MoreInformation = log.MoreInf
        Dim currentcul As CultureInfo = Thread.CurrentThread.CurrentUICulture
        Model.FlowDirection = If(currentcul.TextInfo.IsRightToLeft, FlowDirection.RightToLeft, FlowDirection.LeftToRight)
        Dim langs As New List(Of CultureInfo)()
        langs.Add(CultureInfo.InvariantCulture)
        For Each dirname In Directory.EnumerateDirectories(Directory.GetCurrentDirectory())
            Try
                langs.Add(New CultureInfo((New DirectoryInfo(dirname)).Name))
            Catch ex As CultureNotFoundException

            End Try
        Next
        langs.Sort(New CultureInfoComparer(currentcul))
        For i = 0 To langs.Count - 1
            Model.Languages.Add(langs(i))
            If CompareCulture(langs(i), currentcul) Then
                Model.LanguagesSelectIndex = i
            End If
        Next
    End Sub
    Private Sub MainWindow_Closed() Handles Me.Closed
        log.Username = Model.Username
        log.Password = Model.Password
        log.State = Model.State
        log.MoreInf = Model.MoreInformation
        log.Save()
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
    Private Sub Notify_MouseClick(sender As Object, e As Forms.MouseEventArgs) Handles Notify.MouseClick
        If e.Button = Forms.MouseButtons.Left Then
            ShowFromMinimized()
        End If
    End Sub
    Private Sub ChangeLanguage()
        Dim selectcul As CultureInfo = Model.Languages(Model.LanguagesSelectIndex)
        If Not CompareCulture(selectcul, Thread.CurrentThread.CurrentUICulture) Then
            log.Language = selectcul
            Me.Close()
            Forms.Application.Restart()
        End If
    End Sub
    Private Function CompareCulture(base As CultureInfo, current As CultureInfo) As Boolean
        If base.LCID = &H7F Then
            Return current.LCID = &H7F
        End If
        If current.LCID = &H7F Then
            Return base.LCID = &H7F
        End If
        Return base.Name = current.Name OrElse CompareCulture(base, current.Parent)
    End Function
    Private Sub ViewOnGitHub()
        Process.Start("https://github.com/Berrysoft/Tsinghua_Auth4_Net")
    End Sub
End Class

Class CultureInfoComparer
    Implements IComparer(Of CultureInfo)

    Private converter As CultureToDisplayString
    Private strcmp As StringComparer

    Public Sub New(current As CultureInfo)
        converter = New CultureToDisplayString()
        strcmp = StringComparer.Create(current, True)
    End Sub

    Public Function Compare(x As CultureInfo, y As CultureInfo) As Integer Implements IComparer(Of CultureInfo).Compare
        Return strcmp.Compare(converter.Convert(x, GetType(String), Nothing, Nothing), converter.Convert(y, GetType(String), Nothing, Nothing))
    End Function
End Class
