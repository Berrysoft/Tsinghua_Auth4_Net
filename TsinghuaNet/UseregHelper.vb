Imports System.Globalization
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions

Class UseregHelper
    Inherits NetHelperBase
    Private Const ConnectUrl = "https://usereg.tsinghua.edu.cn/do.php"
    Private Const InfoUrl = "https://usereg.tsinghua.edu.cn/online_user_ipv4.php"
    Private Const ConnectData = "action=login&user_login_name={0}&user_password={1}"
    Private Const LogoutData = "action=drop&user_ip={0}"
    Public Sub New(username As String, password As String)
        MyBase.New(username, password)
    End Sub
    Public Async Function ConnectAsync() As Task(Of String)
        Return (Await PostAsync(ConnectUrl, String.Format(ConnectData, Username, NetHelper.GetMD5(Password)))).ErrorMessage
    End Function
    Public Async Function GetUsersAsync() As Task(Of IEnumerable(Of NetUser))
        Dim userhtml = Await GetAsync(InfoUrl)
        If userhtml.ErrorMessage Is Nothing Then
            Dim info = Regex.Matches(userhtml.Response, "<tr align=""center"">.+?</tr>", RegexOptions.Singleline)
            Dim users = From r As Match In info
                        Let details = Regex.Matches(r.Value, "(?<=\<td class=""maintd""\>)(.*?)(?=\</td\>)")
                        Select New NetUser() With
                            {
                                .Address = IPAddress.Parse(details(0).Value),
                                .LoginTime = Date.ParseExact(details(1).Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                .Client = details(10).Value
                            }
            Return users
        End If
        Return Nothing
    End Function
    Public Async Function LogoutAsync(user As NetUser) As Task
        Await PostAsync(InfoUrl, String.Format(LogoutData, user.Address))
    End Function
End Class

Class NetUser
    Inherits DependencyObject

    Public Shared ReadOnly AddressProperty As DependencyProperty = DependencyProperty.Register(NameOf(Address), GetType(IPAddress), GetType(NetUser))
    Public Property Address As IPAddress
        Get
            Return GetValue(AddressProperty)
        End Get
        Set(value As IPAddress)
            SetValue(AddressProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LoginTimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(LoginTime), GetType(Date), GetType(NetUser))
    Public Property LoginTime As Date
        Get
            Return GetValue(LoginTimeProperty)
        End Get
        Set(value As Date)
            SetValue(LoginTimeProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ClientProperty As DependencyProperty = DependencyProperty.Register(NameOf(Client), GetType(String), GetType(NetUser))
    Public Property Client As String
        Get
            Return GetValue(ClientProperty)
        End Get
        Set(value As String)
            SetValue(ClientProperty, value)
        End Set
    End Property
End Class