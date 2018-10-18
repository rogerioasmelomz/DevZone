Imports Interop.StdBE800
Imports Interop.GcpBE800
Imports Interop.CrmBE800
Imports Interop.ErpBS800

Public Class ModImpLiquidacoes

    Public objmotor As New ErpBS

    '---------------------------------------------------------------------------------------
    ' Procedure : ImportaFicheiroBanco
    ' Autor     : ricardo.silva
    ' Data      : 28-10-2011
    ' Descrição :
    '---------------------------------------------------------------------------------------
    '
#Region "Importacao Liquidacoes"
    Public Function ImportaFicheiroBanco(NomeFicheiroPorIntegrar As String, NomeFicheiroSimples As String) As Boolean
        Dim Params As Parametros

        Dim objDocLiq As GcpBEDocumentoLiq
        Dim objPendente As GcpBEPendente
        Dim objAdiantamento As GcpBEPendente
        Dim lstLista As StdBELista

        Dim strTipoentidade As String
        Dim strEntidade As String
        Dim strSQL As String
        Dim lngLinFile As Long
        Dim dtDataMovBMEPS As Date
        Dim strReferenciaMB As String
        Dim strIdTransBMEPS As String
        Dim strFicheiroBMEPS As String
        Dim strTipoLinha As String
        Dim dblValorPago As Double
        Dim dblValorTaxa As Double
        Dim dblValorRestante As Double
        Dim strIdHistorico As String
        Dim dblValorLiquidar As Double
        Dim dblValorIntegradoERP As Double

        Dim strFilial As String
        Dim strModulo As String
        Dim strTipoDoc As String
        Dim strSerie As String
        Dim lngNumDoc As Long
        Dim intNumPrestacao As Integer
        Dim intNumTransferencia As Integer
        Dim dblTotalTaxas As Double
        Dim strRubrica As String
        Dim lngNumDocADC As Long

        Dim blnTeveErros As Boolean
        Dim objFicheiroLinhas As Object
        Dim strLinFile As String
        Dim strPontoErro As String


        'Variáveis com parametros Institutos
        Dim strInstituto As String
        Dim strDocLiquidacao As String
        Dim strDocAdiantamento As String
        Dim strDocTesouraria As String
        Dim strContaBancaria As String
        Dim strMovimentoBancario As String
        Dim strMovimentoBancarioCom As String


        'arrays para imprimir recibos.
        Dim ArrTipoDoc(10000) As String
        Dim ArrSerie(10000) As String
        Dim ArrNumDoc(10000) As Long
        Dim ArrFilial(10000) As String
        Dim ArrTipoEntidade(10000) As String
        Dim ArrEntidade(10000) As String
        Dim iImp As Long
        Dim I As Long

        Dim blnTrans As Boolean



        On Error GoTo errHandler

        blnTrans = False
        ImportaFicheiroBanco = True
        blnTeveErros = False
        strFicheiroBMEPS = NomeFicheiroSimples
        dblTotalTaxas = 0
        'TextoCriacaoEmail = ""
        'EmailResponsavelLQ = ""
        iImp = 0

        strInstituto = ""
        strDocLiquidacao = ""
        strDocAdiantamento = ""
        strDocTesouraria = ""
        strContaBancaria = ""
        strMovimentoBancario = ""

        'Valida a existencia do ficheiro em historico, caso afirmativo, aborta a integração deste mesmo ficheiro
        If Not ValidaFicheiroHistorico(NomeFicheiroSimples) Then
            ImportaFicheiroBanco = False
            Exit Function
        End If

        objmotor.IniciaTransaccao()
        blnTrans = True




        'ABRE FICHEIRO PARA COLLECTION
        objFicheiroLinhas = objFicheiros.Le_Ficheiro(NomeFicheiroPorIntegrar)

        'PERCORRE TODAS AS LINHAS DO FICHEIRO
        For lngLinFile = 1 To objFicheiroLinhas.Count

            'LINHA ACTUAL DO FICHEIRO
            strLinFile = objFicheiroLinhas.Item(lngLinFile)

            strTipoLinha = FStr(Mid(strLinFile, 1, 1))

            If strTipoLinha = "1" Then

                'LÊ DADOS DA LINHA DO FICHEIRO
                strReferenciaMB = FStr(Mid(strLinFile, 2, 11))
                dtDataMovBMEPS = DaDataInv(Mid(strLinFile, 45, 8))
                strIdTransBMEPS = FStr(Mid(strLinFile, 59, 7))
                dblValorPago = DaValor(Mid(strLinFile, 13, 16), 2)
                dblValorTaxa = DaValor(Mid(strLinFile, 29, 16), 2)

                dblTotalTaxas = dblTotalTaxas + dblValorTaxa

                'SELECCTIONA A ENTIDADE A QUEM A REFERÊNCIA FOI ATRIBUIDA E O INSTITUTO
                strSQL = "SELECT TOP 1 TIPOENTIDADECOMERCIAL, ENTIDADECOMERCIAL, CDU_INSTITUTO FROM HISTORICO" & vbCrLf & _
                         "WHERE MODULO = 'V' AND CDU_REFERENCIA = '" & strReferenciaMB & "'"

                lstLista = objmotor.Consulta(strSQL)

                'VERIFICA SE EXISTE ALGUM DOCUMENTO COM A REFERÊNCIA INDICADA
                If lstLista.NumLinhas > 0 Then

                    strTipoentidade = lstLista.Valor("TIPOENTIDADECOMERCIAL")
                    strEntidade = lstLista.Valor("ENTIDADECOMERCIAL")

                    If Len(strInstituto) = 0 Then
                        strInstituto = lstLista.Valor("CDU_INSTITUTO")
                        strDocLiquidacao = Params.DaDocLiquidacao(strInstituto)
                        strDocAdiantamento = Params.DaDocAdiantamento(strInstituto)
                        strDocTesouraria = Params.DaDocTesouraria(strInstituto)
                        strContaBancaria = Params.DaContaBancaria(strInstituto)
                        strMovimentoBancario = Params.DaMovimentoBancario(strInstituto)
                        'Movimento para comissoes
                        strMovimentoBancarioCom = Params.DaMovimentoBancarioCom(strInstituto)
                        'EmailResponsavelLQ = Params.DaEmailResponsavelLQ(strInstituto)
                    End If

                    lstLista = Nothing

                    'DETERMINA SE JÁ INTEGROU A LINHA ACTUAL
                    'SE INTEGROU, ENTÃO DEVOLVE VALOR INTEGRADO
                    '****************************************************************************************************
                    'Alterado 2013-08-28 RM e AL
                    '****************************************************************************************************
                    '                strSQL = "SELECT SUM (ISNULL(VAL,0)) AS VALOR " & vbCrLf & _
                    '                         "FROM (SELECT SUM(ABS(VALORREC)) AS VAL FROM CabLiq" & vbCrLf & _
                    '                         "      WHERE                        VALORREC > 0 AND TIPOENTIDADE            = '" & strTipoentidade & "' AND ENTIDADE            = '" & strEntidade & "' AND CDU_Referencia = '" & strReferenciaMB & "' AND CDU_FicheiroBMEPS = '" & strFicheiroBMEPS & "' AND CDU_IDTransBMEPS = '" & strIdTransBMEPS & "'" & vbCrLf & _
                    '                         "      Union All" & vbCrLf & _
                    '                         "      SELECT SUM(ABS(P.VALORTOTAL)) AS VAL FROM PENDENTES P INNER JOIN HISTORICO H ON P.IDHISTORICO = H.ID " & vbCrLf & _
                    '                         "      WHERE P.MODULO = 'M' AND P.VALORTOTAL < 0 AND P.TIPOENTIDADECOMERCIAL = '" & strTipoentidade & "' AND P.ENTIDADECOMERCIAL = '" & strEntidade & "' AND H.CDU_Referencia = '" & strReferenciaMB & "' AND H.CDU_FicheiroBMEPS = '" & strFicheiroBMEPS & "' AND H.CDU_IDTransBMEPS = '" & strIdTransBMEPS & "') AS TEMP" & vbCrLf
                    '
                    '                Set lstLista = ErpBSO.Consulta(strSQL)
                    '
                    '                If lstLista.NumLinhas > 0 Then
                    '
                    '                    lstLista.Inicio
                    '                    dblValorIntegradoERP = lstLista("VALOR")
                    '
                    '                Else
                    '
                    '                    dblValorIntegradoERP = 0
                    '
                    '                End If
                    '                Set lstLista = Nothing
                    '                '******************************************
                    '****************************************************************************************************

                    'RETIRA O VALOR JÁ INTEGRADO NO ERP PRIMAVERA
                    'Alterado 2013-08-28 RM e AL
                    dblValorRestante = dblValorPago '- dblValorIntegradoERP


                    'SÓ VERIFICA PENDENTES SE AINDA TIVER VALOR PARA INTEGRAR
                    If dblValorRestante > 0 Then

                        'SELECCIONA DOCUMENTOS PENDENTES DA ENTIDADE (LIQUIDA PRIMEIRO DOCUMENTOS MAIS ANTIGOS)
                        strSQL = "SELECT P.Modulo, P.TipoDoc, P.Serie, P.Filial, P.NumdocInt, P.NumDoc, P.NumPrestacao, P.NumTransferencia, P.ValorPendente, P.DataDoc, P.DataVenc, P.ModoPag, P.CondPag, P.TipoConta, P.Estado, P.CDU_Referencia, P.IDHistorico" & vbCrLf & _
                                 "FROM PENDENTES P INNER JOIN HISTORICO H ON P.IDHISTORICO = H.ID " & vbCrLf & _
                                 "WHERE P.ValorPendente > 0 AND P.TIPOENTIDADECOMERCIAL = '" & strTipoentidade & "'  AND P.ENTIDADECOMERCIAL = '" & strEntidade & "' AND ISNULL(H.CDU_Referencia,'') <> ''" & vbCrLf & _
                                 "ORDER BY P.DATADOC ASC, P.TIPODOC, P.SERIE, P.NUMDOCINT, P.NUMPRESTACAO" & vbCrLf

                        lstLista = objmotor.Consulta(strSQL)

                        If lstLista.NumLinhas > 0 Then

                            'ENCONTROU DOCUMENTOS PENDENTES PARA LIQUIDAR
                            lstLista.Inicio()


                            While Not lstLista.NoFim

                                strFilial = lstLista.Valor("Filial")
                                strModulo = lstLista.Valor("Modulo")
                                strTipoDoc = lstLista.Valor("TipoDoc")
                                strSerie = lstLista.Valor("Serie")
                                lngNumDoc = lstLista.Valor("NumdocInt")
                                intNumPrestacao = lstLista.Valor("NumPrestacao")
                                intNumTransferencia = lstLista.Valor("NumTransferencia")

                                'VAI BUSCAR A RÚBRICA A USAR NO LANÇAMENTO DE TAXAS NA TESOURARIA
                                strRubrica = objmotor.Comercial.TabVendas.DaValorAtributo(strTipoDoc, "CDU_RubricaTaxas")


                                If objDocLiq Is Nothing Then

                                    'CRIA CABEÇALHO DE DOCUMENTO DE LIQUIDAÇÃO
                                    'RMELRO: Alterada a data do documento liquidação de Now para a Data da Linha do ficheiro do banco

                                    'Set objDocLiq = CriaCabecalhoLiquidacao(strTipoentidade, strEntidade, strDocLiquidacao, Now, strReferenciaMB, strIdTransBMEPS, strFicheiroBMEPS, strInstituto)
                                    objDocLiq = CriaCabecalhoLiquidacao(strTipoentidade, strEntidade, strDocLiquidacao, dtDataMovBMEPS, strReferenciaMB, strIdTransBMEPS, strFicheiroBMEPS, strInstituto)

                                End If

                                'EDITA PENDETE PARA ADICIONAR AO DOCUMENTO DE LIQUIDAÇÃO
                                objPendente = objmotor.Comercial.Pendentes.Edita(strFilial, strModulo, strTipoDoc, strSerie, lngNumDoc, intNumPrestacao, intNumTransferencia)

                                If Not objPendente Is Nothing Then

                                    dblValorLiquidar = objPendente.ValorPendente

                                    If dblValorLiquidar <= dblValorRestante Then
                                        dblValorRestante = dblValorRestante - dblValorLiquidar
                                    Else
                                        dblValorLiquidar = dblValorRestante
                                        dblValorRestante = 0
                                    End If

                                    'ADICCIONA LINHA DE LIQUIDAÇÃO DO DOCUMENTO
                                    If Not AdiccionaPendente(objDocLiq, objPendente, dblValorLiquidar) Then

                                        blnTeveErros = True

                                    End If

                                    'SE AINDA TIVER VALOR PARA PAGAR, CONTINUA A PERCORRER OS PENDENTES
                                    If dblValorRestante <= 0 Then

                                        lstLista.Fim()
                                        lstLista.Seguinte()

                                    Else

                                        lstLista.Seguinte()

                                    End If

                                End If

                            End While

                            'GRAVA DOCUMENTO DE LIQUIDAÇÃO
                            If Not GravaDocumentoLiquidacao(objDocLiq) Then

                                blnTeveErros = True

                            Else

                                If Not GravaLigacaoBancos(objDocLiq.TipoEntidade, objDocLiq.Entidade, objDocLiq.Moeda, objDocLiq.ValorRec, objDocLiq.DataDoc, objDocLiq.ID, strInstituto, strReferenciaMB) Then

                                    blnTeveErros = True

                                Else

                                    'TextoCriacaoEmail = TextoCriacaoEmail & " - " & objDocLiq.Tipodoc & " " & objDocLiq.Serie & "/" & CStr(objDocLiq.NumDoc) & " no valor de " & objDocLiq.ValorRec & " " & objDocLiq.Moeda & vbCrLf
                                    'EnviarRecibo objDocLiq.TipoDoc, objDocLiq.Serie, objDocLiq.NumDoc, objDocLiq.Filial, objDocLiq.TipoEntidade, objDocLiq.Entidade, "Emissão de Recibo", "Foi emitido um documento de Recibo de Pagamento"
                                    'Guarda os dados dos documentos, para envio de email fora da transação, já que com transação ele bloqueia.
                                    iImp = iImp + 1
                                    ArrTipoDoc(iImp) = objDocLiq.Tipodoc
                                    ArrSerie(iImp) = objDocLiq.Serie
                                    ArrNumDoc(iImp) = objDocLiq.NumDoc
                                    ArrFilial(iImp) = objDocLiq.Filial
                                    ArrTipoEntidade(iImp) = objDocLiq.TipoEntidade
                                    ArrEntidade(iImp) = objDocLiq.Entidade

                                End If

                            End If

                            objDocLiq = Nothing

                        End If


                        'CASO NÃO EXISTAM PENDENTES PARA LIQUIDAR, ENTÃO É CRIADO UM DOCUMENTO DO TIPO ADIANTAMENTO NA CONTA DO CLIENTE
                        If dblValorRestante > 0 Then

                            lngNumDocADC = 0

                            'RMELRO:Alterada a data do documento Adiantamento de Now para a Data da Linha do ficheiro do banco

                            'If Not CriaDocumentoAdiantamento(strTipoentidade, strEntidade, strDocAdiantamento, Now, dblValorRestante, strReferenciaMB, strIdTransBMEPS, strFicheiroBMEPS, objAdiantamento) Then
                            If Not CriaDocumentoAdiantamento(strTipoentidade, strEntidade, strDocAdiantamento, dtDataMovBMEPS, dblValorRestante, strReferenciaMB, strIdTransBMEPS, strFicheiroBMEPS, objAdiantamento) Then

                                blnTeveErros = True

                            Else

                                If Not objAdiantamento Is Nothing Then

                                    If Not GravaLigacaoBancos(objAdiantamento.TipoEntidade, objAdiantamento.Entidade, objAdiantamento.Moeda, objAdiantamento.ValorTotal, objAdiantamento.DataDoc, objAdiantamento.IDHistorico, strInstituto, strReferenciaMB, True) Then

                                        blnTeveErros = True

                                    Else

                                        ' TextoCriacaoEmail = TextoCriacaoEmail & " - " & objAdiantamento.Tipodoc & " " & objAdiantamento.Serie & "/" & CStr(objAdiantamento.NumDocInt) & " no valor de " & objAdiantamento.ValorTotal & " " & objAdiantamento.Moeda & vbCrLf

                                    End If

                                End If

                            End If

                            objAdiantamento = Nothing

                        End If

                    Else

                        '   AdicionaTextoMsgFinal "Aviso: A referência '" & strReferenciaMB & "' já foi integrada noutro processamento."

                    End If

                Else

                    blnTeveErros = True
                    'AdicionaTextoMsgFinal "Erro: A referência '" & strReferenciaMB & "' não tem documentos pendentes associados."

                End If

            End If

        Next lngLinFile


        If dblTotalTaxas > 0 Then

            'RMELRO:Alterada a data do movimento bancário de Now para a Data da Linha do ficheiro do banco

            'If Not GravaDocumentoTesouraria(strDocTesouraria, strContaBancaria, strMovimentoBancarioCom, strRubrica, dblTotalTaxas, Now, strFicheiroBMEPS) Then
            If Not GravaDocumentoTesouraria(strDocTesouraria, strContaBancaria, strMovimentoBancarioCom, strRubrica, dblTotalTaxas, dtDataMovBMEPS, strFicheiroBMEPS, strReferenciaMB) Then

                blnTeveErros = True

            End If

        End If

        objFicheiroLinhas = Nothing
        objAdiantamento = Nothing
        objPendente = Nothing
        objDocLiq = Nothing

        ImportaFicheiroBanco = Not blnTeveErros

        If blnTeveErros Then
            ImportaFicheiroBanco = False
            objmotor.DesfazTransaccao()
            blnTrans = False

        Else

            objmotor.TerminaTransaccao()
            blnTrans = False

            'Enviar recibo fora da transação
            For I = 1 To iImp
                If ArrTipoDoc(I) <> "" Then
                    'EnviarRecibo(ArrTipoDoc(I), ArrSerie(I), ArrNumDoc(I), ArrFilial(I), ArrTipoEntidade(I), ArrEntidade(I), "Emissão de Recibo", "Foi emitido um documento de Recibo de Pagamento")
                End If
            Next I

        End If

        Exit Function

errHandler:

        If blnTrans = True Then objmotor.DesfazTransaccao()

        ImportaFicheiroBanco = False
        'AdicionaTextoMsgFinal "Error " & Err.Number & " (" & strPontoErro & " - " & Err.Description & ") in procedure ImportaFicheiroBanco of Módulo ModImpLiquidacoes"
        objFicheiroLinhas = Nothing
        objAdiantamento = Nothing
        objPendente = Nothing
        objDocLiq = Nothing

    End Function


    '---------------------------------------------------------------------------------------
    ' Procedure : CriaCabecalhoLiquidacao
    ' Autor     : ricardo.silva
    ' Data      : 31-10-2011
    ' Descrição :
    '---------------------------------------------------------------------------------------
    '

    Private Function CriaCabecalhoLiquidacao(TipoEntidade As String, Entidade As String, TipoDocLiq As String, Data As Date, ReferenciaMB As String, IdTransBMEPS As String, FicheiroBMEPS As String, Instituto As String) As GcpBE800.GcpBEDocumentoLiq

        Dim strCondPag As String

        Dim objDocLiquidacao As GcpBEDocumentoLiq

        Dim strContaBancaria As String
        Dim strMovBancario As String


        On Error GoTo Erro

        objDocLiquidacao = CreateObject("GcpBE800.GcpBEDocumentoLiq")

        With objDocLiquidacao


            .TipoEntidade = TipoEntidade
            .Entidade = Entidade

            .Tipodoc = TipoDocLiq
            .Serie = objmotor.Comercial.Series.DaSerieDefeito("M", TipoDocLiq, Data)

            .Moeda = objmotor.Contexto.MoedaBase
            .Utilizador = objmotor.Contexto.UtilizadorActual

            '.DataDoc = Data
            .DataIntroducao = Now


            .CamposUtil("CDU_Referencia") = ReferenciaMB
            .CamposUtil("CDU_IDTransBMEPS") = IdTransBMEPS
            .CamposUtil("CDU_FicheiroBMEPS") = FicheiroBMEPS

        End With

        'Preenche o objecto com os dados sugeridos para o documento
        objmotor.Comercial.Liquidacoes.PreencheDadosRelacionados(objDocLiquidacao)

        'RMELRO: Foi colocado aqui o preenchimento da DataDoc, pois quando são preenchidos os dados relacionados esta mesma data era subvertida

        objDocLiquidacao.DataDoc = Data

        strContaBancaria = Params.DaContaBancaria(Instituto)
        strMovBancario = Params.DaMovimentoBancario(Instituto)

        objDocLiquidacao.ModoPag = strMovBancario
        objDocLiquidacao.ContaBancaria = strContaBancaria


        CriaCabecalhoLiquidacao = objDocLiquidacao
        objDocLiquidacao = Nothing

        Exit Function

Erro:

        'AdicionaTextoMsgFinal "- Erro: " & Err.Number & " (" & Err.Description & ") in procedure CriaCabecalhoLiquidacao of Módulo ModImpLiquidacoes"
        objDocLiquidacao = Nothing
        CriaCabecalhoLiquidacao = Nothing

    End Function

    '---------------------------------------------------------------------------------------
    ' Procedure : AdiccionaPendente
    ' Autor     : ricardo.silva
    ' Data      : 24-07-2012
    ' Descrição :
    '---------------------------------------------------------------------------------------
    '
    Private Function AdiccionaPendente(ByRef DocLiquidacao As GcpBE800.GcpBEDocumentoLiq, ByRef Pendente As GcpBE800.GcpBEPendente, ValorLiq As Double) As Boolean

        Dim strEstadoLiq As String

        On Error GoTo Erro

        If Not DocLiquidacao Is Nothing And ValorLiq > 0 Then

            strEstadoLiq = objmotor.Comercial.TabVendas.DaValorAtributo(Pendente.TipoDoc, "Estado")

            'Adiciona o documento de compra ao documento de liquidação
            objmotor.Comercial.Liquidacoes.AdicionaLinha(DocLiquidacao, Pendente.Filial, Pendente.Modulo, Pendente.TipoDoc, Pendente.Serie, Pendente.NumDocInt, Pendente.NumPrestacao, strEstadoLiq, Pendente.NumTransferencia, ValorLiq)

            'Gera as retenções com base no resumo de retenções do pendente
            DocLiquidacao.RetencoesGerar = Nothing

            AdiccionaPendente = True

        End If
        AdiccionaPendente = False
        Exit Function

Erro:

        AdiccionaPendente = False
        'AdicionaTextoMsgFinal "- Erro: " & Err.Number & " (" & Err.Description & ") in procedure AdiccionaPendente of Módulo ModImporta"
        'Set objLinhaCompra = Nothing

    End Function



    '---------------------------------------------------------------------------------------
    ' Procedure : GravaDocumentoLiquidacao
    ' Autor     : ricardo.silva
    ' Data      : 07-08-2012
    ' Descrição :
    '---------------------------------------------------------------------------------------
    '
    Private Function GravaDocumentoLiquidacao(ByRef DocLiquidacao As GcpBE800.GcpBEDocumentoLiq) As Boolean

        Dim strErro As String
        Dim strAvisos As String

        On Error GoTo Erro

        GravaDocumentoLiquidacao = True

        'Actualiza a liquidação
        If objmotor.Comercial.Liquidacoes.ValidaActualizacao(DocLiquidacao, strErro) Then

            objmotor.Comercial.Liquidacoes.Actualiza(DocLiquidacao, strAvisos)


            'AdicionaTextoMsgFinal "- Doc. Liquidação criado com Sucesso (Entidade: " & DocLiquidacao.Entidade & ") - " & DocLiquidacao.TipoDoc & " " & DocLiquidacao.Serie & "\" & DocLiquidacao.NumDoc & ""

            'If strAvisos <> "" Then

            '    AdicionaTextoMsgFinal "- Aviso: " & strAvisos

            'End If

        Else

            'AdicionaTextoMsgFinal "- Erro ao gravar documento Liquidação: " & strErro
            GravaDocumentoLiquidacao = False

        End If

        Exit Function

Erro:

        'AdicionaTextoMsgFinal "- Erro ao gravar documento Liquidação: " & Err.Description
        DocLiquidacao = Nothing
        GravaDocumentoLiquidacao = False

    End Function

    Private Function GravaLigacaoBancos(TipoEntidade As String, Entidade As String, Moeda As String, Valor As Double, DataDoc As Date, IdDoc As String, Instituto As String, RefATM As String, Optional DocumentoAdiantamento As Boolean = False) As Boolean

        Dim strTipoDocTes As String
        Dim strSerieDocTes As String
        Dim strContaBancaria As String
        Dim strMovBancario As String
        Dim strErro As String
        Dim strAvisos As String


        Dim oDocTes As GcpBEDocumentoTesouraria
        oDocTes = New GcpBEDocumentoTesouraria

        On Error GoTo hErro

        GravaLigacaoBancos = True

        strTipoDocTes = Params.DaDocTesouraria(Instituto)
        strSerieDocTes = objmotor.Comercial.Series.DaSerieDefeito("B", strTipoDocTes)
        strContaBancaria = Params.DaContaBancaria(Instituto)
        strMovBancario = Params.DaMovimentoBancario(Instituto)


        'SE O DOCUMENTO DE LOGISTICA FOR UMA ADIANTAMENTO
        If DocumentoAdiantamento Then

            Valor = Math.Abs(Valor)

        End If


        With oDocTes
            .EmModoEdicao = False

            .ModuloOrigem = "M"
            .Filial = "000"
            .TipoLancamento = "000"
            .Tipodoc = strTipoDocTes
            .Serie = strSerieDocTes
            .Data = DataDoc
            .TipoEntidade = TipoEntidade
            .Entidade = Entidade

            .ContaOrigem = strContaBancaria

            .Moeda = Moeda
            .Cambio = objmotor.Contexto.MBaseCambioCompra
            .CambioMBase = objmotor.Contexto.MBaseCambioCompra
            .CambioMAlt = objmotor.Contexto.MAltCambioCompra

            .IdDocOrigem = IdDoc

            .AgrupaMovimentos = False

        End With

        objmotor.Comercial.Tesouraria.AdicionaLinha(oDocTes, strMovBancario, strContaBancaria, objmotor.Contexto.MoedaBase, Valor, TipoEntidade, Entidade)

        With oDocTes.Linhas(oDocTes.Linhas.NumItens)
            .DataMovimento = DataDoc
            .DataValor = .DataMovimento
            .Cambio = objmotor.Contexto.MBaseCambioCompra
            .CambioMBase = objmotor.Contexto.MBaseCambioCompra
            .CambioMAlt = objmotor.Contexto.MAltCambioCompra
            .Descricao = "Ref. ATM " & RefATM
        End With

        If objmotor.Comercial.Tesouraria.ValidaActualizacao(oDocTes, strErro) Then
            objmotor.Comercial.Tesouraria.Actualiza(oDocTes, strAvisos)

            'AdicionaTextoMsgFinal "- Doc. Tesouraria criado com Sucesso (Entidade: " & oDocTes.Entidade & ") - " & oDocTes.Tipodoc & " " & oDocTes.Serie & "\" & oDocTes.NumDoc & ""

            'If strAvisos <> "" Then
            '    AdicionaTextoMsgFinal "- Aviso: " & strAvisos
            'End If
        Else
            'AdicionaTextoMsgFinal "- Erro ao gravar documento tesouraria: " & strErro
            GravaLigacaoBancos = False
        End If

        Exit Function

hErro:
        'AdicionaTextoMsgFinal "- Erro ao gravar documento tesouraria: " & Err.Description
        oDocTes = Nothing
        GravaLigacaoBancos = False

    End Function


    '---------------------------------------------------------------------------------------
    ' Função    : CriaDocumentoAdiantamento
    ' Autor     : ricardo.silva
    ' Data      : 17-12-2012
    ' Objectivo :
    '---------------------------------------------------------------------------------------
    '
    Private Function CriaDocumentoAdiantamento(TipoEntidade As String, Entidade As String, TipoDoc As String, Data As Date, Valor As Double, ReferenciaMB As String, IdTransBMEPS As String, FicheiroBMEPS As String, ByRef DocAdiantamento As Object) As Boolean
        Dim objLinhaPendente As GCPBELinhaPendente

        Dim strAvisos As String
        Dim strErros As String
        Dim strCondPag As String

        On Error GoTo Erro

        CriaDocumentoAdiantamento = True


        DocAdiantamento = New GcpBEPendente
        objLinhaPendente = New GCPBELinhaPendente



        With DocAdiantamento
            .TipoDoc = TipoDoc
            .Serie = objmotor.Comercial.Series.DaSerieDefeito("M", TipoDoc, Data)

            .TipoEntidade = TipoEntidade
            .Entidade = Entidade

            .Moeda = objmotor.Contexto.MoedaBase
            .Utilizador = objmotor.Contexto.UtilizadorActual

            '.DataDoc = Data
            .DataIntroducao = Now

            '.IDHistorico = Plataforma.FuncoesGlobais.CriaGuid(True)

            objmotor.Comercial.Pendentes.PreencheDadosRelacionados(DocAdiantamento, 5) ' 5 - todos os dados

            'RMELRO:Foi colocado aqui o preenchimento da DataDoc, pois quando são preenchidos os dados relacionados esta mesma data era subvertida

            DocAdiantamento.DataDoc = Data

            If .DataVenc = "00:00:00" Then

                .DataVenc = .DataDoc

            End If

        End With

        With objLinhaPendente
            .Descricao = "Ref. Banc.: " & ReferenciaMB
            .Incidencia = Valor
            .Total = .Incidencia
            .PercIvaDedutivel = 100
        End With


        DocAdiantamento.ValorTotal = objLinhaPendente.Incidencia
        DocAdiantamento.TotalIva = objLinhaPendente.ValorIva
        DocAdiantamento.ValorPendente = objLinhaPendente.Incidencia


        DocAdiantamento.CamposUtil("CDU_Referencia") = ReferenciaMB
        DocAdiantamento.CamposUtil("CDU_IDTransBMEPS") = IdTransBMEPS
        DocAdiantamento.CamposUtil("CDU_FicheiroBMEPS") = FicheiroBMEPS


        If objmotor.Comercial.Pendentes.ValidaActualizacao(DocAdiantamento, strErros) Then

            objmotor.Comercial.Pendentes.Actualiza(DocAdiantamento, strAvisos)

            'AdicionaTextoMsgFinal "- Doc. Adiantamento criado com Sucesso (Entidade: " & DocAdiantamento.Entidade & ") - " & DocAdiantamento.TipoDoc & " " & DocAdiantamento.Serie & "\" & DocAdiantamento.NumDoc & ""

            If strAvisos <> "" Then

                'AdicionaTextoMsgFinal "- Aviso: " & strAvisos

            End If

        Else

            'AdicionaTextoMsgFinal "- Erro ao gravar Documento Adiantamento: " & strErros
            CriaDocumentoAdiantamento = False

        End If

        Exit Function

Erro:

        'AdicionaTextoMsgFinal "- Erro ao gravar Documento Adiantamento: " & Err.Description
        DocAdiantamento = Nothing
        CriaDocumentoAdiantamento = False

    End Function

    '---------------------------------------------------------------------------------------
    ' Procedure : GravaDocumentoTesouraria
    ' Autor     : ricardo.silva
    ' Data      : 08-08-2012
    ' Descrição :
    '---------------------------------------------------------------------------------------
    '
    Private Function GravaDocumentoTesouraria(DocumentoTesouraria As String, ByVal ContaBancaria As String, MovimentoBancario As String, Rubrica As String, Valor As Double, Data As Date, FicheiroBMEPS As String, RefATM As String) As Boolean

        Dim objDocTesouraria As GcpBEDocumentoTesouraria
        Dim lstLista As StdBELista

        Dim dblValor As Double
        Dim strErro As String
        Dim strAvisos As String
        Dim strSQL As String

        On Error GoTo errHandler

        GravaDocumentoTesouraria = True

        'VERIFICA SE AS TAXAS DO FICHEIRO JÁ FORAM INTEGRADAS
        strSQL = "SELECT TIPODOC, SERIE, NUMDOC, CONTAORIGEM FROM CabecTesouraria WHERE CDU_FicheiroBMEPS = '" & FicheiroBMEPS & "'"

        lstLista = objmotor.Consulta(strSQL)

        'SE JÁ INTEGROU FICHEIRO, ENTÃO NÃO VOLTA A INTEGRAR
        If lstLista.NumLinhas > 0 Then

            lstLista.Inicio()
            'AdicionaTextoMsgFinal "Aviso: Doc. Tesouraria com Taxas já existia (Conta: " & lstLista("CONTAORIGEM") & ") - " & lstLista("TipoDoc") & " " & lstLista("SERIE") & "/" & lstLista("NUMDOC") & ""
            Exit Function

        End If


        'SE NÃO EXISTE DOCUMENTO, ENTÃO CRIA NOVO DOCUMENTO

        objDocTesouraria = New GcpBEDocumentoTesouraria


        With objDocTesouraria
            .EmModoEdicao = False

            .TipoLancamento = "000"
            .ModuloOrigem = "B"
            .Filial = "000"
            .Tipodoc = DocumentoTesouraria
            .Serie = objmotor.Comercial.Series.DaSerieDefeito("B", DocumentoTesouraria, Data)
            .Data = Data
            .ContaOrigem = ContaBancaria

            .Moeda = objmotor.Contexto.MoedaBase
            .Cambio = objmotor.Contexto.MBaseCambioCompra
            .CambioMBase = objmotor.Contexto.MBaseCambioCompra
            .CambioMAlt = objmotor.Contexto.MAltCambioCompra

            .CamposUtil("CDU_FicheiroBMEPS") = FicheiroBMEPS

            .Observacoes = "Criado na integração do Ficheiro BMEPS. " & vbCrLf & "Taxas do ficheiro '" & FicheiroBMEPS & "'."

            .AgrupaMovimentos = False

        End With

        'Linha
        objmotor.Comercial.Tesouraria.AdicionaLinha(objDocTesouraria, MovimentoBancario, ContaBancaria, objDocTesouraria.Moeda, Valor, , , Rubrica)

        With objDocTesouraria.Linhas(objDocTesouraria.Linhas.NumItens)

            .DataMovimento = objDocTesouraria.Data
            .DataValor = objDocTesouraria.Data
            .Cambio = objDocTesouraria.Cambio
            .CambioMBase = objDocTesouraria.CambioMBase
            .CambioMAlt = objDocTesouraria.CambioMAlt
            .Descricao = "Ref. ATM" & RefATM

        End With


        If objmotor.Comercial.Tesouraria.ValidaActualizacao(objDocTesouraria, strErro) Then

            objmotor.Comercial.Tesouraria.Actualiza(objDocTesouraria, strAvisos)

            'AdicionaTextoMsgFinal "- Doc. Tesouraria criado com Sucesso (Taxas BMEPS) - " & objDocTesouraria.Tipodoc & " " & objDocTesouraria.Serie & "/" & CStr(objDocTesouraria.NumDoc)

            If strAvisos <> "" Then

                ' AdicionaTextoMsgFinal "- Aviso: " & strAvisos

            End If

        Else

            GravaDocumentoTesouraria = False
            'AdicionaTextoMsgFinal "- Erro ao gravar documento Tesouraria (Taxas BMEPS): " & strErro

        End If

        objDocTesouraria = Nothing

        Exit Function

errHandler:

        'AdicionaTextoMsgFinal "- Erro ao gravar documento Tesouraria (Taxas BMEPS): " & Err.Description
        objDocTesouraria = Nothing
        GravaDocumentoTesouraria = False

    End Function



    '---------------------------------------------------------------------------------------
    ' Procedure : ConfiguracaoMovimentoBancContrario
    ' Autor     : ricardo.silva
    ' Data      : 02-05-2012
    ' Descrição :
    '---------------------------------------------------------------------------------------
    '
    Public Function ConfiguracaoMovimentoBancContrario(MovBanc As String) As String

        Dim objLista As StdBELista

        On Error GoTo errHandler

        ConfiguracaoMovimentoBancContrario = ""

        If MovBanc <> "" Then
            objLista = objmotor.Consulta("SELECT TOP 1 MovimNatInversa " & vbCrLf & _
                                           "FROM dbo.DocumentosBancos " & vbCrLf & _
                                           "WHERE Movim = '" & UCase(MovBanc) & "'")

            If Not objLista.Vazia Then

                objLista.Inicio()
                ConfiguracaoMovimentoBancContrario = objLista.Valor("MovimNatInversa")

            End If

        End If

        Exit Function

errHandler:


        'AdicionaTextoMsgFinal "- Erro carregar configuração bancária: " & Err.Description
        objLista = Nothing
        ConfiguracaoMovimentoBancContrario = ""

    End Function


    Private Function ValidaFicheiroHistorico(ByVal strNomeFicheiroSimples As String) As Boolean
        Dim strSQL As String
        Dim lstLista As Object

        On Error GoTo Erro

        If Len(strNomeFicheiroSimples) <> 33 Then
            ValidaFicheiroHistorico = False
            'AdicionaTextoMsgFinal "Aviso: O ficheiro '" & strNomeFicheiroSimples & "' não está com o nome correcto(ex:BMEPS_XXX00000000000000000000.TXT)!"
            Exit Function
        End If


        'SELECCTIONA numero de linhas com referencia ao ficheiro a importar!
        strSQL = "SELECT Top 1 DataDoc, Count(*) as NumRecords FROM HISTORICO" & vbCrLf & _
                 "WHERE CDU_FicheiroBMEPS = '" & strNomeFicheiroSimples & "' GROUP BY DataDoc" & vbCrLf & _
                 "UNION ALL" & vbCrLf & _
                 "SELECT Top 1 DataDoc, Count(*) as NumRecords FROM CABLIQ" & vbCrLf & _
                 "WHERE CDU_FicheiroBMEPS = '" & strNomeFicheiroSimples & "' GROUP BY DataDoc"


        lstLista = objmotor.Consulta(strSQL)

        'VERIFICA SE EXISTE ALGUM DOCUMENTO COM A REFERÊNCIA INDICADA
        If lstLista.NumLinhas > 0 Then

            If lstLista("NumRecords") > 0 Then
                ValidaFicheiroHistorico = False
                'AdicionaTextoMsgFinal "Aviso: O ficheiro '" & strNomeFicheiroSimples & "' já foi integrado noutro processamento(" & Format(lstLista("DataDoc"), "DD-MM-YYYY") & ")!"
                lstLista = Nothing
                Exit Function
            End If
        End If
        lstLista = Nothing

        ValidaFicheiroHistorico = True

        Exit Function

Erro:

        ValidaFicheiroHistorico = False
        'AdicionaTextoMsgFinal "- Erro: " & Err.Number & " (" & Err.Description & ") in procedure AdiccionaPendente of Módulo ModImporta"
        'Set objLinhaCompra = Nothing

    End Function

#End Region

#Region "Constantes"
    Option Explicit


    Public Function DEFVALUE_DATA() As Object
        DEFVALUE_DATA = Empty
    End Function
    Public Function DEFVALUE_SINGLE() As Object
        DEFVALUE_SINGLE = CSng(0)
    End Function
    Public Function DEFVALUE_DOUBLE() As Object
        DEFVALUE_DOUBLE = CDbl(0)
    End Function
    Public Function DEFVALUE_STRING() As Object
        DEFVALUE_STRING = vbNullString
    End Function
    Public Function DEFVALUE_INTEGER() As Object
        DEFVALUE_INTEGER = CInt(0)
    End Function
    Public Function DEFVALUE_LONG() As Object
        DEFVALUE_LONG = CLng(0)
    End Function
    Public Function DEFVALUE_BOOLEAN() As Object
        DEFVALUE_BOOLEAN = False
    End Function
    Public Function DEFVALUE_BYTE() As Object
        DEFVALUE_BYTE = CByte(0)
    End Function
    Public Function DEFVALUE_CURRENCY() As Object
        DEFVALUE_CURRENCY = CDbl(0)
    End Function

    Public Function DEFVALUE(ByVal vbType As VariantType) As Object
        Select Case vbType
            Case vbString : DEFVALUE = DEFVALUE_STRING()
            Case vbInteger : DEFVALUE = DEFVALUE_INTEGER()
            Case vbLong : DEFVALUE = DEFVALUE_LONG()
            Case vbDate : DEFVALUE = DEFVALUE_DATA()
            Case vbBoolean : DEFVALUE = DEFVALUE_BOOLEAN()
            Case vbByte : DEFVALUE = DEFVALUE_BYTE()
            Case vbSingle : DEFVALUE = DEFVALUE_SINGLE()
            Case vbDouble : DEFVALUE = DEFVALUE_DOUBLE()
            Case vbCurrency : DEFVALUE = DEFVALUE_CURRENCY()
            Case Else : DEFVALUE = DEFVALUE_STRING()
        End Select
    End Function


    Public Function FData(ByVal varVal As Object) As Date
        Select Case varType(varVal)
            
            Case vbDate
                FData = varVal
            Case vbString, vbDouble
                If IsDate(varVal) Then FData = CDate(varVal) Else FData = Empty
            Case vbByte, vbInteger, vbLong, vbSingle, vbCurrency, vbBoolean, vbDecimal
                FData = DEFVALUE_DATA
            Case Else
                FData = DEFVALUE_DATA
        End Select
    End Function
    Public Function FDataDef(ByVal varVal As Object, ByVal datDefault As Date) As Date
        If IsNothing(varVal) Or Not IsDate(varVal) Then
            FDataDef = datDefault
        Else
            FDataDef = FData(varVal)
        End If
    End Function

    Public Function FSng(ByVal varVal As Object) As Single
        'type-declaration character: !
        Select Case varType(varVal)
             
            Case vbString
                If IsNumeric(varVal) Then FSng = CSng(varVal) Else FSng = DEFVALUE_SINGLE
            Case vbDouble, vbByte, vbInteger, vbLong, vbSingle, vbCurrency, vbBoolean, vbDecimal
                FSng = CSng(varVal)
            Case Else
                FSng = DEFVALUE_SINGLE
        End Select
    End Function
    Public Function FSngDef(ByVal varVal As Object, ByVal SngDefault As Single) As Single
        If IsNothing(varVal) Or Not IsNumeric(varVal) Then
            FSngDef = SngDefault
        Else
            FSngDef = FSng(varVal)
        End If
    End Function

    Public Function FDbl(ByVal varVal As Object) As Double
        'type-declaration character: #
        Select Case varType(varVal)
            
            Case vbString
                If IsNumeric(varVal) Then FDbl = CDbl(varVal) Else FDbl = DEFVALUE_DOUBLE
            Case vbDouble, vbByte, vbInteger, vbLong, vbSingle, vbCurrency, vbBoolean, vbDecimal
                FDbl = CDbl(varVal)
            Case Else
                FDbl = DEFVALUE_DOUBLE
        End Select
    End Function
    Public Function FDblDef(ByVal varVal As Object, ByVal dblDefault As Double) As Double
        If IsNothing(varVal) Or Not IsNumeric(varVal) Then
            FDblDef = dblDefault
        Else
            FDblDef = FDbl(varVal)
        End If
    End Function

    Public Function FStr(ByVal varVal As Object) As String
        'type-declaration character: $
        varVal = "" & varVal
        Select Case True
            Case IsError(varVal), IsNothing(varVal), IsNothing(varVal)
                FStr = DEFVALUE_STRING()
            Case (Len(Trim$(varVal)) = 0)
                FStr = DEFVALUE_STRING
            Case Else
                FStr = Trim$(CStr(varVal))
        End Select
    End Function
    Public Function FStrDef(ByVal varVal As Object, ByVal strDefault As String) As String
        If IsNothing(varVal) Then
            FStrDef = strDefault
        Else
            FStrDef = FStr(varVal)
        End If
    End Function

    Public Function FInt(ByVal varVal As Object) As Integer
        'type-declaration character: %
        Select Case varType(varVal)

            Case vbString
                If IsNumeric(varVal) Then FInt = CInt(varVal) Else FInt = DEFVALUE_INTEGER
            Case vbInteger, vbByte, vbLong, vbSingle, vbDouble, vbCurrency, vbBoolean, vbDecimal
                If ((varVal >= -32768) And (varVal <= 32767)) Then FInt = CInt(varVal) Else FInt = DEFVALUE_INTEGER
            Case Else
                FInt = DEFVALUE_INTEGER
        End Select
    End Function
    Public Function FIntDef(ByVal varVal As Object, ByVal intDefault As Integer) As Integer
        If IsNothing(varVal) Or Not IsNumeric(varVal) Then
            FIntDef = intDefault
        Else
            FIntDef = FInt(varVal)
        End If
    End Function

    Public Function FLng(ByVal varVal As Object) As Long
        'type-declaration character: &
        Select Case varType(varVal)
            
            Case vbString
                If IsNumeric(varVal) Then FLng = CLng(varVal) Else FLng = DEFVALUE_LONG
            Case vbByte, vbInteger, vbLong, vbBoolean
                FLng = CLng(varVal)
            Case vbSingle, vbDouble, vbCurrency, vbDecimal
                If (varVal <= 2147483647) Then FLng = CLng(varVal) Else FLng = DEFVALUE_LONG
            Case Else
                FLng = DEFVALUE_LONG
        End Select
    End Function
    Public Function FLngDef(ByVal varVal As Object, ByVal lngDefault As Long) As Long
        If IsNothing(varVal) Or Not IsNumeric(varVal) Then
            FLngDef = lngDefault
        Else
            FLngDef = FLng(varVal)
        End If
    End Function

    Public Function FBool(ByVal varVal As Object) As Boolean
        Select Case varType(varVal)
            
            Case vbString
                If IsNumeric(varVal) Then FBool = CBool(varVal) Else FBool = DEFVALUE_BOOLEAN
            Case vbBoolean, vbByte, vbInteger, vbLong
                FBool = CBool(varVal)
            Case vbSingle, vbDouble, vbCurrency, vbDecimal
                FBool = IIf((varVal Mod 1) = 0, CBool(varVal \ 1), DEFVALUE_BOOLEAN)
            Case Else
                FBool = DEFVALUE_BOOLEAN
        End Select
    End Function
    Public Function FBoolDef(ByVal varVal As Object, ByVal blnDefault As Boolean) As Boolean
        If IsNothing(varVal) Then
            FBoolDef = blnDefault
        Else
            FBoolDef = FBool(varVal)
        End If
    End Function

    Public Function FByt(ByVal varVal As Object) As Byte
        Select Case varType(varVal)
            
            Case vbString
                If IsNumeric(varVal) Then FByt = CByte(varVal) Else FByt = DEFVALUE_BYTE
            Case vbByte, vbInteger, vbLong, vbBoolean
                FByt = CByte(varVal)
            Case vbSingle, vbDouble, vbCurrency, vbDecimal
                If (varVal <= 255) Then FByt = CByte(varVal) Else FByt = DEFVALUE_BYTE
            Case Else
                FByt = DEFVALUE_BYTE
        End Select
    End Function
    Public Function FBytDef(ByVal varVal As Object, ByVal byDefault As Byte) As Byte
        If IsNothing(varVal) Or Not IsNumeric(varVal) Then
            FBytDef = byDefault
        Else
            FBytDef = FByt(varVal)
        End If
    End Function


    Public Function FCur(ByVal varVal As Object) As Currency
        Select Case varType(varVal)
            
            Case vbString
                If IsNumeric(varVal) Then FCur = CCur(varVal) Else FCur = DEFVALUE_CURRENCY
            Case vbCurrency, vbByte, vbInteger, vbLong, vbSingle, vbDouble, vbCurrency, vbDecimal
                FCur = CCur(varVal)
            Case Else
                FCur = DEFVALUE_CURRENCY
        End Select
    End Function
    Public Function FCurDef(ByVal varVal As Object, ByVal blnDefault As Currency) As Currency
        If IsNothing(varVal) Then
            FCurDef = blnDefault
        Else
            FCurDef = FCur(varVal)
        End If
    End Function




    Public Function FGeneric(ByVal varVal As Object, ByVal vbType As VBA.VbVarType) As Object
        Select Case vbType
            Case vbString
                FGeneric = FStr(varVal)
            Case vbCurrency, vbDouble, vbSingle, vbDecimal
                FGeneric = FDbl(varVal)
            Case vbDate
                FGeneric = FData(varVal)
            Case vbBoolean
                FGeneric = FBool(varVal)
            Case vbByte
                FGeneric = FByt(varVal)
            Case vbInteger
                FGeneric = FInt(varVal)
            Case vbLong
                FGeneric = FLng(varVal)
            Case Else
                FGeneric = FStr(varVal)
        End Select
    End Function

    Public Function FGenericDef(ByVal varVal As Object, ByVal varDefault As Object, ByVal vbType As VBA.VbVarType) As Object
        Select Case vbType
            Case vbString
                FGenericDef = FStrDef(varVal, varDefault)
            Case vbCurrency, vbDouble, vbSingle, vbDecimal
                FGenericDef = FDblDef(varVal, varDefault)
            Case vbDate
                FGenericDef = FDataDef(varVal, varDefault)
            Case vbBoolean
                FGenericDef = FBoolDef(varVal, varDefault)
            Case vbByte
                FGenericDef = FBytDef(varVal, varDefault)
            Case vbInteger
                FGenericDef = FIntDef(varVal, varDefault)
            Case vbLong
                FGenericDef = FLngDef(varVal, varDefault)
            Case Else
                FGenericDef = FStrDef(varVal, varDefault)
        End Select
    End Function


    '--------------------------------------------------------------------------------------
    ' GET Data from ADO TYPE
    '--------------------------------------------------------------------------------------


#If DEV = 1 Then
Public Function GetAdoTypeName(ByVal eAdoType As ADODB.DataTypeEnum) As String

    Select Case eAdoType
        Case adWChar
             GetAdoTypeName = "adWChar"
        Case adVarWChar
             GetAdoTypeName = "adVarWChar"
        Case adVarNumeric
             GetAdoTypeName = "adVarNumeric"
        Case adVariant
             GetAdoTypeName = "adVariant"
        Case adVarChar
             GetAdoTypeName = "adVarChar"
        Case adVarBinary
             GetAdoTypeName = "adVarBinary"
        Case adUserDefined
             GetAdoTypeName = "adUserDefined"
        Case adUnsignedTinyInt
             GetAdoTypeName = "adUnsignedTinyInt"
        Case adUnsignedSmallInt
             GetAdoTypeName = "adUnsignedSmallInt"
        Case adUnsignedInt
             GetAdoTypeName = "adUnsignedInt"
        Case adUnsignedBigInt
             GetAdoTypeName = "adUnsignedBigInt"
        Case adTinyInt
             GetAdoTypeName = "adTinyInt"
        Case adSmallInt
             GetAdoTypeName = "adSmallInt"
        Case adSingle
             GetAdoTypeName = "adSingle"
        Case adPropVariant
             GetAdoTypeName = "adPropVariant"
        Case adNumeric
             GetAdoTypeName = "adNumeric"
        Case adLongVarWChar
             GetAdoTypeName = "adLongVarWChar"
        Case adLongVarChar
             GetAdoTypeName = "adLongVarChar"
        Case adLongVarBinary
             GetAdoTypeName = "adLongVarBinary"
        Case adIUnknown
             GetAdoTypeName = "adIUnknown"
        Case adInteger
             GetAdoTypeName = "adInteger"
        Case adIDispatch
             GetAdoTypeName = "adIDispatch"
        Case adGUID
             GetAdoTypeName = "adGUID"
        Case adFileTime
             GetAdoTypeName = "adFileTime"
        Case adError
             GetAdoTypeName = "adError"
        Case adEmpty
             GetAdoTypeName = "adEmpty"
        Case adDouble
             GetAdoTypeName = "adDouble"
        Case adDecimal
             GetAdoTypeName = "adDecimal"
        Case adDBTimeStamp
             GetAdoTypeName = "adDBTimeStamp"
        Case adDBTime
             GetAdoTypeName = "adDBTime"
        Case adDBDate
             GetAdoTypeName = "adDBDate"
        Case adDate
             GetAdoTypeName = "adDate"
        Case adCurrency
             GetAdoTypeName = "adCurrency"
        Case adChar
             GetAdoTypeName = "adChar"
        Case adChapter
             GetAdoTypeName = "adChapter"
        Case adBSTR
             GetAdoTypeName = "adBSTR"
        Case adBoolean
             GetAdoTypeName = "adBoolean"
        Case adBinary
             GetAdoTypeName = "adBinary"
        Case adBigInt
             GetAdoTypeName = "adBigInt"
        Case adArray
             GetAdoTypeName = "adArray"
        Case Else
             GetAdoTypeName = "Not a Type"
    End Select
End Function

#Else

    Public Function GetAdoTypeName(ByVal eAdoType As Long) As String

        Select Case eAdoType
            Case 130 ' adWChar
                GetAdoTypeName = "adWChar"
            Case 202 ' adVarWChar
                GetAdoTypeName = "adVarWChar"
            Case 139 ' adVarNumeric
                GetAdoTypeName = "adVarNumeric"
            Case 12 ' adVariant
                GetAdoTypeName = "adVariant"
            Case 200 ' adVarChar
                GetAdoTypeName = "adVarChar"
            Case 204 ' adVarBinary
                GetAdoTypeName = "adVarBinary"
            Case 132 ' adUserDefined
                GetAdoTypeName = "adUserDefined"
            Case 17 ' adUnsignedTinyInt
                GetAdoTypeName = "adUnsignedTinyInt"
            Case 18 ' adUnsignedSmallInt
                GetAdoTypeName = "adUnsignedSmallInt"
            Case 19 ' adUnsignedInt
                GetAdoTypeName = "adUnsignedInt"
            Case 21 ' adUnsignedBigInt
                GetAdoTypeName = "adUnsignedBigInt"
            Case 16 ' adTinyInt
                GetAdoTypeName = "adTinyInt"
            Case 2 ' adSmallInt
                GetAdoTypeName = "adSmallInt"
            Case 4 ' adSingle
                GetAdoTypeName = "adSingle"
            Case 138 ' adPropVariant
                GetAdoTypeName = "adPropVariant"
            Case 131 ' adNumeric
                GetAdoTypeName = "adNumeric"
            Case 203 ' adLongVarWChar
                GetAdoTypeName = "adLongVarWChar"
            Case 201 ' adLongVarChar
                GetAdoTypeName = "adLongVarChar"
            Case 205 ' adLongVarBinary
                GetAdoTypeName = "adLongVarBinary"
            Case 13 ' adIUnknown
                GetAdoTypeName = "adIUnknown"
            Case 3 ' adInteger
                GetAdoTypeName = "adInteger"
            Case 9 ' adIDispatch
                GetAdoTypeName = "adIDispatch"
            Case 72 ' adGUID
                GetAdoTypeName = "adGUID"
            Case 64 ' adFileTime
                GetAdoTypeName = "adFileTime"
            Case 10 ' adError
                GetAdoTypeName = "adError"
            Case 0 ' adEmpty
                GetAdoTypeName = "adEmpty"
            Case 5 ' adDouble
                GetAdoTypeName = "adDouble"
            Case 14 ' adDecimal
                GetAdoTypeName = "adDecimal"
            Case 135 ' adDBTimeStamp
                GetAdoTypeName = "adDBTimeStamp"
            Case 134 ' adDBTime
                GetAdoTypeName = "adDBTime"
            Case 133 ' adDBDate
                GetAdoTypeName = "adDBDate"
            Case 7 ' adDate
                GetAdoTypeName = "adDate"
            Case 6 ' adCurrency
                GetAdoTypeName = "adCurrency"
            Case 129 ' adChar
                GetAdoTypeName = "adChar"
            Case 136 ' adChapter
                GetAdoTypeName = "adChapter"
            Case 8 ' adBSTR
                GetAdoTypeName = "adBSTR"
            Case 11 ' adBoolean
                GetAdoTypeName = "adBoolean"
            Case 128 ' adBinary
                GetAdoTypeName = "adBinary"
            Case 20 ' adBigInt
                GetAdoTypeName = "adBigInt"
            Case 8192 ' adArray
                GetAdoTypeName = "adArray"
            Case Else
                GetAdoTypeName = "Not a Type"
        End Select
    End Function
#End If

#If DEV = 1 Then
Public Function CastAdoType2VBType(ByVal eAdoType As ADODB.DataTypeEnum) As VBA.VbVarType

    Select Case eAdoType
        Case adWChar
             CastAdoType2VBType = VariantType.String
        Case adVarWChar
             CastAdoType2VBType = VariantType.String
        Case adVarNumeric
             CastAdoType2VBType = VariantType.Long
        Case adVariant
             CastAdoType2VBType = VariantType.Variant
        Case adVarChar
             CastAdoType2VBType = VariantType.String
        Case adVarBinary
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adUserDefined
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adUnsignedTinyInt
             CastAdoType2VBType = VariantType.Byte
        Case adUnsignedSmallInt
             CastAdoType2VBType = VariantType.Integer
        Case adUnsignedInt
             CastAdoType2VBType = VariantType.Long
        Case adUnsignedBigInt
             CastAdoType2VBType = VariantType.Long
        Case adTinyInt
             CastAdoType2VBType = VariantType.Byte
        Case adSmallInt
             CastAdoType2VBType = VariantType.Integer
        Case adSingle
             CastAdoType2VBType = VariantType.Double
        Case adPropVariant
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adNumeric
             CastAdoType2VBType = VariantType.Double
        Case adLongVarWChar
             CastAdoType2VBType = VariantType.String
        Case adLongVarChar
             CastAdoType2VBType = VariantType.String
        Case adLongVarBinary
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adIUnknown
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adInteger
             CastAdoType2VBType = VariantType.Long
        Case adIDispatch
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adGUID
             CastAdoType2VBType = VariantType.String
        Case adFileTime
             CastAdoType2VBType = VariantType.Date
        Case adError
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adEmpty
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adDouble
             CastAdoType2VBType = VariantType.Double
        Case adDecimal
             CastAdoType2VBType = VariantType.Decimal
        Case adDBTimeStamp
             CastAdoType2VBType = VariantType.Date
        Case adDBTime
             CastAdoType2VBType = VariantType.Date
        Case adDBDate
             CastAdoType2VBType = VariantType.Date
        Case adDate
             CastAdoType2VBType = VariantType.Date
        Case adCurrency
             CastAdoType2VBType = VariantType.Currency
        Case adChar
             CastAdoType2VBType = VariantType.String
        Case adChapter
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adBSTR
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adBoolean
             CastAdoType2VBType = VariantType.Boolean
        Case adBinary
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case adBigInt
             CastAdoType2VBType = VariantType.Long
        Case adArray
             CastAdoType2VBType = VariantType.Variant ' should be some other type
        Case Else
             CastAdoType2VBType = VariantType.Variant ' should be some other type
    End Select
End Function

#Else

    Public Function CastAdoType2VBType(ByVal eAdoType As Long) As VariantType

        Select Case eAdoType
            Case 130 'adWChar
                CastAdoType2VBType = VariantType.String
            Case 202 ' adVarWChar
                CastAdoType2VBType = VariantType.String
            Case 139 ' adVarNumeric
                CastAdoType2VBType = VariantType.Long
            Case 12 ' adVariant
                CastAdoType2VBType = VariantType.Variant
            Case 200 'adVarChar
                CastAdoType2VBType = VariantType.String
            Case 204 ' adVarBinary
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 132 ' adUserDefined
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 17 ' adUnsignedTinyInt
                CastAdoType2VBType = VariantType.Byte
            Case 18 ' adUnsignedSmallInt
                CastAdoType2VBType = VariantType.Integer
            Case 19 ' adUnsignedInt
                CastAdoType2VBType = VariantType.Long
            Case 21 ' adUnsignedBigInt
                CastAdoType2VBType = VariantType.Long
            Case 16 ' adTinyInt
                CastAdoType2VBType = VariantType.Byte
            Case 2 ' adSmallInt
                CastAdoType2VBType = VariantType.Integer
            Case 4 ' adSingle
                CastAdoType2VBType = VariantType.Double
            Case 138 ' adPropVariant
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 131 ' adNumeric
                CastAdoType2VBType = VariantType.Double
            Case 203 ' adLongVarWChar
                CastAdoType2VBType = VariantType.String
            Case 201 ' adLongVarChar
                CastAdoType2VBType = VariantType.String
            Case 205 ' adLongVarBinary
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 13 ' adIUnknown
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 3 ' adInteger
                CastAdoType2VBType = VariantType.Integer
            Case 9 ' adIDispatch
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 72 ' adGUID
                CastAdoType2VBType = VariantType.String
            Case 64 ' adFileTime
                CastAdoType2VBType = VariantType.Date
            Case 10 ' adError
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 0 ' adEmpty
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 5 ' adDouble
                CastAdoType2VBType = VariantType.Double
            Case 14 ' adDecimal
                CastAdoType2VBType = VariantType.Decimal
            Case 135 ' adDBTimeStamp
                CastAdoType2VBType = VariantType.Date
            Case 134 ' adDBTime
                CastAdoType2VBType = VariantType.Date
            Case 133 ' adDBDate
                CastAdoType2VBType = VariantType.Date
            Case 7 ' adDate
                CastAdoType2VBType = VariantType.Date
            Case 6 ' adCurrency
                CastAdoType2VBType = VariantType.Currency
            Case 129 ' adChar
                CastAdoType2VBType = VariantType.String
            Case 136 ' adChapter
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 8 ' adBSTR
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 11 ' adBoolean
                CastAdoType2VBType = VariantType.Boolean
            Case 128 ' adBinary
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case 20 ' adBigInt
                CastAdoType2VBType = VariantType.Long
            Case 8192 ' adArray
                CastAdoType2VBType = VariantType.Variant ' should be some other type
            Case Else
                CastAdoType2VBType = VariantType.Variant ' should be some other type
        End Select
    End Function
#End If

#If DEV = 1 Then
Public Function CastAdoValue2String(ByVal eAdoType As ADODB.DataTypeEnum, ByVal varVal As Variant) As String

    Select Case eAdoType
        Case adWChar
             CastAdoValue2String = CStr(varVal)
        Case adVarWChar
             CastAdoValue2String = CStr(varVal)
        Case adVarNumeric
             CastAdoValue2String = CStr(varVal)
        Case adVariant
             CastAdoValue2String = "N/A"
        Case adVarChar
             CastAdoValue2String = CStr(varVal)
        Case adVarBinary
             CastAdoValue2String = "N/A"
        Case adUserDefined
             CastAdoValue2String = "N/A"
        Case adUnsignedTinyInt
             CastAdoValue2String = CStr(varVal)
        Case adUnsignedSmallInt
             CastAdoValue2String = CStr(varVal)
        Case adUnsignedInt
             CastAdoValue2String = CStr(varVal)
        Case adUnsignedBigInt
             CastAdoValue2String = CStr(varVal)
        Case adTinyInt
             CastAdoValue2String = CStr(varVal)
        Case adSmallInt
             CastAdoValue2String = CStr(varVal)
        Case adSingle
             CastAdoValue2String = CStr(varVal)
        Case adPropVariant
             CastAdoValue2String = "N/A"
        Case adNumeric
             CastAdoValue2String = CStr(varVal)
        Case adLongVarWChar
             CastAdoValue2String = "N/A"
        Case adLongVarChar
             CastAdoValue2String = "N/A"
        Case adLongVarBinary
             CastAdoValue2String = "N/A"
        Case adIUnknown
             CastAdoValue2String = "N/A"
        Case adInteger
             CastAdoValue2String = CStr(varVal)
        Case adIDispatch
             CastAdoValue2String = "N/A"
        Case adGUID
             CastAdoValue2String = CStr(varVal)
        Case adFileTime
             CastAdoValue2String = "N/A"
        Case adError
             CastAdoValue2String = "N/A"
        Case adEmpty
             CastAdoValue2String = "N/A"
        Case adDouble
             CastAdoValue2String = CStr(varVal)
        Case adDecimal
             CastAdoValue2String = CStr(varVal)
        Case adDBTimeStamp
             CastAdoValue2String = "N/A"
        Case adDBTime
             CastAdoValue2String = CStr(varVal)
        Case adDBDate
             CastAdoValue2String = CStr(varVal)
        Case adDate
             CastAdoValue2String = CStr(varVal)
        Case adCurrency
             CastAdoValue2String = CStr(varVal)
        Case adChar
             CastAdoValue2String = CStr(varVal)
        Case adChapter
             CastAdoValue2String = "N/A"
        Case adBSTR
             CastAdoValue2String = "N/A"
        Case adBoolean
             CastAdoValue2String = IIf(CStr(varVal) = "True", 1, 0)
        Case adBinary
             CastAdoValue2String = "N/A"
        Case adBigInt
             CastAdoValue2String = CStr(varVal)
        Case adArray
             CastAdoValue2String = "N/A"
        Case Else
             CastAdoValue2String = "N/A"
    End Select
End Function

#Else

    Public Function CastAdoValue2String(ByVal eAdoType As Long, ByVal varVal As Object) As String

        Select Case eAdoType
            Case 130 ' adWChar
                CastAdoValue2String = CStr(varVal)
            Case 202 ' adVarWChar
                CastAdoValue2String = CStr(varVal)
            Case 139 ' adVarNumeric
                CastAdoValue2String = CStr(varVal)
            Case 12 ' adVariant
                CastAdoValue2String = "N/A"
            Case 200 ' adVarChar
                CastAdoValue2String = CStr(varVal)
            Case 204 ' adVarBinary
                CastAdoValue2String = "N/A"
            Case 132 ' adUserDefined
                CastAdoValue2String = "N/A"
            Case 17 ' adUnsignedTinyInt
                CastAdoValue2String = CStr(varVal)
            Case 18 ' adUnsignedSmallInt
                CastAdoValue2String = CStr(varVal)
            Case 19 ' adUnsignedInt
                CastAdoValue2String = CStr(varVal)
            Case 21 ' adUnsignedBigInt
                CastAdoValue2String = CStr(varVal)
            Case 16 ' adTinyInt
                CastAdoValue2String = CStr(varVal)
            Case 2 ' adSmallInt
                CastAdoValue2String = CStr(varVal)
            Case 4 ' adSingle
                CastAdoValue2String = CStr(varVal)
            Case 138 ' adPropVariant
                CastAdoValue2String = "N/A"
            Case 131 ' adNumeric
                CastAdoValue2String = CStr(varVal)
            Case 203 ' adLongVarWChar
                CastAdoValue2String = "N/A"
            Case 201 ' adLongVarChar
                CastAdoValue2String = "N/A"
            Case 205 ' adLongVarBinary
                CastAdoValue2String = "N/A"
            Case 13 ' adIUnknown
                CastAdoValue2String = "N/A"
            Case 3 ' adInteger
                CastAdoValue2String = CStr(varVal)
            Case 9 ' adIDispatch
                CastAdoValue2String = "N/A"
            Case 72 ' adGUID
                CastAdoValue2String = CStr(varVal)
            Case 64 ' adFileTime
                CastAdoValue2String = "N/A"
            Case 10 ' adError
                CastAdoValue2String = "N/A"
            Case 0 ' adEmpty
                CastAdoValue2String = "N/A"
            Case 5 ' adDouble
                CastAdoValue2String = CStr(varVal)
            Case 14 ' adDecimal
                CastAdoValue2String = CStr(varVal)
            Case 135 ' adDBTimeStamp
                CastAdoValue2String = "N/A"
            Case 134 ' adDBTime
                CastAdoValue2String = CStr(varVal)
            Case 133 ' adDBDate
                CastAdoValue2String = CStr(varVal)
            Case 7 ' adDate
                CastAdoValue2String = CStr(varVal)
            Case 6 ' adCurrency
                CastAdoValue2String = CStr(varVal)
            Case 129 ' adChar
                CastAdoValue2String = CStr(varVal)
            Case 136 ' adChapter
                CastAdoValue2String = "N/A"
            Case 8 ' adBSTR
                CastAdoValue2String = "N/A"
            Case 11 ' adBoolean
                CastAdoValue2String = IIf(CStr(varVal) = "True", 1, 0)
            Case 128 ' adBinary
                CastAdoValue2String = "N/A"
            Case 20 ' adBigInt
                CastAdoValue2String = CStr(varVal)
            Case 8192 ' adArray
                CastAdoValue2String = "N/A"
            Case Else
                CastAdoValue2String = "N/A"
        End Select
    End Function
#End If

    Public Function CastAdoValue2Vb6Value( _
        ByVal eVb6Type As VariantType, _
        ByVal varVal As Object _
        ) As Object

        Select Case eVb6Type
            Case VariantType.Array
                CastAdoValue2Vb6Value = varVal
            Case VariantType.Boolean
                CastAdoValue2Vb6Value = CBool(varVal)
            Case VariantType.Byte
                CastAdoValue2Vb6Value = CByte(varVal)
            Case VariantType.Currency
                CastAdoValue2Vb6Value = CDbl(varVal)
            Case VariantType.DataObject
                CastAdoValue2Vb6Value = varVal
            Case VariantType.Date
                CastAdoValue2Vb6Value = CDate(varVal)
            Case VariantType.Decimal
                CastAdoValue2Vb6Value = CDec(varVal)
            Case VariantType.Double
                CastAdoValue2Vb6Value = CDbl(varVal)
            Case VariantType.Empty
                CastAdoValue2Vb6Value = Nothing
            Case VariantType.Error
                CastAdoValue2Vb6Value = Nothing
            Case VariantType.Integer
                CastAdoValue2Vb6Value = CInt(varVal)
            Case VariantType.Long
                CastAdoValue2Vb6Value = CLng(varVal)
            Case VariantType.Null
                CastAdoValue2Vb6Value = Nothing
            Case VariantType.Object
                CastAdoValue2Vb6Value = varVal
            Case VariantType.Single
                CastAdoValue2Vb6Value = CSng(varVal)
            Case VariantType.String
                CastAdoValue2Vb6Value = FStr(varVal)
            Case VariantType.UserDefinedType
                CastAdoValue2Vb6Value = varVal
            Case VariantType.Variant
                CastAdoValue2Vb6Value = varVal
        End Select
    End Function

    Public Function GetVarTypeName(ByVal vbType As VariantType) As String
        Select Case vbType
            Case vbString : GetVarTypeName = "String"
            Case vbInteger : GetVarTypeName = "Integer"
            Case vbLong : GetVarTypeName = "Long"
            Case vbDate : GetVarTypeName = "Date"
            Case vbBoolean : GetVarTypeName = "Boolean"
            Case vbByte : GetVarTypeName = "Byte"
            Case vbSingle : GetVarTypeName = "Single"
            Case vbDouble : GetVarTypeName = "Double"
            Case vbCurrency : GetVarTypeName = "Currency"
            Case Else : GetVarTypeName = "String"
        End Select
    End Function

    Public Function GetSqlTypeName(ByVal vbType As VariantType) As String
        Select Case vbType
            Case vbString : GetSqlTypeName = "VARCHAR"
            Case vbInteger : GetSqlTypeName = "SMALLINT"
            Case vbLong : GetSqlTypeName = "INT"
            Case vbDate : GetSqlTypeName = "DATETIME"
            Case vbBoolean : GetSqlTypeName = "BIT"
            Case vbByte : GetSqlTypeName = "TINYINT"
            Case vbSingle : GetSqlTypeName = "FLOAT"
            Case vbDouble : GetSqlTypeName = "FLOAT"
            Case vbCurrency : GetSqlTypeName = "MONEY"
            Case Else : GetSqlTypeName = "VARCHAR"
        End Select
    End Function


    Public Function FIsEmpty(ByVal varVal As Object, ByVal varDef As Object)
        If IsNothing(varVal) Or varVal = "" Then
            FIsEmpty = varDef
        Else
            FIsEmpty = varVal
        End If
    End Function

    Public Function CastString2Date(ByVal strDate As String, ByVal strFormat As String) As Date

        Dim intDia As Integer
        Dim intMes As Integer
        Dim intAno As Integer
        Dim intHor As Integer
        Dim intMin As Integer
        Dim intSec As Integer

        Select Case strFormat
            Case "DDMMYY"
                If Len(strDate) = 5 Then
                    strDate = "0" & strDate
                End If
                If Len(strDate) <> 6 Then
                    'Err.Raise(ERROR_EXPECTED, "", "CastString2Date: Invalid date [" & strDate & "], with format [" & strFormat & "].")
                End If
                intDia = CInt(Mid(strDate, 1, 2))
                intMes = CInt(Mid(strDate, 3, 2))
                intAno = 2000 + CInt(Mid(strDate, 5, 2))
                CastString2Date = DateSerial(intAno, intMes, intDia)
            Case "DDMMYYYY"
                If Len(strDate) <> 8 Then
                    'Err.Raise(ERROR_EXPECTED, "", "CastString2Date: Invalid date [" & strDate & "], with format [" & strFormat & "].")
                End If
                intDia = CInt(Mid(strDate, 1, 2))
                intMes = CInt(Mid(strDate, 3, 2))
                intAno = CInt(Mid(strDate, 5, 4))
                CastString2Date = DateSerial(intAno, intMes, intDia)

            Case "YYMMDDMM"
                If Len(strDate) = 5 Then
                    strDate = "0" & strDate
                End If
                If Len(strDate) <> 6 Then
                    'Err.Raise(ERROR_EXPECTED, "", "CastString2Date: Invalid date [" & strDate & "], with format [" & strFormat & "].")
                End If
                intDia = CInt(Mid(strDate, 5, 2))
                intMes = CInt(Mid(strDate, 3, 2))
                intAno = 2000 + CInt(Mid(strDate, 1, 2))
                CastString2Date = DateSerial(intAno, intMes, intDia)
            Case "YYYYMMDD"
                If Len(strDate) <> 8 Then
                    'Err.Raise(ERROR_EXPECTED, "", "CastString2Date: Invalid date [" & strDate & "], with format [" & strFormat & "].")
                End If
                intDia = CInt(Mid(strDate, 7, 2))
                intMes = CInt(Mid(strDate, 5, 2))
                intAno = CInt(Mid(strDate, 1, 4))
                CastString2Date = DateSerial(intAno, intMes, intDia)

            Case "YYYYMMDD HH:mm"
                If Len(strDate) <> 14 Then
                    ' Err.Raise(ERROR_EXPECTED, "", "CastString2Date: Invalid date [" & strDate & "], with format [" & strFormat & "].")
                End If
                intDia = CInt(Mid(strDate, 7, 2))
                intMes = CInt(Mid(strDate, 5, 2))
                intAno = CInt(Mid(strDate, 1, 4))
                intHor = CInt(Mid(strDate, 10, 2))
                intMin = CInt(Mid(strDate, 13, 2))
                CastString2Date = CDate(intAno & "-" & intMes & "-" & intDia & " " & intHor & ":" & intMin)

            Case "YYYYMMDD HH:mm:ss"
                If Len(strDate) <> 17 Then
                    'Err.Raise(ERROR_EXPECTED, "", "CastString2Date: Invalid date [" & strDate & "], with format [" & strFormat & "].")
                End If
                intDia = CInt(Mid(strDate, 7, 2))
                intMes = CInt(Mid(strDate, 5, 2))
                intAno = CInt(Mid(strDate, 1, 4))
                intHor = CInt(Mid(strDate, 10, 2))
                intMin = CInt(Mid(strDate, 13, 2))
                intSec = CInt(Mid(strDate, 16, 2))
                CastString2Date = CDate(intAno & "-" & intMes & "-" & intDia & " " & intHor & ":" & intMin & ":" & intSec)

            Case "YYYYMMDDHHMM"
                If Len(strDate) <> 12 Then
                    'Err.Raise(ERROR_EXPECTED, "", "CastString2Date: Invalid date [" & strDate & "], with format [" & strFormat & "].")
                End If
                intDia = CInt(Mid(strDate, 7, 2))
                intMes = CInt(Mid(strDate, 5, 2))
                intAno = CInt(Mid(strDate, 1, 4))
                intHor = CInt(Mid(strDate, 9, 2))
                intMin = CInt(Mid(strDate, 11, 2))
                CastString2Date = CDate(intAno & "-" & intMes & "-" & intDia & " " & intHor & ":" & intMin)

            Case Else
                Err.Raise(ErrorToString, "", "CastString2Date: format not handled: [" & strFormat & "].")
        End Select
        Return CastString2Date
    End Function

    ' When casting a string to single/double, standard VB6 conversion assumes a decimal separator exists;
    ' If not, the whole string is treated as integer part.
    ' If no decimal separator exists, use this function instead
    Public Function CastString2Double( _
        ByVal strNum As String, _
        Optional ByVal intNDec As Integer = 2, _
        Optional ByVal blnHasDecSep As Boolean = False _
        ) As Double
        Dim strDecPart As String
        Dim strIntPart As String

        strDecPart = Right(strNum, intNDec)
        If blnHasDecSep Then
            strIntPart = Left(strNum, Len(strNum) - intNDec - 1)
        Else
            strIntPart = Left(strNum, Len(strNum) - intNDec)
        End If
        If intNDec = 0 Then
            CastString2Double = CDbl(strIntPart)
        ElseIf CDbl(strDecPart) = 0 Then
            CastString2Double = CDbl(strIntPart)
        Else
        CastString2Double = CDbl(strIntPart) + (CDbl(strDecPart) / CDbl("1" & String(intNDec, "0")))
        End If

    End Function

    ' When casting a single/double to string, standard VB6 conversion assumes a decimal separator exists.
    ' If no decimal separator must exist, use this function instead
    Public Function CastDouble2String( _
        ByVal dblNumber As Double, _
        ByVal intNDigInteger As Integer, _
        Optional ByVal strSignal As String = " " _
        ) As String

        dblNumber = Math.Abs(dblNumber)
        CastDouble2String = TruncatePadItem(FStr(Int(dblNumber)), intNDigInteger, "0") & TruncatePadItem(FStr(GetDecimalPart(dblNumber)), 2, "0") & strSignal

    End Function

    Private Function GetDecimalPart(ByVal dblNumber As Double) As String
        Dim dblDecPart As Double

        dblDecPart = Format(dblNumber, "#####.00") - Int(dblNumber)
        If Len(CStr(dblDecPart)) >= 2 Then
            GetDecimalPart = Left(Right(CStr(Format(dblDecPart, "####.00")), Len(CStr(Format(dblDecPart, "####.00"))) - 1), 2)
        Else
            GetDecimalPart = "00"
        End If

    End Function

    Public Function BuildDataCR(ByVal dtValue As Date) As String

        Dim str As String

        str = " Date(" + Trim(Year(dtValue)) + "," + Trim(Month(dtValue)) + "," + Trim(Day(dtValue)) + ")"

        BuildDataCR = str
    End Function

    Public Function TruncatePadItem( _
    ByVal strIn As String, _
    ByVal lngLEN As Long, _
    Optional ByVal strPadChar As String = " ", _
    Optional ByVal blnPadLeft As Boolean = True _
    ) As String

        Dim strRes As String

        If lngLEN = 0 Then
            strRes = ""
        Else

            ' If it's longer then allowed lenght, truncate string
            strRes = Left(strIn, lngLEN)

            ' If it's shorter then allowed lenght, pad string
            If Len(strRes) < lngLEN Then
                If blnPadLeft Then
                    strRes = Lpad(strRes, strPadChar, lngLEN)
                Else
                    strRes = Rpad(strRes, strPadChar, lngLEN)
                End If
            End If

        End If

        TruncatePadItem = strRes
    End Function

    '------------------------------------------------------------------
    ' Lpad : left padd of some string
    ' PARAMS
    '       strIn     : input string
    '       strPadChar: padding char
    '       lngN      : desired size
    ' Example: Lpad( "XPT", "0", 10 ) returns "0000000XPT"
    ' History: remade FROM other project
    '------------------------------------------------------------------
    Public Function Lpad( _
        ByVal strIn As String, _
        ByVal strPadChar As String, _
        ByVal lngN As Long _
    ) As String

        Dim lngNchars As Long

        lngNchars = lngN - Len(strIn)
        If lngNchars > 0 Then
        Lpad = String(lngNchars, Mid(strPadChar, 1, 1)) & strIn
        Else
            ' greater then desired
            Lpad = strIn
        End If

    End Function

    '------------------------------------------------------------------
    ' Rpad : Right padd of some string
    ' PARAMS
    '       strIn     : input string
    '       strPadChar: padding char
    '       lngN      : desired size
    ' Example: Rpad( "XPT", "0", 10 ) returns "XPT0000000"
    ' History: remade FROM other project
    '------------------------------------------------------------------
    Public Function Rpad( _
        ByVal strIn As String, _
        ByVal strPadChar As String, _
        ByVal lngN As Long _
    ) As String

        Dim lngNchars As Long

        lngNchars = lngN - Len(strIn)
        If lngNchars > 0 Then
        Rpad = strIn & String(lngNchars, Mid(strPadChar, 1, 1))
        Else
            ' greater then desired
            Rpad = strIn
        End If

    End Function

    '--------------------------------------------------------------------------------------
    ' GetKey : FROM "test=1" returns test
    ' Function takes a string with an '=' in it and returns the left part
    '--------------------------------------------------------------------------------------
    Public Function GetKey(ByVal strItem As String) As String

        Dim I As Integer

        I = InStr(1, strItem, "=", vbTextCompare)
        If I > 0 Then
            GetKey = Left(strItem, I - 1)
        Else
            GetKey = ""
        End If
    End Function

#End Region

End Class
