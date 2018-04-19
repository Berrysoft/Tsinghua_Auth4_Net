﻿Imports System.Globalization

Class MainViewModel
    Inherits DependencyObject

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

    Public ReadOnly Property Helper As NetHelperBase
        Get
            Select Case State
                Case NetState.Auth4
                    Return New Auth4Helper(Username, Password)
                Case NetState.Net
                    Return New NetHelper(Username, Password)
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
End Class

Enum NetState
    Auth4
    Net
End Enum

Class FluxToString
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim flux As Long? = value
        If flux.HasValue Then
            Dim f As Long = flux.Value
            If f < 1_000 Then
                Return $"{f}B"
            ElseIf f < 1_000_000 Then
                Return $"{(f / 1_000).ToString("N2")}KB"
            ElseIf f < 1_000_000_000 Then
                Return $"{(f / 1_000_000).ToString("N2")}MB"
            Else
                Return $"{(f / 1_000_000_000).ToString("N2")}GB"
            End If
        Else
            Return String.Empty
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class