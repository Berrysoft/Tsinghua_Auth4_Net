﻿Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports Berrysoft.Tsinghua.Net

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
        Notify.Visible = True
    End Sub
    Friend Overloads Sub Show(log As Settings)
        Me.log = log
        MyBase.Show()
    End Sub
    Private Async Sub Connect()
        WriteEvent("开始登录")
        CancelGetFlux()
        Dim helper As IConnect = Model.Helper
        Using getFluxCancellationTokeSource = New CancellationTokenSource()
            Dim token = getFluxCancellationTokeSource.Token
            SetFlux(My.Resources.Connecting)
            Try
                Dim res = Await helper.LoginAsync()
                WriteLog($"回复: {res.Message}")
                WriteEvent("登录成功")
            Catch ex As Exception
                If Not token.IsCancellationRequested Then
                    SetFlux(My.Resources.ConnectionFailed)
                    WriteException(ex)
                End If
                SetFlux(My.Resources.NoNetwork)
            End Try
        End Using
        Await GetFlux(helper)
    End Sub
    Private Sub LogOut()
        LogoutWithUsername()
    End Sub
    Private Async Sub LogoutWithUsername()
        WriteEvent("开始注销")
        CancelGetFlux()
        Dim helper As IConnect = Model.Helper
        Using getFluxCancellationTokeSource = New CancellationTokenSource()
            Dim token = getFluxCancellationTokeSource.Token
            SetFlux(My.Resources.LoggingOut)
            Try
                Dim res = Await helper.LogoutAsync()
                WriteLog($"回复: {res.Message}")
                WriteEvent("注销成功")
            Catch ex As Exception
                If Not token.IsCancellationRequested Then
                    SetFlux(My.Resources.LogOutFailed)
                    WriteException(ex)
                End If
                SetFlux(My.Resources.NoNetwork)
            End Try
        End Using
        Await GetFlux(helper)
    End Sub
    Private Async Sub LogOutSelected()
        CancelGetFlux()
        Dim usereg As UseregHelper = Model.UseregHelper
        Using getFluxCancellationTokeSource = New CancellationTokenSource()
            Dim token = getFluxCancellationTokeSource.Token
            Try
                Await usereg.LoginAsync()
                For Each user As DependencyNetUser In UsersList.SelectedItems
                    Try
                        WriteEvent($"注销IP: {user.Address}")
                        Dim res = Await usereg.LogoutAsync(user.Address)
                        WriteLog($"回复: {res}")
                    Catch ex As Exception
                        WriteException(ex)
                    End Try
                Next
            Catch ex As Exception
                If Not token.IsCancellationRequested Then
                    WriteException(ex)
                End If
            End Try
        End Using
        GetFlux()
    End Sub
    Private Async Sub GetFlux()
        Await GetFlux(Model.Helper)
    End Sub
    Private Async Function GetFlux(helper As IConnect) As Task
        CancelGetFlux()
        Using getFluxCancellationTokeSource = New CancellationTokenSource()
            Dim usereg As UseregHelper = Model.UseregHelper
            Await GetFluxInternal(helper, usereg, getFluxCancellationTokeSource.Token)
        End Using
    End Function
    Private Sub CancelGetFlux()
        getFluxCancellationTokeSource?.Cancel()
    End Sub
    Private Async Function GetFluxInternal(helper As IConnect, usereg As UseregHelper, token As CancellationToken) As Task
        If helper IsNot Nothing Then
            Dim result As FluxUser = Nothing
            Try
                WriteEvent("开始刷新")
                result = Await helper.GetFluxAsync()
                If Not token.IsCancellationRequested Then
                    If result IsNot Nothing Then
                        SetFlux(result.Username, result.Flux, result.OnlineTime, result.Balance)
                    Else
                        SetFlux(My.Resources.Disconnected)
                    End If
                Else
                    WriteEvent("刷新已取消")
                    Return
                End If
                WriteEvent("流量获取成功")
            Catch ex As Exception
                SetFlux(My.Resources.NoNetwork)
                WriteException(ex)
            End Try
            If Not token.IsCancellationRequested Then
                If usereg IsNot Nothing Then
                    Try
                        Await usereg.LoginAsync()
                        Dim list = (Await usereg.GetUsersAsync()).Select(AddressOf DependencyNetUser.Create)
                        SetUsers(list)
                        WriteEvent("在线信息获取成功")
                    Catch ex As Exception
                        WriteException(ex)
                    End Try
                End If
            Else
                WriteEvent("刷新已取消")
                Return
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
    Private Sub SetUsers(source As IEnumerable(Of DependencyNetUser))
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
    Private Sub Model_ThemeChanged(sender As Object, e As Theme) Handles Model.ThemeChanged
        Select Case e
            Case Theme.Light
                Light.IsChecked = True
            Case Theme.Dark
                Dark.IsChecked = True
        End Select
    End Sub
    Private Sub MainWindow_Loaded() Handles Me.Loaded
        WriteEvent("加载设置")
        Model.Username = log.Username
        Model.Password = log.Password
        Model.State = log.State
        Model.Theme = log.Theme
        Dim currentcul As CultureInfo = Thread.CurrentThread.CurrentUICulture
        Model.FlowDirection = If(currentcul.TextInfo.IsRightToLeft, FlowDirection.RightToLeft, FlowDirection.LeftToRight)
        WriteEvent("加载语言")
        Dim langs As New List(Of CultureInfo)()
        langs.Add(DefaultCultureInfo)
        For Each dirname In Directory.EnumerateDirectories(Directory.GetCurrentDirectory())
            Try
                langs.Add(New CultureInfo((New DirectoryInfo(dirname)).Name))
            Catch ex As CultureNotFoundException
                WriteException(ex)
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
        log.Save()
        Notify.Visible = False
        Notify.Dispose()
        Model.DisposeHelpers()
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
        Select Case e.Button
            Case Forms.MouseButtons.Left
                ShowFromMinimized()
            Case Forms.MouseButtons.Right
                Dim menu As ContextMenu = FindResource("NotifyContectMenu")
                menu.IsOpen = Not menu.IsOpen
        End Select
    End Sub
    Private Sub Light_Checked() Handles Light.Checked
        Model.Theme = Theme.Light
    End Sub
    Private Sub Dark_Checked() Handles Dark.Checked
        Model.Theme = Theme.Dark
    End Sub
    Private Sub ChangeTheme()
        If Model.Theme <> log.Theme Then
            WriteEvent($"切换主题到{Model.Theme}")
            log.Theme = Model.Theme
            Me.Close()
            Forms.Application.Restart()
        End If
    End Sub
    Private Sub ChangeLanguage()
        Dim selectcul As CultureInfo = Model.Languages(Model.LanguagesSelectIndex)
        If Not CompareCulture(selectcul, Thread.CurrentThread.CurrentUICulture) Then
            WriteEvent($"切换语言到{selectcul.Name}")
            log.Language = selectcul
            Me.Close()
            Forms.Application.Restart()
        End If
    End Sub
    Private Function CompareCulture(base As CultureInfo, current As CultureInfo) As Boolean
        If base.LCID = &H7F Then
            Return current.LCID = DefaultCultureInfo.LCID
        End If
        If current.LCID = &H7F Then
            Return False
        End If
        Return base.LCID = current.LCID OrElse CompareCulture(base, current.Parent)
    End Function
End Class

Class CultureInfoComparer
    Implements IComparer(Of CultureInfo)

    Private converter As CultureToString
    Private strcmp As StringComparer

    Public Sub New(current As CultureInfo)
        converter = New CultureToString()
        strcmp = StringComparer.Create(current, True)
    End Sub

    Public Function Compare(x As CultureInfo, y As CultureInfo) As Integer Implements IComparer(Of CultureInfo).Compare
        Return strcmp.Compare(converter.Convert(x, GetType(String), Nothing, Nothing), converter.Convert(y, GetType(String), Nothing, Nothing))
    End Function
End Class
