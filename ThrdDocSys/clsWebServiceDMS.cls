VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsWebService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

'Dimensioning private class variables.
Private sc_Service1 As SoapClient30

Private Sub Class_Initialize()
    '*****************************************************************
    'This subroutine will be called each time the class is instantiated.
    'Creates sc_ComplexTypes as new SoapClient30
    '*****************************************************************
    Dim c_WSDL_URL As String
    Dim str_WSML As String
    Dim c_SERVICE As String
    Dim c_PORT As String
    Dim c_SERVICE_NAMESPACE As String
    Dim sFicheiro As String
    Dim slength As Integer
            
    sFicheiro = App.Path + "\WebServiceConfig.ini"

    c_WSDL_URL = Space(256)
    slength = GetPrivateProfileString("Config", "WSDL_URL", "", c_WSDL_URL, 256, sFicheiro)
    c_WSDL_URL = Left(c_WSDL_URL, slength)
    
    str_WSML = Space(256)
    slength = GetPrivateProfileString("Config", "WSML", "", str_WSML, 256, sFicheiro)
    str_WSML = Left(str_WSML, slength)
    
    c_SERVICE = Space(256)
    slength = GetPrivateProfileString("Config", "SERVICE", "", c_SERVICE, 256, sFicheiro)
    c_SERVICE = Left(c_SERVICE, slength)
    
    c_PORT = Space(256)
    slength = GetPrivateProfileString("Config", "PORT", "", c_PORT, 256, sFicheiro)
    c_PORT = Left(c_PORT, slength)
    
    c_SERVICE_NAMESPACE = Space(256)
    slength = GetPrivateProfileString("Config", "SERVICE_NAMESPACE", "", c_SERVICE_NAMESPACE, 256, sFicheiro)
    c_SERVICE_NAMESPACE = Left(c_SERVICE_NAMESPACE, slength)
    

    Set sc_Service1 = New SoapClient30

    sc_Service1.MSSoapInit2 c_WSDL_URL, str_WSML, c_SERVICE, c_PORT, c_SERVICE_NAMESPACE
    'Use the proxy server defined in Internet Explorer's LAN settings by
    'setting ProxyServer to <CURRENT_USER>
    sc_Service1.ConnectorProperty("ProxyServer") = "<CURRENT_USER>"
    'Autodetect proxy settings if Internet Explorer is set to autodetect
    'by setting EnableAutoProxy to True
    sc_Service1.ConnectorProperty("EnableAutoProxy") = True


End Sub

Private Sub Class_Terminate()
    '*****************************************************************
    'This subroutine will be called each time the class is destructed.
    'Sets sc_ComplexTypes to Nothing.
    '*****************************************************************

    'Error Trap
    On Error GoTo Class_TerminateTrap

    Set sc_Service1 = Nothing

Exit Sub

Class_TerminateTrap:
    Service1ErrorHandler ("Class_Terminate")
End Sub

Private Sub Service1ErrorHandler(str_Function As String)
    '*****************************************************************
    'This subroutine is the class error handler. It can be called from any class subroutine or function
    'when that subroutine or function encounters an error. Then, it will raise the error along with the
    'name of the calling subroutine or function.
    '*****************************************************************

    'SOAP Error
    If sc_Service1.FaultCode <> "" Then
        Err.Raise vbObjectError, str_Function, sc_Service1.FaultString
    'Non SOAP Error
    Else
        Err.Raise Err.Number, str_Function, Err.Description
    End If

End Sub

Public Function wsm_GetDocumentFile(ByVal str_Id As String) As Variant

    'Error Trap
    On Error GoTo wsm_GetDocumentFileTrap

    wsm_GetDocumentFile = sc_Service1.GetDocumentFile(str_Id)

Exit Function
wsm_GetDocumentFileTrap:
    Service1ErrorHandler "wsm_GetDocumentFile"
End Function


Public Function wsm_GetDocumentExtension(ByVal str_Id As String) As String

    'Error Trap
    On Error GoTo wsm_GetDocumentExtensionTrap

    wsm_GetDocumentExtension = sc_Service1.GetDocumentExtension(str_Id)

Exit Function
wsm_GetDocumentExtensionTrap:
    Service1ErrorHandler "wsm_GetDocumentExtension"
End Function

