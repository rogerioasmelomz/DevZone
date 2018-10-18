using Primavera.MotoresPrimavera;
using Primavera.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Primavera.WebServices.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Primavera_Service : IPrimavera_Service
    {
        //objecto que representa os motores Primavera
        public MotoresErp motorErp;

        /// <summary>
        /// Metodo para a criação dos candidatos (clientes) no Erp Primavera
        /// Criado Por: Guimarães Mahota, 20.08.2015
        /// Contato: gmahota@accsys.co.mz, Skype:mahotag
        /// </summary>
        /// <param name="tipoPlataforma">A instancia Primavera, 0-Executiva, 1- Professional</param>
        /// <param name="codEmpresa"></param>
        /// <param name="codUtilizador"></param>
        /// <param name="password"></param>
        /// <param name="cliente"></param>
        /// <param name="nome"></param>
        /// <param name="nomeFiscal"></param>
        /// <param name="desconto"></param>
        /// <param name="facMorada"></param>
        /// <param name="facLocalidade"></param>
        /// <param name="facNrTelefone"></param>
        /// <param name="nuit"></param>
        /// <param name="vendedor">representa o instuto do candidato (cliente) ex: ISU, ITC</param>
        /// <param name="observacao"></param>
        /// <param name="cdu_bolsa"></param>
        /// <param name="cdu_geraMulta"></param>
        /// <param name="cdu_tipoIngresso"></param>
        /// <param name="cdu_codLic"></param>
        /// <param name="cdu_turma"></param>
        /// <param name="contacto"></param>
        /// <returns></returns>
        public PrimaveraResultStructure CriarAluno(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string cliente,
            string nome, string nomeFiscal, float desconto, string facMorada, string facLocalidade, string facNrTelefone, string nuit, string vendedor, string observacao,
            bool cdu_bolsa, bool cdu_geraMulta, string cdu_tipoIngresso, string cdu_codLic, string cdu_turma, Contactos contacto)
        {
            string source = "criarAluno";

            String logMsg = string.Format("[{0}] Comunicação com o metodo Criar_Cliente pelo IP - {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), get_IP());

            PrimaveraWSLogger.escreveInformacao(logMsg, source);

            try
            {
                motorErp = new MotoresErp();

                //Abertura da Empresa Primavera
                var result = motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUtilizador, password);

                //Valida se a empresa primavera se encontra aberta
                if (result.codigo == 0)
                {
                    //Inicialiaza a instancia _erpBs em todos os motores que vão ser usados
                    motorErp.inicializaMotores_EmpresaErp();

                    //Valida se o cliente existe
                    if (!motorErp._comercial.ExisteCliente(cliente))
                    {
                        motorErp._empresaErp.IniciaTransacao();
                        result = motorErp._comercial.GravarCliente(cliente, nome, nuit,vendedor, observacao,facMorada,facLocalidade,facNrTelefone,desconto, cdu_bolsa, cdu_geraMulta,cdu_tipoIngresso, cdu_codLic,cdu_turma );

                        //Valida se ocorreu com sucesso a gravação do cliente
                        if (result.codigo == 0)
                        {
                            result = motorErp._comercial.GravarContactoCliente(cliente, contacto.codigo, contacto.primeiroNome, contacto.ultimoNome, contacto.nomeCompleto,
                                        contacto.morada, contacto.localidade, contacto.emailPrincipal, contacto.emailAlternativo, contacto.nrTelefone, contacto.nrTelefoneAlternativo);
                        }
                        
                        //Valida se ocorrou com sucesso a gravação do contacto
                        if (result.codigo == 0)
                        {
                            var instituto = motorErp._comercial.da_Codigo_Instituto(vendedor);
                            
                            if (instituto != "")
                            {
                                try
                                {
                                    string str_tipoDocRef = instituto == "82001" ? "1" : "2";
                                    
                                    //remove os caracteres não numericos
                                    string str_cliente = new String(cliente.Where(c => char.IsDigit(c)).ToArray());
                                    long codigoCliente = Convert.ToInt64(str_cliente);

                                    //gera o codigo da referencia bancaria
                                    string str_referencia = geraReferencia(instituto, str_tipoDocRef, codigoCliente, 0);

                                    string str_query = string.Format("update clientes set cdu_refbanc_antigo = cdu_REFBNC where cliente='{0}' ; ", cliente);
                                        str_query = string.Format("update clientes set cdu_REFBNC ={0} where cliente='{1}'", str_referencia, cliente);
                                    motorErp._empresaErp.Executa_Query_Insert_Update(str_query);

                                    result.descricao += String.Format(" Referencia Bancaria: #{0}#", str_referencia);
                                }catch(Exception ex){
                                    result = new Primavera.Data.PrimaveraResultStructure()
                                    {
                                        tipoProblema = "Erro lógico no Primavera",
                                        codigo = 3,
                                        subNivel = "302",
                                        codeLevel = "30",
                                        descricao = "Ocorreu um erro durante a criação da referencia bancaria do aluno " + cliente + 
                                            "devido ao seguinte erro: "+ex.Message,
                                        procedimento = "Consultar os técnicos do projecto"
                                    };
                                }
                                
                            }else
                            {
                                result = new Primavera.Data.PrimaveraResultStructure()
                                {
                                    tipoProblema = "Erro lógico no Primavera",
                                    codigo = 3,
                                    subNivel = "301",
                                    codeLevel = "30",
                                    descricao = String.Format("O Codigo do Instuto do aluno {0} não pode estar vazio", cliente),
                                    procedimento = "Consultar os técnicos do projecto"
                                };
                            }                            
                        }

                        //Se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                        if (result.codigo == 0)
                        {
                            PrimaveraWSLogger.escreveInformacao(result.to_String(), source);
                            motorErp._empresaErp.TerminaTransacao();
                        }
                        else
                        {
                            PrimaveraWSLogger.escreveErro(result.to_String(), source);
                            motorErp._empresaErp.DesfazTransacao();
                        }  
                      

                    }
                    else
                    {

                        
                        result = new Primavera.Data.PrimaveraResultStructure { tipoProblema="Erro lógico no Primavera", codigo = 3, subNivel= "300", codeLevel= "30",
                                                                descricao = "Aluno já existe com o mesmo código",
                                                                procedimento = "Consultar os técnicos do projecto"
                        };
                        
                        PrimaveraWSLogger.escreveErro(result.to_String(), source);
                    }
                }

                

                return new PrimaveraResultStructure() { tipoProblema = result.tipoProblema,codigo = result.codigo, subNivel = result.subNivel, codeLevel= result.codeLevel,
                    descricao = result.descricao, procedimento= result.procedimento };

            }
            catch (Exception ex)
            {
                Primavera.Data.PrimaveraResultStructure result = new Primavera.Data.PrimaveraResultStructure()
                {
                    tipoProblema = "Erro no Web Services - Primavera",
                    codigo = 2,
                    codeLevel = "21",
                    descricao = "WebService não consegue carregar as DLL, " + ex,
                    procedimento = "Consultar os técnicos do projecto"
                };

                PrimaveraWSLogger.escreveErro(ex.Message, source);
                motorErp._empresaErp.DesfazTransacao();

                return new PrimaveraResultStructure()
                {
                    tipoProblema = result.tipoProblema,
                    codigo = result.codigo,
                    subNivel = result.subNivel,
                    codeLevel = result.codeLevel,
                    descricao = result.descricao,
                    procedimento = result.procedimento
                };
            }

        }

        public PrimaveraResultStructure ActualizarAluno(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string cliente, string novoNrCliente,
            string nome, string nomeFiscal, float desconto, string facMorada, string facLocalidade, string facNrTelefone, string nuit, string vendedor, string observacao,
            bool cdu_bolsa, bool cdu_geraMulta, string cdu_tipoIngresso, string cdu_codLic, string cdu_turma, Contactos contacto)
        {
            string source = "ActualizarAluno";
            
            string logMsg = string.Format("[{0}] Comunicação com o metodo Actualizar_Cliente pelo IP - {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), get_IP());

            PrimaveraWSLogger.escreveInformacao(logMsg, source);

            try
            {
                //Abre a empresa no Erp Primavera
                motorErp = new MotoresErp();

                var result = motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUtilizador, password);
                //Valida se a empresa primavera se encontra aberta
                if (result.codigo == 0)
                {
                    //Inicialiaza a instancia _erpBs em todos os motores que vão ser usados
                    motorErp.inicializaMotores_EmpresaErp();

                    //Valida se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                    if (result.codigo == 0)
                    {
                        motorErp._empresaErp.IniciaTransacao();
                        result = motorErp._comercial.GravarCliente(cliente, nome, nuit, vendedor, observacao, facMorada, facLocalidade, facNrTelefone, desconto, cdu_bolsa, cdu_geraMulta, cdu_tipoIngresso, cdu_codLic, cdu_turma);
                    }
                }

                //Se não ocorrer erros na gravação dos clientes grava/actualiza os contactos
                if (result.codigo == 0)
                {
                    result = motorErp._comercial.ActualizaTipoTerceiro(cliente, cdu_codLic);

                }

                if (novoNrCliente != "")
                {
                    if (result.codigo == 0)
                        result = motorErp._comercial.RemoveContactoCliente(cliente);
                    
                    if (result.codigo == 0)
                       result = motorErp._comercial.AlteraCodigoCliente(cliente, novoNrCliente);

                    if (result.codigo == 0)
                    {
                        result = motorErp._comercial.GravarContactoCliente(
                            novoNrCliente,
                            contacto.codigo, contacto.primeiroNome, contacto.ultimoNome, contacto.nomeCompleto,
                            contacto.morada, contacto.localidade, contacto.emailPrincipal, contacto.emailAlternativo,
                            contacto.nrTelefone, contacto.nrTelefoneAlternativo
                        );

                    }

                    //Valida se ocorrou com sucesso a gravação do contacto
                    if (result.codigo == 0)
                    {
                        var instituto = motorErp._comercial.da_Codigo_Instituto(vendedor);

                        if (instituto != "")
                        {
                            try
                            {
                                string str_tipoDocRef = instituto == "82001" ? "1" : "2";

                                //remove os caracteres não numericos
                                string str_cliente = new String(novoNrCliente.Where(c => char.IsDigit(c)).ToArray());
                                long codigoCliente = Convert.ToInt64(str_cliente);

                                //gera o codigo da referencia bancaria
                                string str_referencia = geraReferencia(instituto, str_tipoDocRef, codigoCliente, 0);


                                string str_query = string.Format("update clientes set CDU_REFBNC_ANTIGO = cdu_REFBNC where cliente='{0}' ; ", novoNrCliente);
                                str_query += string.Format("update clientes set cdu_REFBNC ='{0}' where cliente='{1}' ; ", str_referencia, novoNrCliente);

                                //var a = " " + motorErp._empresaErp.Executa_Query_Insert_Update(str_query);

                                //PrimaveraWSLogger.escreveAlerta("Executando query1:{" + str_query + "} " +a.ToString(), source);

                                //Aqui tem que executar a query para actualizar todos os documentos pendentes e historico com a referencia do cliente
                                str_query += "update p set p.cdu_referencia = c.cdu_refbnc,p.CDU_Instituto= td.cdu_instituto  from Pendentes p ";
                                str_query += "inner join Clientes c on c.Cliente = p.Entidade and p.TipoEntidade ='C' ";
                                str_query += "inner join TDU_ParametrosISUTC td on td.CDU_Vendedor = c.Vendedor ";
                                str_query += string.Format("where p.Entidade ='{0}' and p.tipoEntidade = 'C' ; ", novoNrCliente);

                                //var b= " " + motorErp._empresaErp.Executa_Query_Insert_Update(str_query);

                                //PrimaveraWSLogger.escreveAlerta("Executando query2: {"+str_query+"} "+ b.ToString(), source);

                                str_query += "update p set p.cdu_referencia = c.cdu_refbnc,p.CDU_Instituto= td.cdu_instituto  from historico p ";
                                str_query += "inner join Pendentes pe on pe.IdHistorico = p.Id ";
                                str_query += "inner join Clientes c on c.Cliente = p.Entidade and p.TipoEntidade ='C' ";
                                str_query += "inner join TDU_ParametrosISUTC td on td.CDU_Vendedor = c.Vendedor ";
                                str_query += string.Format("where p.Entidade ='{0}' and p.tipoEntidade = 'C' ; ", novoNrCliente);

                                var d= result.descricao += " " + motorErp._empresaErp.Executa_Query_Insert_Update(str_query);

                                PrimaveraWSLogger.escreveAlerta("Executando query1: {" + str_query + "} " + d.ToString(), source);

                                result.descricao += String.Format(" Referencia Bancaria: #{0}#", str_referencia);
                            }
                            catch (Exception ex)
                            {
                                result = new Primavera.Data.PrimaveraResultStructure()
                                {
                                    tipoProblema = "Erro lógico no Primavera",
                                    codigo = 3,
                                    subNivel = "302",
                                    codeLevel = "30",
                                    descricao = "Ocorreu um erro durante a criação da referencia bancaria do aluno " + cliente +
                                        "devido ao seguinte erro: " + ex.Message,
                                    procedimento = "Consultar os técnicos do projecto"
                                };
                            }

                        }
                        else
                        {
                            result = new Primavera.Data.PrimaveraResultStructure()
                            {
                                tipoProblema = "Erro lógico no Primavera",
                                codigo = 3,
                                subNivel = "301",
                                codeLevel = "30",
                                descricao = String.Format("O Codigo do Instuto do aluno {0} não pode estar vazio", cliente),
                                procedimento = "Consultar os técnicos do projecto"
                            };
                        }
                    }
                }
                else
                {
                    //Se não ocorrer erros na gravação dos clientes grava/actualiza os contactos

                    if (result.codigo == 0)
                    {
                        result = motorErp._comercial.GravarContactoCliente(
                            (novoNrCliente == "" || novoNrCliente == null) ? cliente : novoNrCliente,
                            contacto.codigo, contacto.primeiroNome, contacto.ultimoNome, contacto.nomeCompleto,
                            contacto.morada, contacto.localidade, contacto.emailPrincipal, contacto.emailAlternativo,
                            contacto.nrTelefone, contacto.nrTelefoneAlternativo
                        );

                    }
                }
                
                //Se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                if (result.codigo == 0){
                
                    PrimaveraWSLogger.escreveInformacao(result.to_String(), source);
                    motorErp._empresaErp.TerminaTransacao();
                }
                else
                {
                    PrimaveraWSLogger.escreveErro(result.to_String(), source);
                    motorErp._empresaErp.DesfazTransacao();
                }               
                
                return new PrimaveraResultStructure() 
                {  
                    tipoProblema= result.tipoProblema, 
                    codigo = result.codigo,
                    subNivel = result.subNivel,
                    codeLevel = result.codeLevel,
                    descricao = result.descricao,
                    procedimento= result.procedimento
                };

            }
            catch (Exception ex)
            {
                var result = new Primavera.Data.PrimaveraResultStructure()
                {
                    tipoProblema = "Erro lógico no Primavera",
                    codigo = 2,
                    subNivel = "202",
                    codeLevel = "20",
                    descricao = "Erro ao contactar com o metodo actuaizarAluno devido ao : " + ex.Message,
                    procedimento = "Consultar os técnicos do projecto"
                };

               PrimaveraWSLogger.escreveErro(result.ToString(), source);
               motorErp._empresaErp.DesfazTransacao();

                return new PrimaveraResultStructure()
                {
                    tipoProblema = result.tipoProblema,
                    codigo = result.codigo,
                    subNivel = result.subNivel,
                    codeLevel = result.codeLevel,
                    descricao = result.descricao,
                    procedimento = result.procedimento
                };

            }
                        
        }

        public PrimaveraResultStructure GravarFactura(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string nomeUtilizador,string cliente,
            string tipoDoc, Artigo[] listaArtigos)
        {
            string source = "GravarFactura";

            PrimaveraWSLogger.escreveInformacao(string.Format("[{0}] Comunicação com o metodo GravarFactura pelo IP - {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), get_IP()),source);
            try
            {
                //Abre a empresa no Erp Primavera
                motorErp = new MotoresErp();

                var result = motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUtilizador, password);
                
                //Valida se a empresa primavera se encontra aberta
                if (result.codigo == 0)
                {
                    //Inicialiaza a instancia _erpBs em todos os motores que vão ser usados
                    motorErp.inicializaMotores_EmpresaErp();
                }

                //Valida se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                if (result.codigo == 0)
                {
                    motorErp._empresaErp.IniciaTransacao();
                    result = motorErp._comercial.PreencheCabecalho_DocumentoVenda(tipoDoc, DateTime.Now, DateTime.Now,"C",cliente);
                }

                //Valida se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                if (result.codigo == 0)
                {
                    PrimaveraWSLogger.escreveInformacao("inicia o adiciona linha", source);
                    foreach (var artigo in listaArtigos)
                    {
                        if (result.codigo ==0)
                        result = motorErp._comercial.Adiciona_Linhas_DocumentoVenda(artigo.artigo,artigo.quantidade);
                    }
                }

                if (result.codigo == 0)
                {
                    PrimaveraWSLogger.escreveInformacao("grava documento", source);
                    result = motorErp._comercial.Gravar_DocumentoVenda(nomeUtilizador);
                }

                if (result.codigo == 0)
                {
                    //Aqui tem que executar a query para actualizar todos os documentos pendentes e historico com a referencia do cliente
                    string str_query = "update p set p.cdu_referencia = c.cdu_refbnc,p.CDU_Instituto= td.cdu_instituto  from Pendentes p ";
                    str_query += "inner join Clientes c on c.Cliente = p.Entidade and p.TipoEntidade ='C' ";
                    str_query += "inner join TDU_ParametrosISUTC td on td.CDU_Vendedor = c.Vendedor ";
                    str_query += string.Format("where p.Entidade ='{0}' and p.tipoEntidade = 'C' and ( p.cdu_referencia is null or p.CDU_Instituto is null) ; ", cliente);

                    result.descricao += " " + motorErp._empresaErp.Executa_Query_Insert_Update(str_query);

                    str_query = "update p set p.cdu_referencia = c.cdu_refbnc,p.CDU_Instituto= td.cdu_instituto  from historico p ";
                    str_query += "inner join Pendentes pe on pe.IdHistorico = p.Id ";
                    str_query += "inner join Clientes c on c.Cliente = p.Entidade and p.TipoEntidade ='C' ";
                    str_query += "inner join TDU_ParametrosISUTC td on td.CDU_Vendedor = c.Vendedor ";
                    str_query += string.Format("where p.Entidade ='{0}' and p.tipoEntidade = 'C' and ( p.cdu_referencia is null or p.CDU_Instituto is null) ; ", cliente);

                    result.descricao +=" "+ motorErp._empresaErp.Executa_Query_Insert_Update(str_query);
                }
                               
                //Se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                if (result.codigo == 0)
                {

                    PrimaveraWSLogger.escreveInformacao(result.to_String(), source);
                    motorErp._empresaErp.TerminaTransacao();
                }
                else
                {
                    PrimaveraWSLogger.escreveErro(result.to_String(), source);
                    motorErp._empresaErp.DesfazTransacao();
                }

                return new PrimaveraResultStructure()
                {
                    tipoProblema = result.tipoProblema,
                    codigo = result.codigo,
                    subNivel = result.subNivel,
                    codeLevel = result.codeLevel,
                    descricao = result.descricao,
                    procedimento = result.procedimento
                };


            }
            catch (Exception ex)
            {
                var result = new Primavera.Data.PrimaveraResultStructure()
                {
                    tipoProblema = "Erro lógico no Primavera",
                    codigo = 2,
                    subNivel = "202",
                    codeLevel = "20",
                    descricao = "Erro ao contactar com o metodo GravarFactura devido ao : " + ex.Message,
                    procedimento = "Consultar os técnicos do projecto"
                };

                PrimaveraWSLogger.escreveErro(result.ToString(),source);
                
                motorErp._empresaErp.DesfazTransacao();
                return new PrimaveraResultStructure()
                {
                    tipoProblema = result.tipoProblema,
                    codigo = result.codigo,
                    subNivel = result.subNivel,
                    codeLevel = result.codeLevel,
                    descricao = result.descricao,
                    procedimento = result.procedimento
                };
            }
        }

        public PrimaveraResultStructure CriaOuActualizarContrato(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string nomeUtilizador, string cliente,
            string tipoDoc, string semestreId, Artigo[] listaArtigos)
        {
            string source = "CriaOuActualizarContrato";

            PrimaveraWSLogger.escreveInformacao(string.Format("[{0}] Comunicação com o metodo CriaOuActualizarContrato pelo IP - {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), get_IP()), source);

            try
            {
                //Abre a empresa no Erp Primavera
                motorErp = new MotoresErp();

                var result = motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUtilizador, password);

                //Valida se a empresa primavera se encontra aberta
                if (result.codigo == 0)
                {
                    //Inicialiaza a instancia _erpBs em todos os motores que vão ser usados
                    motorErp.inicializaMotores_EmpresaErp();
                }

                //Valida se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                if (result.codigo == 0)
                {
                    motorErp._empresaErp.IniciaTransacao();
                    result = motorErp._comercial.PreencheCabecalho_DocumentoVenda(tipoDoc, DateTime.Now, DateTime.Now, "C", cliente);
                }

                //Valida se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                if (result.codigo == 0)
                {
                    
                    foreach (var artigo in listaArtigos)
                    {
                        if (result.codigo == 0)
                            result = motorErp._comercial.Adiciona_Linhas_DocumentoVenda(artigo.artigo, artigo.quantidade);
                    }
                }

                if (result.codigo == 0)
                {
                    
                    result = motorErp._comercial.Gravar_DocumentoVenda(nomeUtilizador,semestreId);
                }

                if (result.codigo == 0)
                {
                    result = motorErp._comercial.CriaAvenca();
                }

                if (result.codigo == 0)
                {
                    //Aqui tem que executar a query para actualizar todos os documentos pendentes e historico com a referencia do cliente
                    string str_query = "update p set p.cdu_referencia = c.cdu_refbnc,p.CDU_Instituto= td.cdu_instituto  from Pendentes p ";
                    str_query += "inner join Clientes c on c.Cliente = p.Entidade and p.TipoEntidade ='C' ";
                    str_query += "inner join TDU_ParametrosISUTC td on td.CDU_Vendedor = c.Vendedor ";
                    str_query += string.Format("where p.Entidade ='{0}' and p.tipoEntidade = 'C' and ( p.cdu_referencia is null or p.CDU_Instituto is null) ; ", cliente);

                    result.descricao += " " + motorErp._empresaErp.Executa_Query_Insert_Update(str_query);

                    str_query = "update p set p.cdu_referencia = c.cdu_refbnc,p.CDU_Instituto= td.cdu_instituto  from historico p ";
                    str_query += "inner join Pendentes pe on pe.IdHistorico = p.Id ";
                    str_query += "inner join Clientes c on c.Cliente = p.Entidade and p.TipoEntidade ='C' ";
                    str_query += "inner join TDU_ParametrosISUTC td on td.CDU_Vendedor = c.Vendedor ";
                    str_query += string.Format("where p.Entidade ='{0}' and p.tipoEntidade = 'C' and ( p.cdu_referencia is null or p.CDU_Instituto is null) ; ", cliente);

                    result.descricao += " " + motorErp._empresaErp.Executa_Query_Insert_Update(str_query);
                }




                //Se não ocorrer erros na abertura da empresa grava/actualiza o aluno
                if (result.codigo == 0)
                {

                    PrimaveraWSLogger.escreveInformacao(result.to_String(), source);
                    motorErp._empresaErp.TerminaTransacao();
                }
                else
                {
                    PrimaveraWSLogger.escreveErro(result.to_String(), source);
                    motorErp._empresaErp.DesfazTransacao();
                }

                return new PrimaveraResultStructure()
                {
                    tipoProblema = result.tipoProblema,
                    codigo = result.codigo,
                    subNivel = result.subNivel,
                    codeLevel = result.codeLevel,
                    descricao = result.descricao,
                    procedimento = result.procedimento
                };


            }
            catch (Exception ex)
            {
                var result = new Primavera.Data.PrimaveraResultStructure()
                {
                    tipoProblema = "Erro lógico no Primavera",
                    codigo = 2,
                    subNivel = "202",
                    codeLevel = "20",
                    descricao = "Erro ao contactar com o metodo GravarFactura devido ao : " + ex.Message,
                    procedimento = "Consultar os técnicos do projecto"
                };

                PrimaveraWSLogger.escreveErro(result.ToString(), source);

                motorErp._empresaErp.DesfazTransacao();
                return new PrimaveraResultStructure()
                {
                    tipoProblema = result.tipoProblema,
                    codigo = result.codigo,
                    subNivel = result.subNivel,
                    codeLevel = result.codeLevel,
                    descricao = result.descricao,
                    procedimento = result.procedimento
                };
            }
        }

        public PrimaveraResultStructure ConsultaConta(int tipoPlataforma, string codEmpresa, string codUtilizador, string password, string cliente)
        {
            string source = "ConsultaConta";

            PrimaveraWSLogger.escreveInformacao(string.Format("[{0}] Comunicação com o metodo ConsultaConta pelo IP - {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), get_IP()), source);
            try
            {
                //Abre a empresa no Erp Primavera
                motorErp = new MotoresErp();
               
                var result = motorErp._empresaErp.AbreEmpresaPrimavera(tipoPlataforma, codEmpresa, codUtilizador, password);

                if (result.codigo == 0)
                {
                    motorErp.inicializaMotores_EmpresaErp();

                    var lista = motorErp._comercial.ConsultaConta(cliente);

                    List<Pendente> listaPendentes = new List<Pendente>();

                    foreach (var pendente in lista)
                    {
                        listaPendentes.Add(
                            new Pendente()
                            {
                                cliente = pendente.cliente,
                                dataCriacao = pendente.dataCriacao,
                                dataVencimento = pendente.dataVencimento,
                                tipoDoc = pendente.tipoDoc,
                                numDoc = pendente.numDoc,
                                serie = pendente.serie,
                                moeda = pendente.moeda,
                                valorPendente = pendente.valorPendente,
                                valorTotal = pendente.valorTotal
                            }
                        );
                    }

                    return new PrimaveraResultStructure()
                    {
                        tipoProblema = result.tipoProblema,
                        codigo = result.codigo,
                        subNivel = result.subNivel,
                        codeLevel = result.codeLevel,
                        descricao = result.descricao,
                        procedimento = result.procedimento,
                        pendentes = listaPendentes.ToArray()
                    };


                }
                else
                {
                    PrimaveraWSLogger.escreveErro(result.to_String(), source);

                    return new PrimaveraResultStructure()
                    {
                        tipoProblema = result.tipoProblema,
                        codigo = result.codigo,
                        subNivel = result.subNivel,
                        codeLevel = result.codeLevel,
                        descricao = result.descricao,
                        procedimento = result.procedimento
                    };
                }

            }
            catch (Exception ex)
            {
                string descricao = string.Format("Ocorreu um erro na consulta do saldo do cliente {0} devido ao seguinte erro: {1}", cliente, ex.Message);
                
                PrimaveraWSLogger.escreveErro(descricao,source);

                return new PrimaveraResultStructure()
                {
                    tipoProblema = "Erro lógico no Primavera",
                    codigo = 2,
                    subNivel = "202",
                    codeLevel = "20",
                    descricao = "Erro ao contactar com o metodo consultaSaldo devido ao : " + ex.Message,
                    procedimento = "Consultar os técnicos do projecto"
                };
            }
            
        }

        #region Funções

        private string get_IP()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            return ip;

        }
        #endregion

        #region Gerador de Referencias

        /// <summary>
        /// Coloca os zeros com base no tamanho
        /// </summary>
        /// <param name="tamanho"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        private string colocaZeros(int tamanho, string valor)
        {
            int valorLength = valor.Length;

            if (valorLength >= tamanho)
            {
                return valor;
            }

            string result = new string('0', tamanho - valorLength) + valor;

            return result;
        }

        private string geraReferencia(String entidade, string tipoDoc, long codigo, int numPrestacao)
        {

            string sReferenciaSCheck = "";

            string sReferenciaSCheckCalc = "";

            int I = 1;
            int P = 0;
            int S = 0;
            int numPos = 0;
            int CheckDigitReferencia = 0;

            sReferenciaSCheck = tipoDoc.Substring(0, 1) + colocaZeros(6, codigo.ToString()) + colocaZeros(2, numPrestacao.ToString());

            sReferenciaSCheckCalc = colocaZeros(5, entidade) + sReferenciaSCheck;

            while (I <= 15)
            {
                if (I == 1)
                {
                    P = 0;
                }
                if (I < 15)
                {
                    numPos = int.Parse(sReferenciaSCheckCalc.Substring(I - 1, 1));
                }
                else
                {
                    numPos = 0;
                }

                S = P + numPos;
                P = (S * 10) % 97;

                I++;
            }

            CheckDigitReferencia = 98 - P;

            return sReferenciaSCheck + colocaZeros(2, CheckDigitReferencia.ToString());
        }
                
        #endregion
    }
}
