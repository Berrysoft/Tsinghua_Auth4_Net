Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices

Module Program
    Friend Log As LogWriter
    Public ReadOnly DefaultCultureInfo As New CultureInfo("zh-Hans")
    <STAThread>
    Public Function Main(args As String()) As Integer
        Using Log
            Try
                Dim manager As New SingleInstanceManager()
                manager.Run(args)
                WriteEvent("程序结束")
            Catch ex As Exception
                WriteException(ex)
                ShowError(ex.ToString())
                Return 1
            End Try
        End Using
        Return 0
    End Function
    Public Async Sub WriteLog(message As String)
        Await Log.WriteLogAsync(message)
    End Sub
    Public Async Sub WriteException(ex As Exception)
#If DEBUG Then
        Await Log.WriteFullExceptionAsync(ex)
#Else
        Await Log.WriteExceptionAsync(ex)
#End If
    End Sub
    Public Async Sub WriteEvent(name As String)
        Await Log.WriteEventAsync(name)
    End Sub
#If Debug Then
    Public Async Sub WriteDebug(message As String)
        Await Log.WriteDebugAsync(message)
    End Sub
#End If
    Friend Sub ShowError(message As String)
        MessageBox.Show(message, My.Resources.Title, MessageBoxButton.OK, MessageBoxImage.Error)
    End Sub
End Module

Class SingleInstanceManager
    Inherits WindowsFormsApplicationBase
    Private Const FileName As String = "event.log"
    Private app As Application
    Public Sub New()
        Me.IsSingleInstance = True
    End Sub
    Protected Overrides Function OnStartup(eventArgs As StartupEventArgs) As Boolean
        Try
            Dim eventLog As New FileInfo(FileName)
            If eventLog.Length > Integer.MaxValue Then
                eventLog.Delete()
            End If
            Program.Log = New LogWriter(FileName, True)
        Catch ex As Exception
            ShowError(ex.ToString())
            Return False
        End Try
        app = New Application()
        WriteLog($"程序启动: {Assembly.GetExecutingAssembly().GetName().Version}")
        app.Run()
        Return False
    End Function
    Protected Overrides Sub OnStartupNextInstance(eventArgs As StartupNextInstanceEventArgs)
        MyBase.OnStartupNextInstance(eventArgs)
        app.Activate()
    End Sub
End Class