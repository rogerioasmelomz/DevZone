Option Strict Off
Option Explicit On
<System.Runtime.InteropServices.ProgId("clsOperational_NET.clsOperational")> Public Class clsOperational
	
	Private mCompanyId As String

    'Private Declare Function ShellExecute Lib "shell32.dll"  Alias "ShellExecuteA"(ByVal hWnd As Integer, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Integer) As Integer
    '' Get user temporary files pathname
    'Private Declare Function GetTempPath Lib "kernel32"  Alias "GetTempPathA"(ByVal nBufferLength As Integer, ByVal lpBuffer As String) As Integer



    Public Property CompanyId() As String
		Get
			CompanyId = mCompanyId
		End Get
		Set(ByVal Value As String)
			mCompanyId = Value
		End Set
	End Property
	
	
	Public Function AssociateDocuments(ByRef strErrors As String, ByRef strGUID As String, ByRef objFilter As clsFilter, ByRef lngHWND As Integer) As Boolean
		
		'Associação de documentos
		AssociateDocuments = False
		
	End Function
	
	Public Function RemoveAssociation(ByRef strErrors As String, ByRef strGUID As String, ByRef lngHWND As Integer) As Boolean
		'Remove a associação do documento
		RemoveAssociation = False
	End Function
	
	
	Public Function ViewDocuments(ByRef strErrors As String, ByRef strGUID As String, ByRef lngHWND As Integer) As Boolean

        MessageBox.Show(strGUID)

        ViewDocuments = True
		
	End Function

    ''Obtem a directoria temporaria
    'Private Function fGetTempPath() As String

    '	Dim strTemp As New VB6.FixedLengthString(255)
    '	Dim lngLEN As Integer

    '	strTemp.Value = New String(Chr(0), 255)
    '	lngLEN = GetTempPath(255, strTemp.Value)
    '	fGetTempPath = Left(strTemp.Value, lngLEN)

    'End Function
End Class