Imports System.Globalization
Imports System.Net.NetworkInformation
Imports System.Text
Imports Berrysoft.Console

<Settings("user")>
Class Settings
    Inherits XmlSettings
    <Settings("name")>
    Public Property Username As String
    <Settings("password", ConverterType:=GetType(PasswordConverter))>
    Public Property Password As String
    Class PasswordConverter
        Implements ISimpleConverter
        Public Function Convert(value As Object) As Object Implements ISimpleConverter.Convert
            Return Encoding.ASCII.GetString(System.Convert.FromBase64String(If(value, String.Empty)))
        End Function
        Public Function ConvertBack(value As Object) As Object Implements ISimpleConverter.ConvertBack
            Return System.Convert.ToBase64String(Encoding.ASCII.GetBytes(value))
        End Function
    End Class
    <Settings("state", ConverterType:=GetType(StateConverter))>
    Public Property State As NetState
    Class StateConverter
        Implements ISimpleConverter
        Public Function Convert(value As Object) As Object Implements ISimpleConverter.Convert
            Dim temp As NetState
            If [Enum].TryParse(value, temp) Then
                Return temp
            Else
                Return NetState.Unknown
            End If
        End Function
        Public Function ConvertBack(value As Object) As Object Implements ISimpleConverter.ConvertBack
            Return value.ToString()
        End Function
    End Class
    <Settings("more", ConverterType:=GetType(BoolConverter))>
    Public Property MoreInf As Boolean
    Class BoolConverter
        Implements ISimpleConverter
        Public Function Convert(value As Object) As Object Implements ISimpleConverter.Convert
            Dim moreinfResult As Boolean
            If Boolean.TryParse(value, moreinfResult) Then
                Return moreinfResult
            Else
                Return False
            End If
        End Function
        Public Function ConvertBack(value As Object) As Object Implements ISimpleConverter.ConvertBack
            Return value.ToString()
        End Function
    End Class
    <Settings("language", ConverterType:=GetType(LanguageConverter))>
    Public Property Language As CultureInfo
    Class LanguageConverter
        Implements ISimpleConverter
        Public Function Convert(value As Object) As Object Implements ISimpleConverter.Convert
            Try
                Return If(value Is Nothing, Nothing, New CultureInfo(value.ToString()))
            Catch ex As Exception
                WriteException(ex)
                Return Nothing
            End Try
        End Function
        Public Function ConvertBack(value As Object) As Object Implements ISimpleConverter.ConvertBack
            Return CType(value, CultureInfo)?.Name
        End Function
    End Class
    Private Const logPath As String = "log.xml"
    Public Sub New()
        Username = String.Empty
        Password = String.Empty
        OpenAsync()
    End Sub
    Private Async Sub OpenAsync()
        Try
            Open(logPath)
        Catch ex As Exception
            WriteException(ex)
        End Try
        If State = NetState.Unknown Then
            State = Await GetConnectableStateAsync()
        End If
    End Sub
    Private Async Function GetConnectableStateAsync() As Task(Of NetState)
        Dim ping As New Ping()
        Dim response As PingReply = Await ping.SendPingAsync("auth4.tsinghua.edu.cn")
        If response.Status = IPStatus.Success Then
            Return NetState.Auth4
        Else
            Return NetState.Net
        End If
    End Function
    Public Overloads Sub Save()
        Save(logPath)
    End Sub
End Class
