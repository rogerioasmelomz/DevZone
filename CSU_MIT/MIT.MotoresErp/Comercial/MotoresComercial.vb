Imports Interop.ErpBS900
Imports Interop.GcpBE900
Imports System.IO
Imports Interop.StdBE900
Imports Interop.StdPlatBS900

Namespace Comercial
    Public Class MotoresComercial
        Public modulo As String = "V"

#Region "Documentos de Vendas"

        Public documentoVenda As GcpBEDocumentoVenda
        Public linhasDocVenda As GcpBELinhasDocumentoVenda


        Public Sub PreencheCabecalho_DocumentoVenda(objmotor As ErpBS, serie As String, dataDoc As Date, dataGravacao As Date, tipoEntidade As String,
                                                    codigoEntidade As String, ByRef condicoesPagamento As String, ByRef modoPagamento As String,
                                                    ByRef moeda As String, cambio As Double, cambioMoedaAlternativa As Double, referencia As String)

            documentoVenda = New GcpBEDocumentosVenda

            documentoVenda.Serie = serie
            documentoVenda.DataDoc = dataDoc
            documentoVenda.DataGravacao = dataGravacao

            documentoVenda.TipoEntidade = tipoEntidade
            documentoVenda.Entidade = codigoEntidade
            documentoVenda.Requisicao = referencia

            objmotor.Comercial.Vendas.PreencheDadosRelacionados(documentoVenda)

            documentoVenda.DataDoc = dataDoc
            documentoVenda.DataGravacao = dataGravacao
        End Sub

        Public Sub Adiciona_Linhas_DocumentoVenda(objmotor As ErpBS, artigo As String, quantidade As Double, ByRef armazem As String,
                                                  ByRef localizacao As String, precoUnitario As Double)
            objmotor.Comercial.Vendas.AdicionaLinha(documentoVenda, artigo, quantidade, armazem, localizacao, precoUnitario)
        End Sub

        Public Function Gravar_DocumentoVenda(objmotor As ErpBS) As PrimaveraResultStructure
            Dim resultado As PrimaveraResultStructure = New PrimaveraResultStructure()
            Dim avisos As String = ""
            Try
                objmotor.Comercial.Vendas.Actualiza(documentoVenda, avisos)

                If avisos = "" Then
                    resultado.codigo = 1
                    resultado.tipoProblema = avisos
                    resultado.descricao = documentoVenda.Tipodoc + " " + Str(documentoVenda.NumDoc) + "/" + documentoVenda.Serie

                Else
                    resultado.codigo = 0
                    resultado.descricao = documentoVenda.Tipodoc + " " + Str(documentoVenda.NumDoc) + "/" + documentoVenda.Serie
                End If

            Catch ex As Exception
                resultado.codigo = 3
                resultado.descricao = ex.Message

            End Try

            Return resultado
        End Function

        Public Function ImprimirDocumentoVenda(objmotor As ErpBS, objPlataforma As StdPlatBS) As String

            'Dim objPlat As StdPlatBS
            Dim bMapaSistema As Boolean
            Dim strFormula As String
            Dim sFiltro As String
            Dim sMapa As String

            Dim sFileName As String = String.Format("{0}{1}.pdf", My.Computer.FileSystem.CurrentDirectory, documentoVenda.ID)

            objPlataforma.Mapas.Inicializar("GCP")

            'strFormula = "NumberVar TipoDesc;NumberVar RegimeIva;NumberVar DecQde;NumberVar DecPrecUnit;StringVar MotivoIsencao; TipoDesc:=" & 1 & ";RegimeIva:=3;DecQde:=1;DecPrecUnit:="& 2 & ";MotivoIsencao:=' ';"
            'objPlat.Mapas.AddFormula("InicializaParametros", strFormula)

            strFormula = " StringVar Nome; StringVar Morada;StringVar Localidade; StringVar CodPostal; StringVar Telefone; StringVar Fax; StringVar Contribuinte; StringVar CapitalSocial; StringVar Conservatoria; StringVar Matricula;StringVar MoedaCapitalSocial;StringVar DecQtd;StringVar DecArrMoedaBase;StringVar DecPrecMoedaBase;"

            strFormula = strFormula & "Nome:='" & objPlataforma.Contexto.Empresa.IDNome & "'"
            strFormula = strFormula & ";Localidade:='" & objPlataforma.Contexto.Empresa.IDLocalidade & "'"
            strFormula = strFormula & ";CodPostal:='" & objPlataforma.Contexto.Empresa.IDLocalidadeCod & "'"
            strFormula = strFormula & ";Telefone:='" & objPlataforma.Contexto.Empresa.IDTelefone & "'"
            strFormula = strFormula & ";Fax:='" & objPlataforma.Contexto.Empresa.IDFax & "'"
            strFormula = strFormula & ";Contribuinte:='" & objPlataforma.Contexto.Empresa.IFNIF & "'"
            strFormula = strFormula & ";CapitalSocial:='" & objPlataforma.Contexto.Empresa.ICCapitalSocial & "'"
            strFormula = strFormula & ";Conservatoria:='" & objPlataforma.Contexto.Empresa.ICConservatoria & "'"
            strFormula = strFormula & ";Matricula:='" & objPlataforma.Contexto.Empresa.ICMatricula & "'"
            strFormula = strFormula & ";MoedaCapitalSocial:='" & objPlataforma.Contexto.Empresa.ICMoedaCapSocial & "'"
            strFormula = strFormula & ";DecQtd:='2';DecArrMoedaBase:='0';DecPrecMoedaBase:='2'"

            strFormula = strFormula & ";"
            objPlataforma.Mapas.AddFormula("DadosEmpresa", strFormula)

            'filtro para o documento a imprimir

            sFiltro = String.Format("{{Cabecdoc.Serie}}='{0}' and {{cabecdoc.tipodoc}}='{1}' and {{cabecdoc.numdoc}}={2}",
                                          documentoVenda.Serie, documentoVenda.Tipodoc, documentoVenda.NumDoc)

            objPlataforma.Mapas.AddFormula("NumVia", "'Original'")
            objPlataforma.Mapas.SelectionFormula = sFiltro

            ' Modelo definido para a serie
            sMapa = objmotor.Comercial.Series.DaValorAtributo(modulo, documentoVenda.Tipodoc, documentoVenda.Serie, "Config")


            Try
                bMapaSistema = Conversion.Int(objPlataforma.Administrador.Consulta("select Custom from mapas where mapa = '" +
                                                                                   sMapa + "'").Valor(0))
            Catch ex As Exception
                bMapaSistema = False
            End Try

            ' Propriedades para o output dos mapas
            'objPlataforma.Mapas.set(CRPEExportDestino.edFicheiro)
            objPlataforma.Mapas.SetFileProp(CRPEExportFormat.efPdf, sFileName)

            objPlataforma.Mapas.ImprimeListagem(sMapa, "Factura", "P", 1, "S", sFiltro, CRPESentidoOrdenacao.soNenhuma, False, bMapaSistema, "", False, 0, False)
            'objPlat.FechaPlataformaEmpresa()
            'objPlat = Nothing
            If (File.Exists(sFileName)) Then

                Return sFileName
            End If




            ' Descomentar as duas linhas seguintes para exportar a factura para pdf.
            ' objPlat.Mapas.Destino = edFicheiro
            ' objPlat.Mapas.SetFileProp(efPdf, "TESTE.pdf")

            Return Nothing
        End Function

        Private Sub imprimirDoc(idDoc As String)

        End Sub
#End Region

#Region "Contas Correntes"

        Public Sub Gravar_Diferencas_Cambio(objmotor As ErpBS, docDiferencaPositivos As String,
                                            docDiferencasNegativos As String, valor As Double,
                                            entidade As String, tipoEntidade As String, moeda As String, data As Date)


            Dim i As Long

            documentoPendente = New GcpBEPendente


            If valor > 0 Then
                documentoPendente.Tipodoc = docDiferencaPositivos
            Else
                documentoPendente.Tipodoc = docDiferencasNegativos
                valor = valor * -1
            End If


            documentoPendente.Serie = objmotor.Comercial.Series.DaSerieDefeito("M", documentoPendente.Tipodoc, data)
            documentoPendente.Entidade = entidade
            documentoPendente.TipoEntidade = tipoEntidade

            objmotor.Comercial.Pendentes.PreencheDadosRelacionados(documentoPendente)
            documentoPendente.Moeda = objmotor.Contexto.MoedaBase

            documentoPendente.ValorTotal = valor
            documentoPendente.TotalIva = 0
            documentoPendente.ValorPendente = valor

            documentoPendente.Cambio = 1
            documentoPendente.CambioMBase = 1
            documentoPendente.CambioMAlt = 0.0

            objmotor.Comercial.Pendentes.Actualiza(documentoPendente)

            documentoPendente.EmModoEdicao = True

            documentoPendente.ValorTotal = valor
            documentoPendente.TotalIva = 0
            documentoPendente.ValorPendente = valor


            documentoPendente.Cambio = 1
            documentoPendente.CambioMBase = 1
            documentoPendente.CambioMAlt = 0.0

            objmotor.Comercial.Pendentes.Actualiza(documentoPendente)


        End Sub

        Public documentoPendente As GcpBEPendente
        Public linhasdocumentoPendente As GCPBELinhaPendente

        Public Sub PreencheCabecalho_DocumentoPendente(objmotor As ErpBS, serie As String, dataDoc As Date, dataGravacao As Date, tipoEntidade As String,
                                                    codigoEntidade As String, ByRef condicoesPagamento As String, ByRef modoPagamento As String,
                                                    ByRef moeda As String, Optional ByVal cambio As Double = 1, Optional ByVal cambioMoedaBase As Double = 1,
                                                    Optional ByVal cambioMoedaAlternativa As Double = 0.0,
                                                    Optional ByVal valor As Double = 0, Optional ByVal totalIva As Double = 0)

            documentoPendente = New GcpBEPendente

            documentoPendente.Serie = serie


            documentoPendente.TipoEntidade = tipoEntidade
            documentoPendente.Entidade = codigoEntidade

            objmotor.Comercial.Pendentes.PreencheDadosRelacionados(documentoPendente)

            documentoPendente.DataDoc = dataDoc
            documentoPendente.DataIntroducao = dataGravacao
            documentoPendente.Moeda = moeda


            documentoPendente.ValorTotal = valor
            documentoPendente.TotalIva = 0
            documentoPendente.ValorPendente = valor

            documentoPendente.Cambio = cambio
            documentoPendente.CambioMBase = cambioMoedaBase
            documentoPendente.CambioMAlt = cambioMoedaAlternativa



        End Sub

        Public Sub Adiciona_Linhas_DocumentoPendente(objmotor As ErpBS, Optional ByVal valor As Double = 0, Optional ByVal totalIva As Double = 0)
            'linhasdocumentoPendente = New GCPBELinhaPendente
            'documentoPendente.Linhas.Insere(linhasdocumentoPendente)

            'objmotor.Comercial.Pendentes.LstPen.AdicionaLinha(documentoVenda, artigo, quantidade, armazem, localizacao, precoUnitario)
        End Sub

        Public Function Gravar_DocumentoPendente(objmotor As ErpBS) As PrimaveraResultStructure
            Dim resultado As PrimaveraResultStructure = New PrimaveraResultStructure()
            Dim avisos As String = ""
            Try

                objmotor.Comercial.Pendentes.Actualiza(documentoPendente, avisos)
                If avisos = "" Then
                    resultado.codigo = 1
                    resultado.tipoProblema = avisos
                    resultado.descricao = documentoPendente.Tipodoc + " " + Str(documentoPendente.NumDoc) + "/" + documentoPendente.Serie

                Else
                    resultado.codigo = 0
                    resultado.descricao = documentoPendente.Tipodoc + " " + Str(documentoPendente.NumDoc) + "/" + documentoPendente.Serie
                End If

            Catch ex As Exception
                resultado.codigo = 3
                resultado.descricao = ex.Message

            End Try

            Return resultado
        End Function

#End Region
    End Class

End Namespace