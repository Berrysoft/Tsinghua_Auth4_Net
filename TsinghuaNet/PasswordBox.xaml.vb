Public Class PasswordBox
    Public Sub New()
        InitializeComponent()
        'Me.DataContext = Me
    End Sub
    Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(PasswordBox), New PropertyMetadata(Nothing, AddressOf TextPropertyChangedCallback))
    Public Property Text As String
        Get
            Return GetValue(TextProperty)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property
    Private Shared Sub TextPropertyChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim box As PasswordBox = d
        If box.Password.Password <> CStr(e.NewValue) Then
            box.Password.Password = e.NewValue
        End If
    End Sub
    Private Sub Password_PasswordChanged(sender As Object, e As RoutedEventArgs) Handles Password.PasswordChanged
        Me.Text = Password.Password
    End Sub
End Class
