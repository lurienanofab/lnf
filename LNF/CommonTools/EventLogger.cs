using LNF.Repository;
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
        public enum LogMessageTypes
        {
            Error = 1,
            Warning = 2,
            Info = 3
        }

        public static void WriteToWindowsEvent(string Message)
        {
            string Log = "Application";
            string Source = "LNF Application";

            EventLog objEventLog = new EventLog();

            // Register the App as an Event Source
            if (!EventLog.SourceExists(Source))
                EventLog.CreateEventSource(Source, Log);

            objEventLog.Source = Source;
            objEventLog.WriteEntry(Message, EventLogEntryType.Information, 1, (short)1);
        }

        public static void WriteToSystemLog(int clientId, Guid messageGuid, LogMessageTypes messageType, string message)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                string sql = "INSERT SystemLog (ClientID, LogMessageGUID, LogMessageDateTime, LogMessageType, LogMessageText) VALUES (@ClientID, @LogMessageGUID, GETDATE(), @LogMessageType, @LogMessageText)";
                dba.SelectCommand
                    .ApplyParameters(new
                    {
                        ClientID = (clientId > 0) ? (object)clientId : DBNull.Value,
                        LogMessageGUID = messageGuid,
                        LogMessageType = EventLogger.LogMessageTypeToString(messageType),
                        LogMessageText = message
                    })
                    .CommandTypeText()
                    .ExecuteNonQuery(sql);
            }
        }

        public static void WriteToHTML(string message)
        {
            string p = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\log.html";
            FileStream fs = File.Open(p, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(message);
            sw.Close();
        }

        public static string LogMessageTypeToString(LogMessageTypes MessageType)
        {
            switch (MessageType)
            {
                case LogMessageTypes.Error:
                    return "error";
                case LogMessageTypes.Warning:
                    return "warning";
                case LogMessageTypes.Info:
                    return "info";
                default:
                    return "undefined";
            }
        }
    }

}
