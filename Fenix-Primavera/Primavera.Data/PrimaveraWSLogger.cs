using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Primavera.Data
{
    public class PrimaveraWSLogger
    {
        private static void escreveLog(string message, string sourceName, EventLogEntryType eventType)
        {
            string logName = "WS-SERVICE-PRIMAVERA";

            EventLog elog = new EventLog();

            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);
            }
            elog.Source = sourceName;
            elog.EnableRaisingEvents = true;

            EventLog.WriteEntry(sourceName, message, eventType);
        }

        public static void escreveErro(string msg, string sourceName)
        {
            escreveLog(msg, sourceName, EventLogEntryType.Error);
        }

        public static void escreveInformacao(string msg, string sourceName)
        {
            escreveLog(msg, sourceName, EventLogEntryType.Information);
        }

        public static void escreveAlerta(string msg, string sourceName)
        {
            escreveLog(msg, sourceName, EventLogEntryType.Warning);
        }

        public static void escreveSucesso(string msg, string sourceName)
        {
            escreveLog(msg, sourceName, EventLogEntryType.SuccessAudit);
        }

        public static void escreveFalha(string msg, string sourceName)
        {
            escreveLog(msg, sourceName, EventLogEntryType.FailureAudit);
        }

    }
}
