﻿'------------------------------------------------------------------------------
' <auto-generated>
'     此代码由工具生成。
'     运行时版本:4.0.30319.42000
'
'     对此文件的更改可能会导致不正确的行为，并且如果
'     重新生成代码，这些更改将会丢失。
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    '此类是由 StronglyTypedResourceBuilder
    '类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    '若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    '(以 /str 作为命令选项)，或重新生成 VS 项目。
    '''<summary>
    '''  一个强类型的资源类，用于查找本地化的字符串等。
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Public Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  返回此类使用的缓存的 ResourceManager 实例。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("TsinghuaNet.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  使用此强类型资源类，为所有资源查找
        '''  重写当前线程的 CurrentUICulture 属性。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  查找类似 Client 的本地化字符串。
        '''</summary>
        Public ReadOnly Property ClientHeader() As String
            Get
                Return ResourceManager.GetString("ClientHeader", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Exit 的本地化字符串。
        '''</summary>
        Public ReadOnly Property CloseText() As String
            Get
                Return ResourceManager.GetString("CloseText", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Login 的本地化字符串。
        '''</summary>
        Public ReadOnly Property Connect() As String
            Get
                Return ResourceManager.GetString("Connect", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Logging in... 的本地化字符串。
        '''</summary>
        Public ReadOnly Property Connecting() As String
            Get
                Return ResourceManager.GetString("Connecting", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Login failed 的本地化字符串。
        '''</summary>
        Public ReadOnly Property ConnectionFailed() As String
            Get
                Return ResourceManager.GetString("ConnectionFailed", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Login failed: {0} 的本地化字符串。
        '''</summary>
        Public ReadOnly Property ConnectionFailedWithResult() As String
            Get
                Return ResourceManager.GetString("ConnectionFailedWithResult", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Logged out. 的本地化字符串。
        '''</summary>
        Public ReadOnly Property Disconnected() As String
            Get
                Return ResourceManager.GetString("Disconnected", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 18 的本地化字符串。
        '''</summary>
        Public ReadOnly Property FontSize() As String
            Get
                Return ResourceManager.GetString("FontSize", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 IP address 的本地化字符串。
        '''</summary>
        Public ReadOnly Property IPHeader() As String
            Get
                Return ResourceManager.GetString("IPHeader", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Language: 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LanguageWithColon() As String
            Get
                Return ResourceManager.GetString("LanguageWithColon", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Less information 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LessInf() As String
            Get
                Return ResourceManager.GetString("LessInf", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 150 的本地化字符串。
        '''</summary>
        Public ReadOnly Property ListHeight() As String
            Get
                Return ResourceManager.GetString("ListHeight", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Logging out... 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LoggingOut() As String
            Get
                Return ResourceManager.GetString("LoggingOut", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Login time 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LoginTimeHeader() As String
            Get
                Return ResourceManager.GetString("LoginTimeHeader", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Logout 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LogOut() As String
            Get
                Return ResourceManager.GetString("LogOut", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Logout failed 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LogOutFailed() As String
            Get
                Return ResourceManager.GetString("LogOutFailed", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Logout failed: {0} 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LogOutFailedWithResult() As String
            Get
                Return ResourceManager.GetString("LogOutFailedWithResult", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Logout selected items 的本地化字符串。
        '''</summary>
        Public ReadOnly Property LogOutSelected() As String
            Get
                Return ResourceManager.GetString("LogOutSelected", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 More information 的本地化字符串。
        '''</summary>
        Public ReadOnly Property MoreInf() As String
            Get
                Return ResourceManager.GetString("MoreInf", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 No network. 的本地化字符串。
        '''</summary>
        Public ReadOnly Property NoNetwork() As String
            Get
                Return ResourceManager.GetString("NoNetwork", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Show client 的本地化字符串。
        '''</summary>
        Public ReadOnly Property NotifyText() As String
            Get
                Return ResourceManager.GetString("NotifyText", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 OK 的本地化字符串。
        '''</summary>
        Public ReadOnly Property OK() As String
            Get
                Return ResourceManager.GetString("OK", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Password: 的本地化字符串。
        '''</summary>
        Public ReadOnly Property PasswordWithColon() As String
            Get
                Return ResourceManager.GetString("PasswordWithColon", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Refresh 的本地化字符串。
        '''</summary>
        Public ReadOnly Property Refresh() As String
            Get
                Return ResourceManager.GetString("Refresh", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 50 的本地化字符串。
        '''</summary>
        Public ReadOnly Property SingleHeight() As String
            Get
                Return ResourceManager.GetString("SingleHeight", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Tsinghua Campus Network Client 的本地化字符串。
        '''</summary>
        Public ReadOnly Property Title() As String
            Get
                Return ResourceManager.GetString("Title", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 Username: 的本地化字符串。
        '''</summary>
        Public ReadOnly Property UsernameWithColon() As String
            Get
                Return ResourceManager.GetString("UsernameWithColon", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 View code on GitHub... 的本地化字符串。
        '''</summary>
        Public ReadOnly Property ViewOnGitHub() As String
            Get
                Return ResourceManager.GetString("ViewOnGitHub", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  查找类似 460 的本地化字符串。
        '''</summary>
        Public ReadOnly Property Width() As String
            Get
                Return ResourceManager.GetString("Width", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
