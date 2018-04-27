Namespace Helpers
    Public Class NetHelper
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
    End Class
End Namespace
