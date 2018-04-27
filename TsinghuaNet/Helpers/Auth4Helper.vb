Namespace Helpers
    Public Class Auth4Helper
        Inherits NetHelperBase
        Implements IConnect
        Private Const Host = "auth4.tsinghua.edu.cn"
        Private Const ConnectUrl = "https://auth4.tsinghua.edu.cn/srun_portal_pc.php"
        Private Const LogOutUrl = "https://auth4.tsinghua.edu.cn/cgi-bin/srun_portal"
        Private Const FluxUrl = "https://auth4.tsinghua.edu.cn/rad_user_info.php"
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

        Public Async Function ConnectAsync() As Task(Of String) Implements IConnect.ConnectAsync
            Return (Await PostAsync(ConnectUrl, String.Format(ConnectData, Username, Password))).ErrorMessage
        End Function

        Public Async Function LogOutAsync() As Task(Of String) Implements IConnect.LogOutAsync
            Return (Await PostAsync(LogOutUrl, LogOutData)).ErrorMessage
        End Function

        Public Async Function GetFluxAsync() As Task(Of (Response As String, ErrorMessage As String)) Implements IConnect.GetFluxAsync
            Return Await PostAsync(FluxUrl, Nothing)
        End Function
    End Class
End Namespace
