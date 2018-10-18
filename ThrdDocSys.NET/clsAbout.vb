Option Strict Off
Option Explicit On
<System.Runtime.InteropServices.ProgId("clsAbout_NET.clsAbout")> Public Class clsAbout
	Public ReadOnly Property ApplicationName() As String
		Get
			ApplicationName = ""
		End Get
	End Property
	
	Public ReadOnly Property ApplicationVersion() As String
		Get
			ApplicationVersion = ""
		End Get
	End Property
	
	Public ReadOnly Property APIVersion() As String
		Get
			APIVersion = ""
		End Get
	End Property
	
	Public ReadOnly Property Copyright() As String
		Get
			Copyright = ""
		End Get
	End Property
End Class