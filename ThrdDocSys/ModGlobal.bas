Attribute VB_Name = "ModGlobal"
Option Explicit

' standard API declarations for INI access
' changing only "As Long" to "As Int32" (As Integer would work also)
Public Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal Secao As String, ByVal parametro As Any, ByVal padrao As String, ByVal variavel As String, ByVal tam As Long, ByVal arquivo As String) As Long

Public DocWebService As clsWebService

