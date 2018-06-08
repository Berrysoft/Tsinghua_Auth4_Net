Imports System.IO
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Ipc
Imports System.Runtime.Serialization.Formatters
Imports System.Threading
Imports System.Windows.Threading
Imports System.Security
Imports System.Runtime.InteropServices
Imports System.ComponentModel

Friend Enum WM
    NULL = &H0
    CREATE = &H1
    DESTROY = &H2
    MOVE = &H3
    SIZE = &H5
    ACTIVATE = &H6
    SETFOCUS = &H7
    KILLFOCUS = &H8
    ENABLE = &HA
    SETREDRAW = &HB
    SETTEXT = &HC
    GETTEXT = &HD
    GETTEXTLENGTH = &HE
    PAINT = &HF
    CLOSE = &H10
    QUERYENDSESSION = &H11
    QUIT = &H12
    QUERYOPEN = &H13
    ERASEBKGND = &H14
    SYSCOLORCHANGE = &H15
    SHOWWINDOW = &H18
    ACTIVATEAPP = &H1C
    SETCURSOR = &H20
    MOUSEACTIVATE = &H21
    CHILDACTIVATE = &H22
    QUEUESYNC = &H23
    GETMINMAXINFO = &H24
    WINDOWPOSCHANGING = &H46
    WINDOWPOSCHANGED = &H47
    CONTEXTMENU = &H7B
    STYLECHANGING = &H7C
    STYLECHANGED = &H7D
    DISPLAYCHANGE = &H7E
    GETICON = &H7F
    SETICON = &H80
    NCCREATE = &H81
    NCDESTROY = &H82
    NCCALCSIZE = &H83
    NCHITTEST = &H84
    NCPAINT = &H85
    NCACTIVATE = &H86
    GETDLGCODE = &H87
    SYNCPAINT = &H88
    NCMOUSEMOVE = &HA0
    NCLBUTTONDOWN = &HA1
    NCLBUTTONUP = &HA2
    NCLBUTTONDBLCLK = &HA3
    NCRBUTTONDOWN = &HA4
    NCRBUTTONUP = &HA5
    NCRBUTTONDBLCLK = &HA6
    NCMBUTTONDOWN = &HA7
    NCMBUTTONUP = &HA8
    NCMBUTTONDBLCLK = &HA9
    SYSKEYDOWN = &H104
    SYSKEYUP = &H105
    SYSCHAR = &H106
    SYSDEADCHAR = &H107
    COMMAND = &H111
    SYSCOMMAND = &H112
    MOUSEMOVE = &H200
    LBUTTONDOWN = &H201
    LBUTTONUP = &H202
    LBUTTONDBLCLK = &H203
    RBUTTONDOWN = &H204
    RBUTTONUP = &H205
    RBUTTONDBLCLK = &H206
    MBUTTONDOWN = &H207
    MBUTTONUP = &H208
    MBUTTONDBLCLK = &H209
    MOUSEWHEEL = &H20A
    XBUTTONDOWN = &H20B
    XBUTTONUP = &H20C
    XBUTTONDBLCLK = &H20D
    MOUSEHWHEEL = &H20E
    CAPTURECHANGED = &H215
    ENTERSIZEMOVE = &H231
    EXITSIZEMOVE = &H232
    IME_SETCONTEXT = &H281
    IME_NOTIFY = &H282
    IME_CONTROL = &H283
    IME_COMPOSITIONFULL = &H284
    IME_SELECT = &H285
    IME_CHAR = &H286
    IME_REQUEST = &H288
    IME_KEYDOWN = &H290
    IME_KEYUP = &H291
    NCMOUSELEAVE = &H2A2
    DWMCOMPOSITIONCHANGED = &H31E
    DWMNCRENDERINGCHANGED = &H31F
    DWMCOLORIZATIONCOLORCHANGED = &H320
    DWMWINDOWMAXIMIZEDCHANGE = &H321
    DWMSENDICONICTHUMBNAIL = &H323
    DWMSENDICONICLIVEPREVIEWBITMAP = &H326
    USER = &H400
    TRAYMOUSEMESSAGE = &H800
    APP = &H8000
End Enum

<SuppressUnmanagedCodeSecurity>
Friend Module NativeMethods
    Public Delegate Function MessageHandler(ByVal uMsg As WM, ByVal wParam As IntPtr, ByVal lParam As IntPtr, <Out> ByRef handled As Boolean) As IntPtr
    <DllImport("shell32.dll", EntryPoint:="CommandLineToArgvW", CharSet:=CharSet.Unicode)>
    Private Function _CommandLineToArgvW(<MarshalAs(UnmanagedType.LPWStr)> ByVal cmdLine As String, <Out> ByRef numArgs As Integer) As IntPtr
    End Function
    <DllImport("kernel32.dll", EntryPoint:="LocalFree", SetLastError:=True)>
    Private Function _LocalFree(ByVal hMem As IntPtr) As IntPtr
    End Function

    Function CommandLineToArgvW(ByVal cmdLine As String) As String()
        Dim argv As IntPtr = IntPtr.Zero

        Try
            Dim numArgs As Integer = 0
            argv = _CommandLineToArgvW(cmdLine, numArgs)

            If argv = IntPtr.Zero Then
                Throw New Win32Exception()
            End If

            Dim result = New String(numArgs - 1) {}

            For i As Integer = 0 To numArgs - 1
                Dim currArg As IntPtr = Marshal.ReadIntPtr(argv, i * Marshal.SizeOf(GetType(IntPtr)))
                result(i) = Marshal.PtrToStringUni(currArg)
            Next

            Return result
        Finally
            Dim p As IntPtr = _LocalFree(argv)
        End Try
    End Function
End Module

Interface ISingleInstanceApp
    Function SignalExternalCommandLineArgs(ByVal args As IList(Of String)) As Boolean
End Interface

Module SingleInstance
    Private Const Delimiter As String = ":"
    Private Const ChannelNameSuffix As String = "SingeInstanceIPCChannel"
    Private Const RemoteServiceName As String = "SingleInstanceApplicationService"
    Private Const IpcProtocol As String = "ipc://"
    Private singleInstanceMutex As Mutex
    Private channel As IpcServerChannel
    Private _commandLineArgs As IList(Of String)

    Public ReadOnly Property CommandLineArgs As IList(Of String)
        Get
            Return _commandLineArgs
        End Get
    End Property

    Function InitializeAsFirstInstance(Of TApplication As {Application, ISingleInstanceApp})(ByVal uniqueName As String) As Boolean
        _commandLineArgs = GetCommandLineArgs(uniqueName)
        Dim applicationIdentifier As String = uniqueName & Environment.UserName
        Dim channelName As String = String.Concat(applicationIdentifier, Delimiter, ChannelNameSuffix)
        Dim firstInstance As Boolean
        singleInstanceMutex = New Mutex(True, applicationIdentifier, firstInstance)

        If firstInstance Then
            CreateRemoteService(Of TApplication)(channelName)
        Else
            SignalFirstInstance(Of TApplication)(channelName, CommandLineArgs)
        End If

        Return firstInstance
    End Function

    Sub Cleanup()
        If singleInstanceMutex IsNot Nothing Then
            singleInstanceMutex.Close()
            singleInstanceMutex = Nothing
        End If

        If channel IsNot Nothing Then
            ChannelServices.UnregisterChannel(channel)
            channel = Nothing
        End If
    End Sub

    Private Function GetCommandLineArgs(ByVal uniqueApplicationName As String) As IList(Of String)
        Dim args As String() = Nothing

        If AppDomain.CurrentDomain.ActivationContext Is Nothing Then
            args = Environment.GetCommandLineArgs()
        Else
            Dim appFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), uniqueApplicationName)
            Dim cmdLinePath As String = Path.Combine(appFolderPath, "cmdline.txt")

            If File.Exists(cmdLinePath) Then

                Try

                    Using reader As TextReader = New StreamReader(cmdLinePath, System.Text.Encoding.Unicode)
                        args = NativeMethods.CommandLineToArgvW(reader.ReadToEnd())
                    End Using

                    File.Delete(cmdLinePath)
                Catch __unusedIOException1__ As IOException
                End Try
            End If
        End If

        If args Is Nothing Then
            args = New String() {}
        End If

        Return New List(Of String)(args)
    End Function

    Private Sub CreateRemoteService(Of TApplication As {Application, ISingleInstanceApp})(ByVal channelName As String)
        Dim serverProvider As BinaryServerFormatterSinkProvider = New BinaryServerFormatterSinkProvider()
        serverProvider.TypeFilterLevel = TypeFilterLevel.Full
        Dim props As IDictionary = New Dictionary(Of String, String)()
        props("name") = channelName
        props("portName") = channelName
        props("exclusiveAddressUse") = "false"
        channel = New IpcServerChannel(props, serverProvider)
        ChannelServices.RegisterChannel(channel, True)
        Dim remoteService As New IPCRemoteService(Of TApplication)()
        RemotingServices.Marshal(remoteService, RemoteServiceName)
    End Sub

    Private Sub SignalFirstInstance(Of TApplication As {Application, ISingleInstanceApp})(ByVal channelName As String, ByVal args As IList(Of String))
        Dim secondInstanceChannel As IpcClientChannel = New IpcClientChannel()
        ChannelServices.RegisterChannel(secondInstanceChannel, True)
        Dim remotingServiceUrl As String = IpcProtocol & channelName & "/" & RemoteServiceName
        Dim firstInstanceRemoteServiceReference As IPCRemoteService(Of TApplication) = CType(RemotingServices.Connect(GetType(IPCRemoteService(Of TApplication)), remotingServiceUrl), IPCRemoteService(Of TApplication))

        If firstInstanceRemoteServiceReference IsNot Nothing Then
            firstInstanceRemoteServiceReference.InvokeFirstInstance(args)
        End If
    End Sub

    Private Function ActivateFirstInstanceCallback(Of TApplication As {Application, ISingleInstanceApp})(ByVal arg As Object) As Object
        Dim args As IList(Of String) = TryCast(arg, IList(Of String))
        ActivateFirstInstance(Of TApplication)(args)
        Return Nothing
    End Function

    Private Sub ActivateFirstInstance(Of TApplication As {Application, ISingleInstanceApp})(ByVal args As IList(Of String))
        If Application.Current Is Nothing Then
            Return
        End If

        CType(Application.Current, TApplication).SignalExternalCommandLineArgs(args)
    End Sub

    Private Class IPCRemoteService(Of TApplication As {Application, ISingleInstanceApp})
        Inherits MarshalByRefObject

        Public Sub InvokeFirstInstance(ByVal args As IList(Of String))
            If Application.Current IsNot Nothing Then
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, New DispatcherOperationCallback(AddressOf SingleInstance.ActivateFirstInstanceCallback(Of TApplication)), args)
            End If
        End Sub

        Public Overrides Function InitializeLifetimeService() As Object
            Return Nothing
        End Function
    End Class
End Module
