Imports System.Security.Cryptography
Imports System.Text

Class NetHelper
    Inherits NetHelperBase
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

    Public Overrides Async Function Connect() As Task(Of String)
        Return (Await Post(ConnectUrl, String.Format(ConnectData, Username, "{MD5_HEX}" & GetMD5(Password)))).ErrorMessage
    End Function

    Public Overrides Async Function LogOut() As Task(Of String)
        Return (Await Post(ConnectUrl, LogOutData)).ErrorMessage
    End Function

    Public Overrides Async Function GetFlux() As Task(Of (Response As String, ErrorMessage As String))
        Return Await Post(FluxUrl, Nothing)
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
