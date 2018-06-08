Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Ipc
Imports System.Runtime.Serialization.Formatters
Imports System.Threading
Imports System.Windows.Threading

Interface ISingleInstanceApp
    Function SignalExternalCommandLineArgs(args As IList(Of String)) As Boolean
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

    Function InitializeAsFirstInstance(Of TApplication As {Application, ISingleInstanceApp})(uniqueName As String) As Boolean
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

    Private Function GetCommandLineArgs(uniqueApplicationName As String) As IList(Of String)
        Dim args As String() = Environment.GetCommandLineArgs()

        Return New List(Of String)(If(args, New String() {}))
    End Function

    Private Sub CreateRemoteService(Of TApplication As {Application, ISingleInstanceApp})(channelName As String)
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

    Private Sub SignalFirstInstance(Of TApplication As {Application, ISingleInstanceApp})(channelName As String, args As IList(Of String))
        Dim secondInstanceChannel As New IpcClientChannel()
        ChannelServices.RegisterChannel(secondInstanceChannel, True)
        Dim remotingServiceUrl As String = IpcProtocol & channelName & "/" & RemoteServiceName
        Dim firstInstanceRemoteServiceReference As IPCRemoteService(Of TApplication) = RemotingServices.Connect(GetType(IPCRemoteService(Of TApplication)), remotingServiceUrl)

        firstInstanceRemoteServiceReference?.InvokeFirstInstance(args)
    End Sub

    Private Function ActivateFirstInstanceCallback(Of TApplication As {Application, ISingleInstanceApp})(arg As Object) As Object
        Dim args As IList(Of String) = TryCast(arg, IList(Of String))
        ActivateFirstInstance(Of TApplication)(args)
        Return Nothing
    End Function

    Private Sub ActivateFirstInstance(Of TApplication As {Application, ISingleInstanceApp})(args As IList(Of String))
        If Application.Current Is Nothing Then
            Return
        End If

        CType(Application.Current, TApplication).SignalExternalCommandLineArgs(args)
    End Sub

    Private Class IPCRemoteService(Of TApplication As {Application, ISingleInstanceApp})
        Inherits MarshalByRefObject

        Public Sub InvokeFirstInstance(args As IList(Of String))
            If Application.Current IsNot Nothing Then
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, New DispatcherOperationCallback(AddressOf SingleInstance.ActivateFirstInstanceCallback(Of TApplication)), args)
            End If
        End Sub

        Public Overrides Function InitializeLifetimeService() As Object
            Return Nothing
        End Function
    End Class
End Module
