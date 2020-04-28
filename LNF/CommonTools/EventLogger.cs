using LNF.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LNF.CommonTools
{
    // 2008-03-17 Write logging message to Event Viewer
    // This is created to monitor the activity inside the CommonTools code
    public class EventLogger
    {
        public static void WriteToWindowsEvent(string Message)
        {
            string Log = "Application";
            string Source = "LNF Application";

            EventLog objEventLog = new EventLog();

            // Register the App as an Event Source
            if (!EventLog.SourceExists(Source))
                EventLog.CreateEventSource(Source, Log);

            objEventLog.Source = Source;
            objEventLog.WriteEntry(Message, EventLogEntryType.Information, 1, 1);
        }

        public static void WriteToSystemLog(int clientId, Guid messageGuid, LogMessageTypes messageType, string message)
        {
            ServiceProvider.Current.Log.WriteToSystemLog(clientId, messageGuid, messageType, message);
        }

        public static void WriteToHTML(string message)
        {
            string p = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\log.html";
            FileStream fs = File.Open(p, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(message);
            sw.Close();
        }
    }

}
