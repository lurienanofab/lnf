using LNF.Repository;
using System;
using System.Data;
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
            objEventLog.WriteEntry(Message, EventLogEntryType.Information, 1, 1);
        }

        public static void WriteToSystemLog(int clientId, Guid messageGuid, LogMessageTypes messageType, string message)
        {
            string sql = "INSERT SystemLog (ClientID, LogMessageGUID, LogMessageDateTime, LogMessageType, LogMessageText) VALUES (@ClientID, @LogMessageGUID, GETDATE(), @LogMessageType, @LogMessageText)";

            DA.Command(CommandType.Text).Param(new
            {
                ClientID = Utility.DBNullIf(clientId, clientId == 0),
                LogMessageGUID = messageGuid,
                LogMessageType = LogMessageTypeToString(messageType),
                LogMessageText = message
            }).ExecuteNonQuery(sql);
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
