﻿Imports System.Threading

Class MainWindow
    Private log As XDocument
    Private Const logPath As String = "log.xml"
    Private WithEvents Notify As New Forms.NotifyIcon
    Private getFluxCancellationTokeSource As CancellationTokenSource
    Public Sub New()
        InitializeComponent()
        Me.Icon = Interop.Imaging.CreateBitmapSourceFromHIcon(My.Resources.Logo.Logo.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        Notify.Text = My.Resources.NotifyText
        Notify.Icon = My.Resources.Logo.Logo
        Notify.Visible = True
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
        Dim autoconnect As Boolean = False
        Try
            log = XDocument.Load(logPath)
            Model.Username = log.<user>.<name>.Value
            Model.Password = log.<user>.<password>.Value
            Dim state As NetState
            If [Enum].TryParse(log.<user>.<state>.Value, state) Then
                Model.State = state
            End If
            autoconnect = True
        Catch ex As Exception
            SetConnectableState()
            log =
                <?xml version="1.0" encoding="utf-8"?>
                <user>
                    <name></name>
                    <password></password>
                    <state><%= Model.State.ToString() %></state>
                </user>
        End Try
        GetFlux()
    End Sub
    Private Async Sub SetConnectableState()
        If Await NetHelperBase.CanConnect(Auth4Helper.HostName) Then
            Model.State = NetState.Auth4
        Else
            Model.State = NetState.Net
        End If
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
End Class
