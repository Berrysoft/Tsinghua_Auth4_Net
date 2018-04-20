Class Auth6Helper
    Inherits NetHelperBase
    Private Const Host = "auth6.tsinghua.edu.cn"
    Private Const ConnectUrl = "http://auth6.tsinghua.edu.cn/cgi-bin/srun_portal"
    Private Const FluxUrl = "http://auth6.tsinghua.edu.cn/rad_user_info.php"
    Private Const ConnectData = "action=login&ac_id=1&user_ip=&nas_ip=&user_mac=&url=&username={0}&password={1}&save_me=0"
    Private Const LogOutData = "action=logout&ac_id=1&ip=&double_stack=1"

    Public Sub New(username As String, password As String)
        MyBase.New(username, password)
    End Sub

    Public Shared ReadOnly Property HostName As String
        Get
            Return Host
        End Get
    End Property

    Public Overrides Async Function Connect() As Task(Of String)
        Return (Await Post(ConnectUrl, String.Format(ConnectData, Username, "{MD5_HEX}" & Password))).ErrorMessage
    End Function

    Public Overrides Async Function LogOut() As Task(Of String)
        Return (Await Post(ConnectUrl, LogOutData)).ErrorMessage
    End Function

    Public Overrides Async Function GetFlux() As Task(Of (Response As String, ErrorMessage As String))
        Return Await Post(FluxUrl, Nothing)
    End Function
End Class
