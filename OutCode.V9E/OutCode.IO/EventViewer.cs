using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.IO
{
    public class EVLog
    {
        private static string EventSource = Assembly.GetExecutingAssembly().GetName().Name;
        private const string EventLog = "Application";
        private static EventLog _logger = null;

        /// <summary>
        /// Chage Event Source Name
        /// </summary>
        public static string SourceName {
            get { return EventSource; }
            set { if (!String.IsNullOrWhiteSpace(value)) EventSource = value; }
        }   

        /// <summary>
        /// Log an Error
        /// </summary>
        /// <param name="format">String to log (with format options)</param>
        /// <param name="args">Aguments to format string</param>
        public static void Error(string format, params object[] args)
        {
            System.Media.SystemSounds.Exclamation.Play();
            WriteAsync(EventLogEntryType.Error, format, args);
        }

        public static void Information(string format, params object[] args)
        {
            WriteAsync(EventLogEntryType.Information, format, args);
        }


        public static void Warning(string format, params object[] args)
        {
            WriteAsync(EventLogEntryType.Warning, format, args);
        }

        /// <summary>
        /// Escreve no Log de forma assincrona
        /// </summary>
        /// <param name="entryType">Tipo de mensagem</param>
        /// <param name="format">String formatada</param>
        /// <param name="args">Argumentos para o o formato da string</param>
        private static void WriteAsync(EventLogEntryType entryType, string format, params object[] args)
        {
            new Task(() => { Write(entryType, format, args); }).Start();
        }

        /// <summary>
        /// Escreve Informação para o Log
        /// </summary>
        /// <param name="entryType"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private static void Write(EventLogEntryType entryType, string format, params object[] args)
        {
            string entry = _Format(format, args);

            try
            {
                if (_logger == null) InitLogger();

                _logger.WriteEntry(entry, entryType, 1);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Could not write to event log: {0}{1}{2}", entry, System.Environment.NewLine, ex.ToString()));
            }

        }

        /// <summary>
        /// Formatação de strings
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string _Format(string format, object[] args)
        {
            if ((args == null) || (args.Length == 0))
                return format;
            try
            {
                return String.Format(format, args);
            }
            catch (Exception e)
            {
                return format + " [Format Exception:" + e.Message + "]";
            }
        }

        /// <summary>
        /// Inicializa o Log
        /// </summary>
        private static void InitLogger()
        {
            if (!System.Diagnostics.EventLog.SourceExists(EventSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(EventSource, EventLog);
            }
            _logger = new EventLog() { Source = EventSource, Log = EventLog };
        }
    }
}
