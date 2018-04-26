Imports System.Globalization
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Text.RegularExpressions

Class UseregHelper
    Private Const ConnectUrl = "https://usereg.tsinghua.edu.cn/do.php"
    Private Const InfoUrl = "https://usereg.tsinghua.edu.cn/online_user_ipv4.php"
    Private Const ConnectData = "action=login&user_login_name={0}&user_password={1}"
    Private Const LogoutData = "action=logout"
    Public ReadOnly Property Username As String
    Public ReadOnly Property Password As String
    Public Sub New(username As String, password As String)
        Me.Username = username
        Me.Password = password
    End Sub
    Public Async Function GetUserList() As Task(Of IEnumerable(Of NetUser))
        Dim loginres = Await NetHelperBase.Post(ConnectUrl, String.Format(ConnectData, Username, Password))
        If loginres.ErrorMessage Is Nothing Then
            Dim userhtml = Await NetHelperBase.GetData(InfoUrl)
            If userhtml.ErrorMessage Is Nothing Then
                Dim info = Regex.Matches(userhtml.Response, "<tr align=""center"">.+?</tr>", RegexOptions.Singleline)
                Dim users = From r As Match In info
                            Let details = Regex.Matches(r.Value, "(?<=\<td class=""maintd""\>)(.*?)(?=\</td\>)")
                            Select New NetUser() With
                                {
                                    .Address = IPAddress.Parse(details(0).Value),
                                    .Mac = PhysicalAddress.Parse(details(6).Value),
                                    .LoginTime = Date.ParseExact(details(1).Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                    .Client = details(10).Value
                                }
                Return users
            End If
        End If
        Return Nothing
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

    Public Shared ReadOnly MacProperty As DependencyProperty = DependencyProperty.Register(NameOf(Mac), GetType(PhysicalAddress), GetType(NetUser))
    Public Property Mac As PhysicalAddress
        Get
            Return GetValue(MacProperty)
        End Get
        Set(value As PhysicalAddress)
            SetValue(MacProperty, value)
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