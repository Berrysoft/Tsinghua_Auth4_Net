Imports System.Globalization
Imports System.IO
Imports System.Reflection

Module Program
    Private Const FileName As String = "event.log"
    Friend Log As LogWriter
    Private ReadOnly Lock As New Object()
    Public ReadOnly DefaultCultureInfo As New CultureInfo("zh-Hans")
    <STAThread>
    Public Function Main(args As String()) As Integer
        If SingleInstance.InitializeAsFirstInstance(Of Application)("TsinghuaNet") Then
            Using Log
                Try
                    Try
                        Dim eventLog As New FileInfo(FileName)
                        If eventLog.Length > Integer.MaxValue Then
                            eventLog.Delete()
                        End If
                        Program.Log = New LogWriter(FileName, True)
                    Catch ex As FileNotFoundException
                        Program.Log = New LogWriter(FileName)
                    Catch ex As Exception
                        ShowError(ex.ToString())
                        Return False
                    End Try
                    WriteLog($"程序启动: {Assembly.GetExecutingAssembly().GetName().Version}")
                    Dim app As New Application
                    app.Run()
                    WriteEvent("程序结束")
                Catch ex As Exception
                    WriteException(ex)
                    ShowError(ex.ToString())
                    Return 1
                End Try
                Log.Flush()
            End Using
            SingleInstance.Cleanup()
        End If
        Return 0
    End Function
    Public Async Sub WriteLog(message As String)
        Try
            Dim task As Task
            SyncLock Lock
                task = Log.WriteLogAsync(message)
            End SyncLock
            Await task
        Catch ex As InvalidOperationException
        End Try
    End Sub
    Public Async Sub WriteException(exception As Exception)
        Try
            Dim task As Task
            SyncLock Lock
#If DEBUG Then
                task = Log.WriteFullExceptionAsync(exception)
#Else
                task = Log.WriteExceptionAsync(exception)
#End If
            End SyncLock
            Await task
        Catch ex As InvalidOperationException
        End Try
    End Sub
    Public Async Sub WriteEvent(name As String)
        Try
            Dim task As Task
            SyncLock Lock
                task = Log.WriteEventAsync(name)
            End SyncLock
            Await task
        Catch ex As InvalidOperationException
        End Try
    End Sub
#If DEBUG Then
    Public Async Sub WriteDebug(message As String)
        Try
            Dim task As Task
            SyncLock Lock
                task = Log.WriteDebugAsync(message)
            End SyncLock
            Await task
        Catch ex As InvalidOperationException
        End Try
    End Sub
#End If
    Friend Sub ShowError(message As String)
        MessageBox.Show(message, My.Resources.Title, MessageBoxButton.OK, MessageBoxImage.Error)
    End Sub
End Module
