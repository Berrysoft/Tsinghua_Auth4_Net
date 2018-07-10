Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Net.Http
Imports Berrysoft.Tsinghua.Net

Class MainViewModel
    Inherits DependencyObject

    Public Shared ReadOnly FlowDirectionProperty As DependencyProperty = DependencyProperty.Register(NameOf(FlowDirection), GetType(FlowDirection), GetType(MainViewModel))
    Public Property FlowDirection As FlowDirection
        Get
            Return GetValue(FlowDirectionProperty)
        End Get
        Set(value As FlowDirection)
            SetValue(FlowDirectionProperty, value)
        End Set
    End Property

    Public Shared ReadOnly UsernameProperty As DependencyProperty = DependencyProperty.Register(NameOf(Username), GetType(String), GetType(MainViewModel), New PropertyMetadata(String.Empty))
    Public Property Username As String
        Get
            Return GetValue(UsernameProperty)
        End Get
        Set(value As String)
            SetValue(UsernameProperty, value)
        End Set
    End Property

    Public Shared ReadOnly PasswordProperty As DependencyProperty = DependencyProperty.Register(NameOf(Password), GetType(String), GetType(MainViewModel), New PropertyMetadata(String.Empty))
    Public Property Password As String
        Get
            Return GetValue(PasswordProperty)
        End Get
        Set(value As String)
            SetValue(PasswordProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LoggedUsernameProperty As DependencyProperty = DependencyProperty.Register(NameOf(LoggedUsername), GetType(String), GetType(MainViewModel))
    Public Property LoggedUsername As String
        Get
            Return GetValue(LoggedUsernameProperty)
        End Get
        Set(value As String)
            SetValue(LoggedUsernameProperty, value)
        End Set
    End Property

    Public Shared ReadOnly FluxProperty As DependencyProperty = DependencyProperty.Register(NameOf(Flux), GetType(Long?), GetType(MainViewModel))
    Public Property Flux As Long?
        Get
            Return GetValue(FluxProperty)
        End Get
        Set(value As Long?)
            SetValue(FluxProperty, value)
        End Set
    End Property

    Public Shared ReadOnly OnlineTimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(OnlineTime), GetType(TimeSpan?), GetType(MainViewModel))
    Public Property OnlineTime As TimeSpan?
        Get
            Return GetValue(OnlineTimeProperty)
        End Get
        Set(value As TimeSpan?)
            SetValue(OnlineTimeProperty, value)
        End Set
    End Property

    Public Shared ReadOnly BalanceProperty As DependencyProperty = DependencyProperty.Register(NameOf(Balance), GetType(Decimal?), GetType(MainViewModel))
    Public Property Balance As Decimal?
        Get
            Return GetValue(BalanceProperty)
        End Get
        Set(value As Decimal?)
            SetValue(BalanceProperty, value)
        End Set
    End Property

    Public Event StateChanged As EventHandler(Of NetState)
    Public Shared ReadOnly StateProperty As DependencyProperty = DependencyProperty.Register(NameOf(State), GetType(NetState), GetType(MainViewModel), New PropertyMetadata(NetState.Unknown, AddressOf StatePropertyChangedCallback))
    Public Property State As NetState
        Get
            Return GetValue(StateProperty)
        End Get
        Set(value As NetState)
            SetValue(StateProperty, value)
        End Set
    End Property
    Private Sub OnStateChanged(e As NetState)
        RaiseEvent StateChanged(Me, e)
    End Sub
    Private Shared Sub StatePropertyChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim model As MainViewModel = d
        model.OnStateChanged(e.NewValue)
    End Sub

    Private auth4 As Auth4Helper
    Private auth6 As Auth6Helper
    Private net As NetHelper
    Private client As New HttpClient()
    Private Sub UpdateHelper(helper As NetHelperBase)
        helper.Username = Username
        helper.Password = Password
    End Sub
    Public ReadOnly Property Helper As IConnect
        Get
            Select Case State
                Case NetState.Auth4
                    If auth4 Is Nothing Then
                        auth4 = New Auth4Helper(Username, Password, client)
                    Else
                        UpdateHelper(auth4)
                    End If
                    Return auth4
                Case NetState.Auth6
                    If auth6 Is Nothing Then
                        auth6 = New Auth6Helper(Username, Password, client)
                    Else
                        UpdateHelper(auth6)
                    End If
                    Return auth6
                Case NetState.Net
                    If net Is Nothing Then
                        net = New NetHelper(Username, Password, client)
                    Else
                        UpdateHelper(net)
                    End If
                    Return net
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property

    Public Shared ReadOnly LanguagesProperty As DependencyProperty = DependencyProperty.Register(NameOf(Languages), GetType(ObservableCollection(Of CultureInfo)), GetType(MainViewModel), New PropertyMetadata(New ObservableCollection(Of CultureInfo)))
    Public Property Languages As ObservableCollection(Of CultureInfo)
        Get
            Return GetValue(LanguagesProperty)
        End Get
        Set(value As ObservableCollection(Of CultureInfo))
            SetValue(LanguagesProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LanguagesSelectIndexProperty As DependencyProperty = DependencyProperty.Register(NameOf(LanguagesSelectIndex), GetType(Integer), GetType(MainViewModel))
    Public Property LanguagesSelectIndex As Integer
        Get
            Return GetValue(LanguagesSelectIndexProperty)
        End Get
        Set(value As Integer)
            SetValue(LanguagesSelectIndexProperty, value)
        End Set
    End Property

    Public Event ThemeChanged As EventHandler(Of Theme)
    Public Shared ReadOnly ThemeProperty As DependencyProperty = DependencyProperty.Register(NameOf(Theme), GetType(Theme), GetType(MainViewModel), New PropertyMetadata(CType(-1, Theme), AddressOf ThemeChangedCallback))
    Public Property Theme As Theme
        Get
            Return GetValue(ThemeProperty)
        End Get
        Set(value As Theme)
            SetValue(ThemeProperty, value)
        End Set
    End Property
    Private Sub OnThemeChanged(e As Theme)
        RaiseEvent ThemeChanged(Me, e)
    End Sub
    Private Shared Sub ThemeChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim model As MainViewModel = d
        model.OnThemeChanged(e.NewValue)
    End Sub

    Public Shared ReadOnly UsersProperty As DependencyProperty = DependencyProperty.Register(NameOf(Users), GetType(ObservableCollection(Of DependencyNetUser)), GetType(MainViewModel), New PropertyMetadata(New ObservableCollection(Of DependencyNetUser)))
    Public Property Users As ObservableCollection(Of DependencyNetUser)
        Get
            Return GetValue(UsersProperty)
        End Get
        Set(value As ObservableCollection(Of DependencyNetUser))
            SetValue(UsersProperty, value)
        End Set
    End Property

    Private usereg As UseregHelper
    Public ReadOnly Property UseregHelper As UseregHelper
        Get
            If usereg Is Nothing Then
                usereg = New UseregHelper(Username, Password, client)
            Else
                UpdateHelper(usereg)
            End If
            Return usereg
        End Get
    End Property

    Public Sub DisposeHelpers()
        client.Dispose()
    End Sub
End Class
