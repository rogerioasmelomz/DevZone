VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsAbout"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Public Property Get ApplicationName() As String
    ApplicationName = ""
End Property

Public Property Get ApplicationVersion() As String
    ApplicationVersion = ""
End Property

Public Property Get APIVersion() As String
    APIVersion = ""
End Property

Public Property Get Copyright() As String
    Copyright = ""
End Property





