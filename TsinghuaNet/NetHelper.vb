Imports System.Security.Cryptography
Imports System.Text

Class NetHelper
    Inherits NetHelperBase
    Implements IConnect
    Private Const Host = "net.tsinghua.edu.cn"
    Private Const ConnectUrl = "https://net.tsinghua.edu.cn/do_login.php"
    Private Const FluxUrl = "https://net.tsinghua.edu.cn/rad_user_info.php"
    Private Const ConnectData = "action=login&username={0}&password={1}&ac_id=1"
    Private Const LogOutData = "action=logout"

    Public Sub New(username As String, password As String)
        MyBase.New(username, password)
    End Sub

    Public Shared ReadOnly Property HostName As String
        Get
            Return Host
        End Get
    End Property

    Public Async Function ConnectAsync() As Task(Of String) Implements IConnect.ConnectAsync
        Return (Await PostAsync(ConnectUrl, String.Format(ConnectData, Username, "{MD5_HEX}" & GetMD5(Password)))).ErrorMessage
    End Function

    Public Async Function LogOutAsync() As Task(Of String) Implements IConnect.LogOutAsync
        Return (Await PostAsync(ConnectUrl, LogOutData)).ErrorMessage
    End Function

    Public Async Function GetFluxAsync() As Task(Of (Response As String, ErrorMessage As String)) Implements IConnect.GetFluxAsync
        Return Await PostAsync(FluxUrl, Nothing)
    End Function

    Friend Shared Function GetMD5(input As String) As String
        Dim result As String
        Using md5Hash As MD5 = MD5.Create()
            Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input))
            Dim sBuilder As New StringBuilder()
            Dim i As Integer
            For i = 0 To data.Length - 1
                sBuilder.Append(data(i).ToString("x2"))
            Next i
            result = sBuilder.ToString()
        End Using
        Return result
    End Function
End Class
