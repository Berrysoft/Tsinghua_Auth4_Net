Imports System.Net

Class NetHelper
    Inherits NetHelperBase
    Private Const Host = "net.tsinghua.edu.cn"
    Private Const ConnectUrl = "http://net.tsinghua.edu.cn/do_login.php"
    Private Const FluxUrl = "http://net.tsinghua.edu.cn/rad_user_info.php"
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

    Public Overrides Function Connect() As String
        Return Post(ConnectUrl, String.Format(ConnectData, Username, Password)).ErrorMessage
    End Function

    Public Overrides Function LogOut() As String
        Return Post(ConnectUrl, LogOutData).ErrorMessage
    End Function

    Public Overrides Function GetFlux() As (Response As String, ErrorMessage As String)
        Return Post(FluxUrl, Nothing)
    End Function
End Class
