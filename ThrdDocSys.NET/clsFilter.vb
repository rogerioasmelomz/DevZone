Option Strict Off
Option Explicit On
<System.Runtime.InteropServices.ProgId("clsFilter_NET.clsFilter")> Public Class clsFilter
	Private mDocumentType As String
	Private mEntityType As String
	Private mEntityName As String
	Private mModuleId As String
	
	Public Property DocumentType() As String
		Get
			DocumentType = mDocumentType
		End Get
		Set(ByVal Value As String)
			mDocumentType = Value
		End Set
	End Property
	
	Public Property EntityType() As String
		Get
			EntityType = mEntityType
		End Get
		Set(ByVal Value As String)
			mEntityType = Value
		End Set
	End Property
	
	Public Property EntityName() As String
		Get
			EntityName = mEntityName
		End Get
		Set(ByVal Value As String)
			mEntityName = Value
		End Set
	End Property
	
	Public Property ModuleId() As String
		Get
			ModuleId = mModuleId
		End Get
		Set(ByVal Value As String)
			mModuleId = Value
		End Set
	End Property
End Class