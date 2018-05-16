﻿Imports Berrysoft.Console

Class LogWriter
    Inherits Log
    Public Sub New(fileName As String)
        MyBase.New(fileName)
    End Sub
    Protected Overrides ReadOnly Property ExceptionHeader As String
        Get
            Return "异常"
        End Get
    End Property
    Protected Overrides ReadOnly Property EventHeader As String
        Get
            Return "事件"
        End Get
    End Property
    Protected Overrides ReadOnly Property DebugHeader As String
        Get
            Return "调试"
        End Get
    End Property
    Public Sub WriteFullException(exception As Exception)
        Writer.WriteLine(SpecialMessageFormatString, Date.Now, ExceptionHeader, exception)
    End Sub
End Class
