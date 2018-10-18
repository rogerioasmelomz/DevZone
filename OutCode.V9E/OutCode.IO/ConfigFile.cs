using System;
using OutCode.Security;
using OutCode.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.IO
{
    public class ConfigFile
    {

        #region PUBLICS

        /// <summary>
        /// Returns the name of file in use
        /// </summary>
        public static string FileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_activeIniFile.FileName)) { return ("CSULicense.lic"); }
                else return (_activeIniFile.FileName);
            }
        }

        #endregion

        #region PRIVATES

        private static Dictionary<int, IniFile> _iniFile;

        private static IniFile _activeIniFile = null;

        #endregion

        #region "Contructors"

        /// <summary>
        /// Creates a new file
        /// </summary>
        /// <param name="FileId">File ID to Use</param>
        public static void InitFile(out int FileId)
        {
            InitFile("ConfigFile",out FileId);
        }

        /// <summary>
        /// Creates a new file
        /// </summary>
        /// <param name="FileName">File full name (path)</param>
        /// <param name="FileId">File ID to use</param>
        public static void InitFile(string FileName, out int FileId)
        {

            try
            {
                if (_iniFile == null)
                {
                    _iniFile = new Dictionary<int, IniFile>();
                }

                //_FileName = FileName;

                FileId = _iniFile.Count() + 1;

                _iniFile.Add(FileId, new IniFile(FileName));

                if (_iniFile.TryGetValue(FileId, out _activeIniFile))
                {
                    _activeIniFile.Write("Last Access", DateTime.Now.ToLongDateString(), "SYSTEM");
                }
            }
            catch (Exception err)
            {
                EVLog.Error("ConfigFile Constructor:{0}", err.Message);
                throw;
            }
        }

        #endregion

        #region "IO"

        /// <summary>
        /// Save info in config file
        /// </summary>
        /// <param name="FileId">File ID</param>
        /// <param name="Key">Key</param>
        /// <param name="Value">Value for the key</param>
        /// <param name="Section">file section to use</param>
        /// <param name="Encode">True: Encode information, False: Normal write</param>
        public static void SetKey(int FileId,string Key, string Value, string Section, bool Encode = false)
        {

            string strData;

            try
            {
                if (Encode) { strData = StrCypher.Do(Value, _activeIniFile.FileName  ); }
                else { strData = Value; }

                if (_iniFile.TryGetValue(FileId, out _activeIniFile))
                {
                    _activeIniFile.Write(Key, strData, Section);
                }
            }
            catch (Exception)
            {
               
                throw;
            }
        }

        /// <summary>
        /// Get info from config file
        /// </summary>
        /// <param name="FileId">File ID</param>
        /// <param name="Key">Key to search in file</param>
        /// <param name="Section">Section to search</param>
        /// <param name="Decode">True: Decosded readed string, False: Normal read</param>
        /// <returns></returns>
        public static string GetKey(int FileId,string Key, string Section, bool Decode = false)
        {

            string strData = string.Empty;
            try
            {
                if (_iniFile.TryGetValue(FileId, out _activeIniFile))
                {
                    if (_activeIniFile.KeyExists(Key, Section))
                    {
                        strData = _activeIniFile.Read(Key, Section);
                        if (Decode) { strData = StrCypher.Undo(strData, _activeIniFile.FileName); }
                    }
                }

                return (strData);
            }
            catch (Exception ex)
            {
                EVLog.Error("Error reading #key:\nfile: {0}\n\n{1}",_activeIniFile.FileName ,ex.Message );
                throw;
            }

        }

        /// <summary>
        /// Shows property editor
        /// </summary>
        /// <param name="Properties">Object to read from</param>
        /// <param name="throwErrors">True: erros are throwed; False: Erros stays internaly</param>
        public static void Edit(object Properties, bool throwErrors=true)
        {
            frmParametros frmEdit = null;

            try
            {
                frmEdit = new frmParametros();
                frmEdit.Parametros = Properties;
                frmEdit.ShowDialog();
            }
            catch (Exception)
            {
                if(throwErrors ) throw;
            }
            finally
            {
                frmEdit.Dispose();
                frmEdit = null;
            }

        }
        #endregion

    }
}
