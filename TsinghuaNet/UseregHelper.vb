Imports System.Globalization
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions

Class UseregHelper
    Private Const ConnectUrl = "https://usereg.tsinghua.edu.cn/do.php"
    Private Const InfoUrl = "https://usereg.tsinghua.edu.cn/online_user_ipv4.php"
    Private Const ConnectData = "action=login&user_login_name={0}&user_password={1}"
    Private Const LogoutData = "action=drop&user_ip={0}"
    Public ReadOnly Property Username As String
    Public ReadOnly Property Password As String
    Public Sub New(username As String, password As String)
        Me.Username = username
        Me.Password = password
    End Sub
    Public Async Function GetUserList() As Task(Of IEnumerable(Of NetUser))
        Dim userhtml = Await GetData(InfoUrl, ConnectUrl, String.Format(ConnectData, Username, NetHelper.GetMD5(Password)))
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
    Public Async Function LogoutUser(user As NetUser) As Task
        Await PostData(InfoUrl, String.Format(LogoutData, user.Address), ConnectUrl, String.Format(ConnectData, Username, NetHelper.GetMD5(Password)))
    End Function
    Public Shared Async Function PostData(url As String, data As String, connect As String, connectData As String) As Task(Of (Response As String, ErrorMessage As String))
        Try
            Using client As New HttpClient()
                Using content As New StringContent(If(connectData, String.Empty), Encoding.ASCII, "application/x-www-form-urlencoded")
                    Using response As HttpResponseMessage = Await client.PostAsync(connect, content)
                        Await response.Content.ReadAsStringAsync()
                    End Using
                End Using
                Using content As New StringContent(If(data, String.Empty), Encoding.ASCII, "application/x-www-form-urlencoded")
                    Using response As HttpResponseMessage = Await client.PostAsync(url, content)
                        Dim res As String = Await response.Content.ReadAsStringAsync()
                        Return (res, Nothing)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
    End Function
    Public Shared Async Function GetData(url As String, connect As String, data As String) As Task(Of (Response As String, ErrorMessage As String))
        Try
            Using client As New HttpClient()
                Using content As New StringContent(If(data, String.Empty), Encoding.ASCII, "application/x-www-form-urlencoded")
                    Using response As HttpResponseMessage = Await client.PostAsync(connect, content)
                        Await response.Content.ReadAsStringAsync()
                        Dim res As String = Await client.GetStringAsync(url)
                        Return (res, Nothing)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return (Nothing, ex.Message)
        End Try
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