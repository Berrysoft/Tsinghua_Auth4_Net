Class Auth4Helper
    Inherits NetHelperBase
    Private Const Host = "auth4.tsinghua.edu.cn"
    Private Const ConnectUrl = "http://auth4.tsinghua.edu.cn/srun_portal_pc.php?ac_id=1&"
    Private Const LogOutUrl = "http://auth4.tsinghua.edu.cn/include/auth_action.php"
    Private Const FluxUrl = "http://auth4.tsinghua.edu.cn/rad_user_info.php"
    Private Const ConnectData = "action=login&ac_id=1&user_ip=&nas_ip=&user_mac=&url=&username={0}&password={1}&save_me=0"
    Private Const LogOutData = "action=logout&username={0}&password={1}&ajax=1"

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
        Return Post(LogOutUrl, String.Format(LogOutData, Username, Password)).ErrorMessage
    End Function

    Public Overrides Function GetFlux() As (Response As String, ErrorMessage As String)
        Return Post(FluxUrl, Nothing)
    End Function
End Class
