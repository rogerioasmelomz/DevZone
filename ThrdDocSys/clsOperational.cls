VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsOperational"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Private mCompanyId As String

Private Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" (ByVal hWnd As Long, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Long) As Long
' Get user temporary files pathname
Private Declare Function GetTempPath Lib "kernel32" Alias "GetTempPathA" (ByVal nBufferLength As Long, ByVal lpBuffer As String) As Long


Public Property Get CompanyId() As String
    CompanyId = mCompanyId
End Property

Public Property Let CompanyId(sValue As String)
    mCompanyId = sValue
End Property


Public Function AssociateDocuments(strErrors As String, strGUID As String, objFilter As clsFilter, lngHWND As Long) As Boolean
    
    'Associação de documentos
    AssociateDocuments = False
    
End Function

Public Function RemoveAssociation(strErrors As String, strGUID As String, lngHWND As Long) As Boolean
    'Remove a associação do documento
    RemoveAssociation = False
End Function


Public Function ViewDocuments(strErrors As String, strGUID As String, lngHWND As Long) As Boolean
    Dim sTempFile As String
    Dim iFileNum As Integer
    Dim lFileLength As Long
    Dim sIDteste As String
    Dim abBytes() As Byte
    
    Set DocWebService = New clsWebService
    

    sTempFile = fGetTempPath & "\" & strGUID & "." & DocWebService.wsm_GetDocumentExtension(strGUID)

    iFileNum = FreeFile

    Open sTempFile For Binary As #iFileNum
    
    abBytes() = DocWebService.wsm_GetDocumentFile(strGUID)

    Put #iFileNum, , abBytes()
    Close #iFileNum

    ShellExecute 0&, vbNullString, sTempFile, vbNullString, vbNullString, vbNormalFocus

    Set DocWebService = Nothing
    
    ViewDocuments = True

End Function

''Obtem a directoria temporaria
Private Function fGetTempPath() As String

Dim strTemp       As String * 255
Dim lngLEN        As Long

    strTemp = String(255, 0)
    lngLEN = GetTempPath(255, strTemp)
    fGetTempPath = Left$(strTemp, lngLEN)

End Function

