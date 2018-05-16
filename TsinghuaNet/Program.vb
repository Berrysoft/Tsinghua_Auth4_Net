Imports Microsoft.VisualBasic.ApplicationServices

Module Program
    Private Const FileName As String = "event.log"
    Friend Log As LogWriter
    <STAThread>
    Sub Main(args As String())
        Try
            Log = New LogWriter(FileName)
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
            Return
        End Try
        Using Log
            Try
                WriteEvent("程序启动")
                Dim manager As New SingleInstanceManager()
                manager.Run(args)
                WriteEvent("程序结束")
            Catch ex As Exception
                WriteException(ex)
            End Try
        End Using
    End Sub
    Async Sub WriteLog(message As String)
        Await Log.WriteLogAsync(message)
    End Sub
    Async Sub WriteException(ex As Exception)
#If DEBUG Then
        Await Log.WriteFullExceptionAsync(ex)
#Else
        Await Log.WriteExceptionAsync(ex)
#End If
    End Sub
    Async Sub WriteEvent(name As String)
        Await Log.WriteEventAsync(name)
    End Sub
#If Debug Then
    Async Sub WriteDebug(message As String)
        Await Log.WriteDebugAsync(message)
    End Sub
#End If
End Module

Class SingleInstanceManager
    Inherits WindowsFormsApplicationBase
    Private app As Application
    Public Sub New()
        Me.IsSingleInstance = True
    End Sub
    Protected Overrides Function OnStartup(eventArgs As StartupEventArgs) As Boolean
        app = New Application()
        app.Run()
        Return False
    End Function
    Protected Overrides Sub OnStartupNextInstance(eventArgs As StartupNextInstanceEventArgs)
        MyBase.OnStartupNextInstance(eventArgs)
        app.Activate()
    End Sub
End Class