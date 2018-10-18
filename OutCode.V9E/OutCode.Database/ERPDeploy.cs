using ADODB;
using OutCode.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutCode.Database
{
    public class ERPDeploy
    {

        private static string CMDSeparator = string.Format("{0}GO{0}", Environment.NewLine);

        #region EVENTS

        //*** EVENTO PARA NOTIFICAÇÂO
        public delegate void NotifyEventHandler(object sender, string message, bool BOP, bool EOP);

        public static event NotifyEventHandler OnNotify = delegate { };

        public static void Notify(bool BeginOfProcess, bool EndOfProcess,string FormarString, params object[] args)
        {

            NotifyEventHandler handler = OnNotify;

            handler(null, string.Format (FormarString,args ),BeginOfProcess ,EndOfProcess );
        }

        //*** EVENTO PARA NOTIFICAÇÂO DEALTERAÇÃO DE VERSAO
        public delegate void VersionsHandler(object sender, double platformVersion, double databaseVersion);

        public static event VersionsHandler OnVersionsChange = delegate { };

        public static void NotifyVersions() {

            VersionsHandler handler = OnVersionsChange;

            handler(null, VersaoPlataforma, VersaoBaseDados);

        } 

        #endregion


        #region PROPERTIES

        private static double _VersaoPlataforma;

        public static double VersaoPlataforma
        {
            get { return _VersaoPlataforma; }
            set { _VersaoPlataforma = value; }
        }

        private static double _versaoDB;

        public static double VersaoBaseDados
        {
            get { return _versaoDB; }
            set { _versaoDB = value; }
        }

        private static string _appName;

        public static string ApplicationName
        {
            get { return _appName; }
            set { _appName = value; }
        }

        #endregion


        #region PUBLIC METHODS

        public static void PrepareDatabase(ISqlExecute ERP, Assembly RefAssembly, string AppName)
        {

            //try
            //{

            ApplicationName = AppName;

            Notify(true, false, "INICIO DA ACTUALIZAÇÂO DE ESPECIFICOS");

            PrepareData(ERP);
            ERP.SetVersions(VersaoPlataforma, VersaoBaseDados); // Versao inicial

            UpgradePlataforma(ERP, RefAssembly);
            ERP.SetVersions(VersaoPlataforma, VersaoBaseDados); // UPG Plafaforma

            UpgradeDataBase(ERP, RefAssembly);
            ERP.SetVersions(VersaoPlataforma, VersaoBaseDados); // UPG BD

            Notify(false, true, "FIM DA ACTUALIZAÇÂO DE ESPECIFICOS");

            //}
            //catch (Exception err)
            //{
            //    if (!Silent)
            //    {
            //        throw new Exception("Erro ao preparar a base de dados!", err);
            //    }
            //}
        }

        #endregion


        #region HELPERS

        /// <summary>
        /// Prepara a informação inicial ao processo de upgrade
        /// </summary>
        /// <param name="ERP">Interface para execução de Queries</param>
        private static void PrepareData(ISqlExecute ERP)
        {

            string strSql;
            DataTable dt;

            try
            {
                strSql = string.Format("select [Application],[Description],[VersionDB] from [PLT_Applications]");

                dt = ERP.Consulta(strSql);

                if (dt==null|| dt.Rows.Count == 0) {

                    _versaoDB = 0;
                    _VersaoPlataforma = 0;

                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["Application"].ToString().Equals(ApplicationName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            _versaoDB = Convert.ToDouble(dr["VersionDB"]);
                        }
                        else if(dr["Application"].ToString().Equals("PLT", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _VersaoPlataforma = Convert.ToDouble(dr["VersionDB"]);
                        }

                    }
                }

            }
            catch (Exception err)
            {
                EVLog.Error("**** PREPAREDATA: {0}", err.Message);

                try
                {

                        ERP.ExecutaSql(string.Format("INSERT INTO PLT_Applications([Application],[Description],[VersionDB],[UpgradeDate])values('PLT','Plataforma',0,{0}) ", DateTime.Now.ToString("yyyy-MM-dd") ));
                        ERP.ExecutaSql(string.Format("INSERT INTO PLT_Applications([Application],[Description],[VersionDB],[UpgradeDate])values('{0}','Plataforma',0,{1}) ", ApplicationName, DateTime.Now));
               
                }
                catch (Exception ex)
                {
                    EVLog.Error("**** PREPAREDATA: {0}", ex.Message);
                }

                _versaoDB = 0;
                _VersaoPlataforma = 0;

            }

            Notify(false,false,"Ponto inicial: Plataforma: {0} Base de dados: {1}",_VersaoPlataforma,VersaoBaseDados  ); 
        }

        /// <summary>
        /// UPGRADE DAS SCRIPTS DE PLATAFORMA
        /// </summary>
        /// <param name="Engine">Interface de execução de quesries</param>
        /// <param name="RefAssembly">Assembly com as queries a executar</param>
        /// <returns></returns>
        private static bool UpgradePlataforma(ISqlExecute Engine,Assembly RefAssembly)
        {
            bool blnResult = true;

            try
            {
                string data;

                int VersaoMinima;
                int VersaoDepoisActualizacao;

                List<string> info;

                foreach (string str in OutCode.Interop.Resources.GetResourcesList(RefAssembly, "PLT-"))
                {
                    info = str.Substring(0, str.Length - 4).Split('-').ToList<string>();

                    VersaoMinima = Convert.ToInt32(info[1]);

                    VersaoDepoisActualizacao = Convert.ToInt32(info[2]);


                    if (VersaoMinima == _VersaoPlataforma)
                    {

                        /*
                            ACTUALIZAÇÃO DA PLATAFORMA SQL
                         */

                        EVLog.Information("Upgrading: Platform from {0} to {1}.", VersaoPlataforma, VersaoDepoisActualizacao);

                        Notify(false,false,"A Actualizar a Plataforma de {0} para {1}...", _VersaoPlataforma, VersaoDepoisActualizacao);

                        data = OutCode.Interop.Resources.LoadTextResourceData(RefAssembly, str);

                        try
                        {
                            foreach (string sql in Regex.Split(data,CMDSeparator, RegexOptions.IgnoreCase))
                            {
                                Engine.ExecutaSql(sql, true);
                            }

                            if (_VersaoPlataforma == 0)
                            {
                                Engine.ExecutaSql(string.Format("INSERT INTO PLT_Applications([Application],[Description],[VersionDB])values('PLT','Plataforma',0) "), true);
                            }


                            Engine.ExecutaSql(string.Format("UPDATE PLT_Applications set VersionDB={0},UpgradeDate='{1}' where [Application]='{2}'",
                                                               VersaoDepoisActualizacao, DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss"), "PLT"), true);

                            

                            _VersaoPlataforma = VersaoDepoisActualizacao;

                            NotifyVersions();
                        }
                        catch (Exception err)
                        {
                            EVLog.Error("UpgradePlataforma:{0}{1}{0}{2}", Environment.NewLine, err.Message, data);
                        }

                    }
                }

            }
            catch (Exception err)
            {
                blnResult = false;
                EVLog.Error("Upgrade Plataforma:{0}", err.Message);

            }
            return (blnResult);
        }

        private static void UpgradeDataBase(ISqlExecute ERP, Assembly RefAssembly)
        {
            try
            {
                string data;

                int VersaoInicial;
                int VersaoAposScript;

                List<string> info;

                foreach (string str in OutCode.Interop.Resources.GetResourcesList(RefAssembly, "UPD-"))
                {
                    info = str.Substring(0, str.Length - 4).Split('-').ToList<string>();

                    VersaoInicial = Convert.ToInt32(info[1]);

                    VersaoAposScript = Convert.ToInt32(info[2]);

                    if (VersaoInicial == VersaoBaseDados )
                    {

                        EVLog.Information("Upgrading Platforma from {0} to {1}.", _versaoDB, VersaoAposScript);

                        Notify(false, false, "A actualizar a estrutura da base de dados de {0} para {1}...", VersaoInicial, VersaoAposScript);

                        data = OutCode.Interop.Resources.LoadTextResourceData(RefAssembly, str);

                        try
                        {
                            foreach (string sql in Regex.Split(data, CMDSeparator, RegexOptions.IgnoreCase))
                            {
                                ERP.ExecutaSql(sql, false);
                            }

                            if (_versaoDB == 0)
                            {
                                ERP.ExecutaSql(string.Format("INSERT INTO PLT_Applications([Application],[Description],[VersionDB])values('{0}','base de dados',0) ", ApplicationName), true);
                            }

                             ERP.ExecutaSql(string.Format("UPDATE PLT_Applications set VersionDB={0},UpgradeDate='{1}' where [Application]='{2}'",
                                                                    VersaoAposScript, DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss"), ApplicationName), true);

                            VersaoBaseDados = VersaoAposScript;

                            NotifyVersions();
                        }
                        catch (Exception err)
                        {
                            EVLog.Error("Upgrade Database:{0}{1}{0}{2}", Environment.NewLine, err.Message, data);
                        }

                    }

                }

            }
            catch (Exception err)
            {
                EVLog.Error("UpgradePlataforma:{0}", err.Message);
                throw;
            }
        }

        #endregion
    }
}

