Option Strict Off
Option Explicit On
Module ModGlobal

    ' standard API declarations for INI access
    ' changing only "As Long" to "As Int32" (As Integer would work also)
    'UPGRADE_ISSUE: Declaring a parameter 'As Any' is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
    '   Public Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal Secao As String, ByVal parametro As Object, ByVal padrao As String, ByVal variavel As String, ByVal tam As Integer, ByVal arquivo As String) As Integer

    'Public DocWebService As clsWebService

End Module