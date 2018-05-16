Imports Microsoft.VisualBasic.ApplicationServices

Imports System.IO

Module Program
    Private Const FileName As String = "event.log"
    Friend Log As LogWriter
    <STAThread>
    Sub Main(args As String())
        Try
            Dim eventLog As New FileInfo(FileName)
            If eventLog.Length > Integer.MaxValue Then
                eventLog.Delete()
            End If
            eventLog = Nothing
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
    Sub WriteLog(message As String)
        Log.WriteLog(message)
    End Sub
    Sub WriteException(ex As Exception)
#If DEBUG Then
        Log.WriteFullException(ex)
#Else
        Log.WriteException(ex)
#End If
    End Sub
    Sub WriteEvent(name As String)
        Log.WriteEvent(name)
    End Sub
#If Debug Then
    Sub WriteDebug(message As String)
        Log.WriteDebug(message)
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