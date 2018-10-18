using Interop.ErpBS800;
using Interop.GcpBE800;
using Interop.StdBE800;
using Interop.CrmBE800;

using Primavera.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primavera.MotoresPrimavera.Comercial
{
    public class MotoresComercial
    {
        private ErpBS _erpBs;

        private GcpBEDocumentoVenda documentoVenda;
        private GcpBELinhasDocumentoVenda linhasDocVenda;
        private GcpBEAvenca documentoAvenca;

        public string modulo = "V";

        public MotoresComercial(ErpBS _erpBs)
        {
            this._erpBs = _erpBs;
        }
        
        #region "Documentos de Vendas"
        
        /// <summary>
        /// Metodo para o preenchemento do documento de venda, prenche-se os campos por defeito do documento de venda e os campos enviados no metodo
        /// </summary>
        /// <param name="tipodoc"></param>
        /// <param name="dataDoc"></param>
        /// <param name="dataGravacao"></param>
        /// <param name="tipoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <returns></returns>
        public PrimaveraResultStructure PreencheCabecalho_DocumentoVenda(string tipodoc, DateTime dataDoc, DateTime dataGravacao, string tipoEntidade, string codigoEntidade)
        {
            string sourceName = "PreencheCabecalho_DocumentoVenda";
            //ErpBS objmotor = Primavera.MotoresPrimavera.Parametros.EmpresaErp._erpBs;

            PrimaveraResultStructure result = new PrimaveraResultStructure();

            try
            {
                documentoVenda = new GcpBEDocumentoVenda();

                documentoVenda.set_Tipodoc(tipodoc);
                documentoVenda.set_Serie(_erpBs.Comercial.Series.DaSerieDefeito("V",tipodoc));
                documentoVenda.set_DataDoc(dataDoc);
                documentoVenda.set_DataGravacao(dataGravacao);

                documentoVenda.set_TipoEntidade(tipoEntidade);
                documentoVenda.set_Entidade(codigoEntidade);
                
                //documentoVenda.set_Requisicao(referencia);

                _erpBs.Comercial.Vendas.PreencheDadosRelacionados(documentoVenda);

                
                result = new PrimaveraResultStructure()
                {
                    codigo = 0
                };
                                
            }
            catch (Exception e)
            {
                result = new PrimaveraResultStructure()
                {
                    codigo = 3,
                    descricao = "Erro Logica Primavera - Preenche Cabeçalho do Documento " + e.Message,
                    procedimento = "Contacto os Responsaveis do Projecto"
                };

                PrimaveraWSLogger.escreveErro(result.ToString(), sourceName);
            }

            return result;
        }

        /// <summary>
        /// Metodo para adicionar-se linhas/produtos nos documentos de venda
        /// </summary>
        /// <param name="artigo"></param>
        /// <param name="quantidade"></param>
        /// <param name="armazem"></param>
        /// <param name="localizacao"></param>
        /// <param name="precoUnitario"></param>
        /// <returns></returns>
        public PrimaveraResultStructure Adiciona_Linhas_DocumentoVenda(string artigo, double quantidade, 
            ref string armazem, ref string localizacao, double precoUnitario)
        {
            PrimaveraResultStructure result = new PrimaveraResultStructure();
            string source = "Adiciona_Linhas_DocumentoVenda";
            try
            {
                _erpBs.Comercial.Vendas.AdicionaLinha(documentoVenda, artigo, quantidade, armazem, localizacao, precoUnitario);

                result = new PrimaveraResultStructure()
                {
                    codigo = 0
                };
            }
            catch (Exception e)
            {
                result = new PrimaveraResultStructure()
                {
                    codigo = 3,
                    descricao = "Erro Logica Primavera - Adiciona Linhas DocumentoVenda " + e.Message,
                    procedimento = "Contacto os Responsaveis do Projecto"
                };
                PrimaveraWSLogger.escreveErro(string.Format("[{0}] Erro Logica Primavera devido ao: {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), e.Message), source);
            }

            return result;
            
        }

        /// <summary>
        /// Metodo para adicionar-se linhas/produtos ao documento de venda
        /// </summary>
        /// <param name="artigo"></param>
        /// <param name="quantidade"></param>
        /// <returns></returns>
        public PrimaveraResultStructure Adiciona_Linhas_DocumentoVenda(string artigo,double quantidade)
        {
            ErpBS objmotor = _erpBs;
            string source = "Adiciona_Linhas_DocumentoVenda";

            PrimaveraResultStructure result = new PrimaveraResultStructure();
            try
            {
                objmotor.Comercial.Vendas.AdicionaLinha(documentoVenda, artigo);

                result = new PrimaveraResultStructure()
                {
                    codigo = 0
                };
            }
            catch (Exception e)
            {
                result = new PrimaveraResultStructure()
                {
                    codigo = 3,
                    descricao = "Erro Logica Primavera - Adiciona Linhas DocumentoVenda " + e.Message,
                    procedimento = "Contacto os Responsaveis do Projecto"
                };
                PrimaveraWSLogger.escreveErro(string.Format("[{0}] Erro Logica Primavera devido ao: {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), e.Message), source);
            }

            return result;
        }

        /// <summary>
        /// Metodo para a gravação de um determinado documento de venda
        /// </summary>
        /// <param name="nomeUtilizador"></param>
        /// <param name="cdu_semestre"></param>
        /// <returns></returns>
        public PrimaveraResultStructure Gravar_DocumentoVenda(string nomeUtilizador="",string cdu_semestre ="")
        {
            ErpBS objmotor = _erpBs;
            string source = "Gravar_DocumentoVenda";

            PrimaveraResultStructure resultado = new PrimaveraResultStructure();
            string str_avisos = "";
            try
            {
                objmotor.Comercial.Vendas.Actualiza(documentoVenda, str_avisos);
                objmotor.Comercial.Vendas.ActualizaValorAtributo(documentoVenda.get_Filial(), documentoVenda.get_Tipodoc(),
                    documentoVenda.get_Serie(), documentoVenda.get_NumDoc(), "CDU_NOME_UTILIZADOR", nomeUtilizador);

                objmotor.Comercial.Vendas.ActualizaValorAtributo(documentoVenda.get_Filial(), documentoVenda.get_Tipodoc(),
                    documentoVenda.get_Serie(), documentoVenda.get_NumDoc(), "CDU_Semestre", cdu_semestre);

                resultado.codigo = 0;
                resultado.tipoProblema = str_avisos;             
                resultado.descricao = string.Format("Foi Gerado o documento: #{0} {1}/{2}#", documentoVenda.get_Tipodoc(), documentoVenda.get_NumDoc(), documentoVenda.get_Serie());

            }
            catch (Exception e)
            {
                resultado.codigo = 3;
                resultado.descricao = e.Message;
                PrimaveraWSLogger.escreveErro(string.Format("[{0}] Erro Logica Primavera devido ao: {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), e.Message), source);
            }

            return resultado;
        }

        /// <summary>
        /// Metodo para impressão do documento de venda, ainda não implementado
        /// </summary>
        /// <param name="idDoc"></param>
        /// <returns></returns>
        private Stream imprimirDoc(string idDoc)
        {
            return null;
        }
        
        /// <summary>
        /// Metodo para a criação das Avenças com base no documento de venda 
        /// </summary>
        /// <returns></returns>
        public PrimaveraResultStructure CriaAvenca()
        {
            PrimaveraResultStructure resultado = new PrimaveraResultStructure();
            string source = "CriaAvenca";
            try
            {

                DateTime dataInicio, dataFim, dataPrimeiraPropina;
                string entidade, semestre, semestreAntigo, str_avisos = "";

                

                semestre = _erpBs.Comercial.Vendas.DaValorAtributo(
                    documentoVenda.get_Filial(),
                    documentoVenda.get_Tipodoc(),
                    documentoVenda.get_Serie(),
                    documentoVenda.get_NumDoc(),
                    "CDU_Semestre"
                    ).ToString();

                var sql_query = String.Format("select * from tdu_semestre where CDU_IdSemestre ='{0}'", semestre);
                var objLista = _erpBs.Consulta(sql_query);

                if (!objLista.Vazia())
                {
                    





                    dataInicio = (DateTime)objLista.Valor("CDU_DataInicio");
                    dataFim = (DateTime)objLista.Valor("CDU_DataUltimaPropina");
                    dataPrimeiraPropina = (DateTime)objLista.Valor("CDU_DataPrimeiraPropina");

                    entidade = _erpBs.Comercial.Vendas.DaValorAtributo(
                        documentoVenda.get_Filial(),
                        documentoVenda.get_Tipodoc(),
                        documentoVenda.get_Serie(),
                        documentoVenda.get_NumDoc(),
                        "Entidade"
                        ).ToString();

                    var _aluno = _erpBs.Comercial.Clientes.Edita(entidade);

                    if(_aluno.get_TipoTerceiro() != "")
                    {
                        if (_erpBs.Comercial.Avencas.Existe(documentoVenda.get_Filial(), entidade))
                        {
                            documentoAvenca = _erpBs.Comercial.Avencas.Edita(documentoVenda.get_Filial(), entidade);

                            semestreAntigo = _erpBs.Comercial.Vendas.DaValorAtributo(
                                documentoAvenca.get_FilialDocOriginal(),
                                documentoAvenca.get_TipoDocOriginal(),
                                documentoAvenca.get_SerieDocOriginal(),
                                documentoAvenca.get_NumDocOriginal(),
                                "CDU_Semestre"
                                ).ToString();

                            if (semestreAntigo != semestre)
                            {
                                documentoAvenca.set_SerieDocOriginal(documentoVenda.get_Serie());
                                documentoAvenca.set_TipoDocOriginal(documentoVenda.get_Tipodoc());
                                documentoAvenca.set_NumDocOriginal(documentoVenda.get_NumDoc());

                                documentoAvenca.set_DataInicio(dataInicio);
                                documentoAvenca.set_DataFim(dataFim);
                                documentoAvenca.set_DataProximoProcessamento(dataPrimeiraPropina);

                                _erpBs.Comercial.Avencas.Actualiza(documentoAvenca, str_avisos);
                            }
                            else
                            {
                                documentoAvenca.set_SerieDocOriginal(documentoVenda.get_Serie());
                                documentoAvenca.set_TipoDocOriginal(documentoVenda.get_Tipodoc());
                                documentoAvenca.set_NumDocOriginal(documentoVenda.get_NumDoc());

                                _erpBs.Comercial.Avencas.Actualiza(documentoAvenca, str_avisos);
                            }

                            resultado = new PrimaveraResultStructure()
                            {
                                codigo = 0,
                                descricao = string.Format("Foi actualizado com sucesso o contrato {0} {1}/{2} do aluno {3} e a respectiva avença para o semestre {4}",
                                        documentoVenda.get_Tipodoc(),
                                        documentoVenda.get_NumDoc(),
                                        documentoVenda.get_Serie(),
                                        documentoVenda.get_Nome(),
                                        semestre)
                            };


                        }
                        else
                        {
                            documentoAvenca = new GcpBEAvenca();
                            //Dados da avença
                            documentoAvenca.set_Descricao((semestre + " " + documentoVenda.get_Nome()).Substring(0, 19));
                            documentoAvenca.set_Avenca(documentoVenda.get_Entidade());
                            documentoAvenca.set_Entidade(documentoVenda.get_Entidade());
                            documentoAvenca.set_TipoEntidade("C");
                            documentoAvenca.set_TipoEntidadeFac("C");
                            documentoAvenca.set_EntidadeFac(documentoVenda.get_Entidade());

                            //Dados do documento base
                            documentoAvenca.set_SerieDocOriginal(documentoVenda.get_Serie());
                            documentoAvenca.set_TipoDocOriginal(documentoVenda.get_Tipodoc());
                            documentoAvenca.set_NumDocOriginal(documentoVenda.get_NumDoc());
                            documentoAvenca.set_FilialDocOriginal(documentoVenda.get_Filial());

                            //Dados do documento Destino
                            documentoAvenca.set_TipoDocDestino("FA");
                            documentoAvenca.set_SerieDocDestino(documentoVenda.get_Serie());

                            //Dados do Processamento da Avença
                            documentoAvenca.set_DataInicio(dataInicio);
                            documentoAvenca.set_DataFim(dataFim);
                            documentoAvenca.set_DataProximoProcessamento(dataPrimeiraPropina);
                            documentoAvenca.set_Periodicidade("0");

                            _erpBs.Comercial.Avencas.Actualiza(documentoAvenca, str_avisos);

                            resultado = new PrimaveraResultStructure()
                            {
                                codigo = 0,
                                descricao = string.Format("Foi criado com sucesso o contrato {0} {1}/{2} do aluno {3} e a respectiva avença para o semestre {4}",
                                        documentoVenda.get_Tipodoc(),
                                        documentoVenda.get_NumDoc(),
                                        documentoVenda.get_Serie(),
                                        documentoVenda.get_Nome(),
                                        semestre)
                            };
                        }
                    }
                    else
                    {
                        resultado = new PrimaveraResultStructure()
                        {
                            codigo = 3,
                            tipoProblema = "Erro Logica Primavera",
                            procedimento = "Consulte aos Tecnicos do Projecto",
                            descricao = string.Format("Erro ao criar a avença porque o aluno {0} - {1} não tem o tipo terceiro - curso {2} no ERP Primavera",
                                   documentoVenda.get_Entidade(),
                                   documentoVenda.get_Nome(),
                                   _aluno.get_CamposUtil().get_Item("CDU_CodLic").Valor)
                        };
                    }
                }
                else
                {
                    resultado = new PrimaveraResultStructure()
                    {
                        codigo = 3,
                        tipoProblema="Erro Logica Primavera",
                        procedimento="Consulte aos Tecnicos do Projecto",
                        descricao = string.Format("Erro ao criar a avença do aluno {0} - {1} não existe o semestre {2}",
                                   documentoVenda.get_Entidade(),
                                   documentoVenda.get_Nome(),
                                   semestre)
                    };
                }
            }
            catch (Exception ex)
            {
                resultado.codigo = 3;
                resultado.descricao = ex.Message;

                PrimaveraWSLogger.escreveErro(string.Format("[{0}] Erro Logica Primavera na gravação da avença, devido ao: {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), ex.Message), source);

            }

            return resultado;
        }
        
        /// <summary>
        /// Metodo para a consulta de conta do cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public List<Pendente> ConsultaConta(string cliente)
        {
            string source = "ConsultaConta";

            List<Pendente> listaPendente = new List<Pendente>();
            try
            {
                string str_query = String.Format("Select * from pendentes where entidade = '{0}' and tipoentidade='C'", cliente);
                var objLista = _erpBs.Consulta(str_query);

                while (!(objLista.NoInicio() || objLista.NoFim()))
                {
                    Pendente pendente = new Pendente();

                    string numdoc = objLista.Valor("NumDocInt").ToString();


                    pendente.cliente = (string)objLista.Valor("Entidade");
                    pendente.dataCriacao = (DateTime)objLista.Valor("DataDoc");
                    pendente.dataVencimento = (DateTime)objLista.Valor("DataVenc");
                    pendente.tipoDoc = (string)objLista.Valor("TipoDoc");
                    pendente.numDoc = Convert.ToInt32(numdoc);
                    pendente.serie = (string)objLista.Valor("Serie");
                    pendente.moeda = (string)objLista.Valor("Moeda");
                    pendente.valorPendente = (double)objLista.Valor("ValorPendente");
                    pendente.valorTotal = (double)objLista.Valor("ValorTotal");
                    
                    listaPendente.Add(pendente);
                    objLista.Seguinte();
                }
            }
            catch (Exception ex)
            {
                PrimaveraWSLogger.escreveErro(string.Format("Ocorreu um erro na consulta do saldo do cliente {0} devido ao seguinte erro: {1}", cliente, ex.Message), source);

            }
            
            

            return listaPendente;
            
        }

        #endregion

        #region Clientes

        /// <summary>
        /// Metodo para do codigo do cliente, este metodo altera no primavera todas as tabelas relacionados com um determinado
        /// cliente
        /// </summary>
        /// <param name="codigoAntigo"></param>
        /// <param name="codigoNovo"></param>
        /// <returns></returns>
        public PrimaveraResultStructure AlteraCodigoCliente(string codigoAntigo, string codigoNovo)
        {
            PrimaveraResultStructure result;
            
            try
            {
                _erpBs.Comercial.Clientes.AlteraCodigoCliente(codigoAntigo, codigoNovo);
                result = new PrimaveraResultStructure() 
                { 
                    codigo = 0, 
                    descricao = string.Format("Foi actualizado o codigo do cliente {0} para {1}", codigoAntigo,codigoNovo) 
                };
            }
            catch
            {
                result = new PrimaveraResultStructure()
                {
                    codigo = 3,
                    subNivel= "303",
                    descricao = string .Format("Erro na alteração do codigo do cliente {0} para {1}",codigoAntigo,codigoNovo),
                    procedimento = "Consultar os técnicos do projecto"
                };
            }
            
            
            return result;
        }

        /// <summary>
        /// Valida se existe existe no Erp Primavera
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public bool ExisteCliente(string cliente)
        {
            try
            {
                return _erpBs.Comercial.Clientes.Existe(cliente);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Metodo para a gravação/actualização do cliente no Erp Primavera usando os Motores Primavera
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="nome"></param>
        /// <param name="nuit"></param>
        /// <param name="vendedor"></param>
        /// <param name="observacao"></param>
        /// <param name="fac_Morada"></param>
        /// <param name="fac_Localidade"></param>
        /// <param name="fac_Telefone"></param>
        /// <param name="desconto"></param>
        /// <param name="cdu_bolsa"></param>
        /// <param name="cdu_geraMulta"></param>
        /// <param name="cdu_tipoIngresso"></param>
        /// <param name="cdu_codLic"></param>
        /// <param name="cdu_turma"></param>
        /// <returns></returns>
        public PrimaveraResultStructure GravarCliente(string codigo,string nome, string nuit,string vendedor,string observacao, string fac_Morada, 
            string fac_Localidade, string fac_Telefone, float desconto = 0, Boolean cdu_bolsa = false, Boolean cdu_geraMulta = true, string cdu_tipoIngresso ="",
            string cdu_codLic="" , string cdu_turma = "" )
        {
            string source = "GravarCliente";

            PrimaveraResultStructure result; 
            try
            {
                var str_query = String.Format("select cdu_condpag from TDU_ParametrosISUTC where cdu_vendedor='{0}'",vendedor);
                var condpag = this._erpBs.Consulta(str_query).Valor("cdu_condpag").ToString();
                
                // Verifica se o cliente Existe
                if(_erpBs.Comercial.Clientes.Existe(codigo) == true)
                {
                    //Actualização dos dados do Cliente
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo (codigo , "nome", nome);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo (codigo, "NomeFiscal", nome);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo (codigo, "Moeda", "MT");
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo (codigo, "NumContribuinte", nuit);
                    
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "Vendedor", vendedor);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CondPag", condpag);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "Desconto", desconto);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "Morada", fac_Morada);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "Telefone", fac_Telefone);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "Localidade", fac_Localidade);
                    
                    //Campos de utilizador
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo (codigo, "CDU_Bolsa", cdu_bolsa);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_GeraMulta", cdu_geraMulta);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_TipoIngresso", cdu_tipoIngresso);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_CodLic", cdu_codLic);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_Turma", cdu_turma);

                    result  = new PrimaveraResultStructure()
                        {
                            codigo=0,
                            descricao= "Codigos de Sucesso",
                            tipoProblema = String.Format("Os dados Aluno {0} - {1} Gravados Com Sucesso Completo", codigo, nome)
                        };
                    
                    PrimaveraWSLogger.escreveInformacao(String.Format("[{0}] Os dados Aluno {1} - {2} Gravados Com Sucesso Completo", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), codigo, nome), source);
                }
                else{
                    GcpBECliente _cliente = new GcpBECliente();
                    
                    _cliente.set_Cliente (codigo);
                    _cliente.set_Nome(nome);
                    _cliente.set_NomeFiscal (nome);
                    
                    _cliente.set_Moeda ("MT");
                    _cliente.set_NumContribuinte(nuit);
                    _cliente.set_Desconto(desconto);
                    _cliente.set_Vendedor(vendedor);
                    _cliente.set_CondPag(condpag);

                    _cliente.set_Morada(fac_Morada);
                    _cliente.set_Telefone(fac_Telefone);
                    _cliente.set_Localidade(fac_Localidade);

                    _erpBs.Comercial.Clientes.Actualiza (_cliente);

                    //Campos de utilizador
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_Bolsa", cdu_bolsa);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_GeraMulta", cdu_geraMulta);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_Turma", cdu_turma);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_CodLic", cdu_codLic);
                    _erpBs.Comercial.Clientes.ActualizaValorAtributo(codigo, "CDU_TipoIngresso", cdu_tipoIngresso);
                    
                    result  = new PrimaveraResultStructure()
                        {
                            codigo=0,
                            descricao= "Codigos de Sucesso",
                            tipoProblema = String.Format("Os dados Aluno {0} - {1} Actualizados Com Sucesso Completo", codigo,nome)
                        };
                    PrimaveraWSLogger.escreveInformacao(String.Format("[{0}] Os dados Aluno {1} - {2} Actualizados Com Sucesso Completo", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), codigo, nome),source);
                    
                }
                               
            }
            catch (Exception e)
            {
                
                result  = new PrimaveraResultStructure()
                        {
                            codigo=3,
                            descricao= "Erro ao gravar o aluno - " + e.Message,
                            tipoProblema = "Erro Logica no Primavera",
                            procedimento = "Consultar os técnicos do projecto",
                            subNivel = "30 - O Erro ao gravar o aluno"
                        };
                PrimaveraWSLogger.escreveErro(String.Format("[{0}] Erro ao gravar o aluno {1} - {2} devido ao seguinte erro: {3} ", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), codigo, nome, e.Message),source );
                
            }
            return result;

        }

        /// <summary>
        /// Metodo para a gravação/actualização do contacto de um determinado cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="codigo"></param>
        /// <param name="primeiroNome"></param>
        /// <param name="ultimoNome"></param>
        /// <param name="nomeCompleto"></param>
        /// <param name="morada"></param>
        /// <param name="localidade"></param>
        /// <param name="emailPrincipal"></param>
        /// <param name="emailAlternativo"></param>
        /// <param name="nrTelefone"></param>
        /// <param name="nrTelefoneAlternativo"></param>
        /// <returns></returns>
        public PrimaveraResultStructure GravarContactoCliente(string cliente, string codigo, string primeiroNome, 
            string ultimoNome, string nomeCompleto, string morada, string localidade, string emailPrincipal, string emailAlternativo,
            string nrTelefone, string nrTelefoneAlternativo){
            PrimaveraResultStructure result;
            string source = "GravarContactoCliente";
            try
            {                
                CrmBEContacto objContacto = new CrmBEContacto();
                CrmBELinhaContactoEntidade objEntidateContacto = new CrmBELinhaContactoEntidade();

                //Valida se existe o contacto do aluno no sistema

                if(_erpBs.CRM.Contactos.Existe(codigo)){

                    //Actualiza os contactos do Cliente
                    objContacto = _erpBs.CRM.Contactos.Edita(codigo);

                    objContacto.set_EmModoEdicao(true);
                    objContacto.set_Contacto(codigo); //validar este campo, por ser a chave do contacto
                    objContacto.set_PrimeiroNome(primeiroNome);
                    objContacto.set_UltimoNome(ultimoNome);
                    objContacto.set_Nome(nomeCompleto);
                    objContacto.set_Morada (morada);
                    objContacto.set_Localidade (localidade);
                    objContacto.set_Telefone( nrTelefone);
                    objContacto.set_Telefone2 (nrTelefoneAlternativo);
                    objContacto.set_Email (emailPrincipal);
                    objContacto.set_EmailResid (emailAlternativo);

                    objEntidateContacto = new CrmBELinhaContactoEntidade();
                    
                    _erpBs.CRM.Contactos.Actualiza (objContacto);
                    
                    result  = new PrimaveraResultStructure()
                        {
                            codigo=0,
                            descricao= "Codigos de Sucesso",
                            tipoProblema = "Os Contactos do Aluno Actualizado Com Sucesso Completo"
                        };
                    
                }else{

                    objContacto = new CrmBEContacto();

                    objContacto.set_ID (Guid.NewGuid().ToString());
                    objContacto.set_Contacto ( codigo);
                    objContacto.set_PrimeiroNome(primeiroNome);
                    objContacto.set_UltimoNome(ultimoNome);
                    objContacto.set_Nome(nomeCompleto);
                    objContacto.set_Morada(morada);
                    objContacto.set_Localidade(localidade);
                    objContacto.set_Telefone(nrTelefone);
                    objContacto.set_Telefone2(nrTelefoneAlternativo);
                    objContacto.set_Email(emailPrincipal);
                    objContacto.set_EmailResid(emailAlternativo);
                    
                    objEntidateContacto = new CrmBELinhaContactoEntidade();

                    objEntidateContacto.set_IDContacto(objContacto.get_ID());
                    objEntidateContacto.set_Entidade(cliente);
                    objEntidateContacto.set_TipoEntidade("C");
                    objEntidateContacto.set_Email(emailPrincipal);
                    objEntidateContacto.set_Telefone(nrTelefone);
                    objEntidateContacto.set_Telemovel(nrTelefoneAlternativo);
                    objEntidateContacto.set_TipoContacto( "Aluno"); // Para já Fixo
    
                    objContacto.get_LinhasEntidade().Insere (objEntidateContacto);

                    _erpBs.CRM.Contactos.Actualiza (objContacto);

                    result  = new PrimaveraResultStructure()
                        {
                            codigo=0,
                            descricao= "Codigos de Sucesso",
                            tipoProblema = "Os Contactos do Aluno Gravado Com Sucesso Completo"
                        };
                    PrimaveraWSLogger.escreveInformacao(string.Format("[{0}] O contacto {1} - {2} foi gravado e adicionado ao cliente {3} com Sucesso", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), codigo, nomeCompleto, cliente),source);
                }

                                
                

            }catch (Exception e)
            {
                result  = new PrimaveraResultStructure()
                        {
                            codigo=3,
                            descricao= "Erro ao gravar o aluno - " + e.Message,
                            tipoProblema = "Erro Logica no Primavera",
                            procedimento = "Consultar os técnicos do projecto",
                            subNivel = "30 - O Erro ao gravar o aluno"
                        };
                PrimaveraWSLogger.escreveErro(string.Format("[{0}] Erro ao adicionar o contacto {1} - {2} ao cliente {3} devido ao seguinte erro: {4}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), codigo, nomeCompleto, cliente,e.Message),source);
            }

            return result;
        }

        public PrimaveraResultStructure RemoveContactoCliente(string cliente)
        {
            string source = "RemoveContactoCliente";
            PrimaveraResultStructure result = new PrimaveraResultStructure();
            try
            {

                List<string> lista = new List<string>();
                var l = _erpBs.CRM.Contactos.ListaContactosDaEntidade("C", cliente);
                for(int i =1; i<= l.NumItens; i++)
                {
                    lista.Add(l.Edita[i].get_ID());
                }

                _erpBs.CRM.Contactos.RemoveContactosDaEntidade("C", cliente);

                foreach (var c in lista)
                    _erpBs.CRM.Contactos.RemoveID(c);

                result = new PrimaveraResultStructure()
                {
                    codigo = 0,
                    descricao = "Codigos de Sucesso",
                    tipoProblema = string.Format("[{0}] Os contactos da entidade {1} foram removidos com Sucesso", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), cliente)
                };
                PrimaveraWSLogger.escreveInformacao(string.Format("[{0}] Os contactos da entidade {1} foram removidos com Sucesso", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), cliente), source);

            }
            catch (Exception e)
            {
                result = new PrimaveraResultStructure()
                {
                    codigo = 3,
                    descricao = string.Format("[{0}] Erro ao remover os contactos do cliente {1} devido ao seguinte erro: {2}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), cliente, e.Message),
                    tipoProblema = "Erro Logica no Primavera",
                    procedimento = "Consultar os técnicos do projecto",
                    subNivel = "30 - O Erro ao actualizar o aluno"
                };
                PrimaveraWSLogger.escreveErro(string.Format("[{0}] Erro ao remover os contactos do cliente {1} devido ao seguinte erro: {2}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), cliente, e.Message), source);
            }

            return result;
        }

        /// <summary>
        /// Actualiza o codigo do tipo terceiro com base no codigo da licienciatura
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="codLic"></param>
        /// <returns></returns>
        public PrimaveraResultStructure ActualizaTipoTerceiro(string cliente, string codLic)
        {
            PrimaveraResultStructure resultado = new PrimaveraResultStructure();

            try
            {
                string str_query = String.Format("Select TipoTerceiro from tipoTerceiros where descricao ='{0}'", codLic);
                var objLista = _erpBs.Consulta(str_query);

                if (!objLista.Vazia())
                {
                    var objCliente = _erpBs.Comercial.Clientes.Edita(cliente);
                    objCliente.set_TipoTerceiro(objLista.Valor("TipoTerceiro").ToString());
                    _erpBs.Comercial.Clientes.Actualiza(objCliente);

                    resultado.codigo = 0;
                }
            }
            catch(Exception ex) 
            {
                resultado.codigo = 3;
                resultado.descricao = ex.Message;
            }

            return resultado;
        }

        /// <summary>
        /// Metodo que pesquisa no primavera o codigo do instituto com base no vendedor
        /// </summary>
        /// <param name="vendedor"></param>
        /// <returns>Devovle do codigo do Instituto</returns>
        public string da_Codigo_Instituto(string vendedor)
        {
            string str_query = String.Format("select cdu_instituto from tdu_parametrosIsutc where cdu_vendedor = '{0}'",vendedor);
            var _lista = _erpBs.Consulta(str_query);

            if (!_lista.Vazia())
            {
                return _lista.Valor("cdu_instituto").ToString();
            }
            else
            {
                return "";
            }
            
        }
        
        #endregion
        
    }
}
