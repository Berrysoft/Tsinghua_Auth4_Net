Imports System.Collections.ObjectModel
Imports System.Globalization

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
    Public Shared ReadOnly StateProperty As DependencyProperty = DependencyProperty.Register(NameOf(State), GetType(NetState), GetType(MainViewModel), New PropertyMetadata(NetState.Net, AddressOf StatePropertyChangedCallback))
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

    Public ReadOnly Property Helper As IConnect
        Get
            Select Case State
                Case NetState.Auth4
                    Return New Auth4Helper(Username, Password)
                Case NetState.Auth6
                    Return New Auth6Helper(Username, Password)
                Case NetState.Net
                    Return New NetHelper(Username, Password)
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

    Public Shared ReadOnly UsersProperty As DependencyProperty = DependencyProperty.Register(NameOf(Users), GetType(ObservableCollection(Of NetUser)), GetType(MainViewModel), New PropertyMetadata(New ObservableCollection(Of NetUser)))
    Public Property Users As ObservableCollection(Of NetUser)
        Get
            Return GetValue(UsersProperty)
        End Get
        Set(value As ObservableCollection(Of NetUser))
            SetValue(UsersProperty, value)
        End Set
    End Property

    Public ReadOnly Property UseregHelper As UseregHelper
        Get
            Return New UseregHelper(Username, Password)
        End Get
    End Property
End Class

Enum NetState
    Auth4
    Auth6
    Net
End Enum
