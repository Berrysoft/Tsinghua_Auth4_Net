Imports Microsoft.VisualBasic.ApplicationServices

Module Program
    <STAThread>
    Sub Main(args As String())
        Dim manager As New SingleInstanceManager()
        manager.Run(args)
    End Sub
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