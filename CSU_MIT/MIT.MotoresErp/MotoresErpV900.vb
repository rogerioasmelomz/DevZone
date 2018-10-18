Imports MIT.MotoresErp.Comercial
Imports System.Reflection


Public Class MotoresErpV900
    Public _empresaErp As EmpresaErp
    Public _comercial As MotoresComercial
    Public _administrador As AdministradorErp

    Public Sub New()
        _empresaErp = New EmpresaErp
        _comercial = New MotoresComercial()

        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve
    End Sub

    Public Sub iniciliazarAdministrador(user__1 As String, password__2 As String, Optional instance__3 As String = "DEFAULT", Optional type__4 As String = "Executive", Optional backupsdir__5 As String = Nothing)
        _administrador = New AdministradorErp(user__1, password__2, instance__3, type__4, backupsdir__5)
    End Sub

    Public Sub Gravar_Diferencas_Cambio(docDiferencaPositivos As String,
                                            docDiferencasNegativos As String, valor As Double,
                                            entidade As String, tipoEntidade As String, moeda As String, data As Date)
        _comercial.Gravar_Diferencas_Cambio(_empresaErp.objmotor, docDiferencaPositivos, docDiferencasNegativos, valor, entidade, tipoEntidade, moeda, data)

    End Sub

    ''' <summary>
    ''' Método para resolução das assemblies.
    ''' </summary>
    ''' <param name="sender">Application</param>
    ''' <param name="args">Resolving Assembly Name</param>
    ''' <returns>Assembly</returns>
    Public Function CurrentDomain_AssemblyResolve1(sender As Object, args As ResolveEventArgs) As System.Reflection.Assembly
        Dim assemblyFullName As String
        Dim assemblyName As System.Reflection.AssemblyName

        Dim PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.daPastaConfig() ' pastaConfig '"PRIMAVERA\\S800" ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.

        assemblyName = New System.Reflection.AssemblyName(args.Name)
        assemblyFullName = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER), assemblyName.Name + ".dll")
        If (System.IO.File.Exists(assemblyFullName)) Then
            Return System.Reflection.Assembly.LoadFile(assemblyFullName)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary> 
    '''     Módulo de exemplo de carregamento de Interops da pasta de ficheiros comuns. 
    ''' </summary> 
    ''' <remarks></remarks> 
    <Runtime.InteropServices.ComVisible(False)> _
    Public Function CurrentDomain_AssemblyResolve(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly

        Dim PRIMAVERA_COMMON_FILES_FOLDER As String = Instancia.daPastaConfig()  '"PRIMAVERA\\SG800"

        Dim outAssembly, objExeAssembly As Assembly
        Dim strTempAssemblyPath As String = ""
        Dim strArgToLoad As String

        objExeAssembly = Assembly.GetExecutingAssembly
        Dim arrRefAssemblyNames() As AssemblyName = objExeAssembly.GetReferencedAssemblies

        strArgToLoad = args.Name.Substring(0, args.Name.IndexOf(","))

        For Each strAName As AssemblyName In arrRefAssemblyNames

            If strAName.FullName.Substring(0, strAName.FullName.IndexOf(",")) = strArgToLoad Then

                strTempAssemblyPath = System.IO.Path.Combine(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER),
                    strArgToLoad + ".dll"
                )

                Exit For
            End If

        Next

        'Valida nome do assembly
        If String.IsNullOrEmpty(strTempAssemblyPath) Then
            outAssembly = Nothing
        Else
            outAssembly = Assembly.LoadFrom(strTempAssemblyPath)
        End If

        Return outAssembly

    End Function


End Class
