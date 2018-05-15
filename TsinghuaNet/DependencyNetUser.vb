Imports System.Net
Imports Berrysoft.Tsinghua.Net

Public Class DependencyNetUser
    Inherits DependencyObject

    Public Shared Function Create(user As NetUser) As DependencyNetUser
        Return New DependencyNetUser() With {.Address = user.Address, .Client = user.Client, .LoginTime = user.LoginTime}
    End Function

    Public Shared ReadOnly AddressProperty As DependencyProperty = DependencyProperty.Register(NameOf(Address), GetType(IPAddress), GetType(DependencyNetUser))
    Public Property Address As IPAddress
        Get
            Return GetValue(AddressProperty)
        End Get
        Set(value As IPAddress)
            SetValue(AddressProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LoginTimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(LoginTime), GetType(Date), GetType(DependencyNetUser))
    Public Property LoginTime As Date
        Get
            Return GetValue(LoginTimeProperty)
        End Get
        Set(value As Date)
            SetValue(LoginTimeProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ClientProperty As DependencyProperty = DependencyProperty.Register(NameOf(Client), GetType(String), GetType(DependencyNetUser))
    Public Property Client As String
        Get
            Return GetValue(ClientProperty)
        End Get
        Set(value As String)
            SetValue(ClientProperty, value)
        End Set
    End Property
End Class
