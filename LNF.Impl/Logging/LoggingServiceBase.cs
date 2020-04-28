using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Logging;
using NHibernate.Transform;
using System;
using System.Linq;

namespace LNF.Impl.Logging
{
    public abstract class LoggingServiceBase : RepositoryBase, ILoggingService
    {
        public LoggingServiceBase(ISessionManager mgr) : base(mgr)
        {
            Name = Configuration.Current.Log.Name;
            Enabled = Configuration.Current.Log.Enabled;
        }

        public string Name { get; }

        public bool Enabled { get; }

        public virtual int Purge(DateTime cutoff)
        {
            return -1; // purging is disabled by default
        }

        public void Write(LogMessage message)
        {
            if (Enabled)
                AddMessage(message);
        }

        protected abstract void AddMessage(LogMessage message);

        protected virtual string FormatMessage(LogMessage message)
        {
            return $"{Name}:{message.Level}> [{message.Subject}] {message.Body} ({message.MessageID})";
        }

        public void WriteToSystemLog(int clientId, Guid messageGuid, LogMessageTypes messageType, string message)
        {
            string sql = "INSERT SystemLog (ClientID, LogMessageGUID, LogMessageDateTime, LogMessageType, LogMessageText) VALUES (:ClientID, :LogMessageGUID, GETDATE(), :LogMessageType, :LogMessageText)";

            Session.CreateSQLQuery(sql)
                .SetParameter("ClientID", clientId > 0, clientId)
                .SetParameter("LogMessageGUID", messageGuid)
                .SetParameter("LogMessageType", LogMessageTypeToString(messageType))
                .SetParameter("LogMessageText", message)
                .ExecuteUpdate();
        }

        public Guid SetPassError(string errorMsg, int clientId, string clientName, string filePath)
        {
            return Session.CreateSQLQuery("EXEC dbo.PassError_Select @Action = 'Set', @ErrorMsg = :ErrorMsg, @ClientID = :ClientID, @ClientName = :ClientName, @FilePath = :FilePath")
                .SetParameter("ErrorMsg", !string.IsNullOrEmpty(errorMsg), errorMsg)
                .SetParameter("ClientID", clientId > 0, clientId)
                .SetParameter("ClientName", !string.IsNullOrEmpty(clientName), clientName)
                .SetParameter("FilePath", !string.IsNullOrEmpty(filePath), filePath)
                .UniqueResult<Guid>();
        }

        public IPassError GetPassError(Guid errorId)
        {
            return Session.CreateSQLQuery("EXEC dbo.PassError_Select @Action = 'Get', @ErrorID = :ErrorID")
                .SetParameter("ErrorID", errorId)
                .SetResultTransformer(Transformers.AliasToBean<PassErrorItem>())
                .List<PassErrorItem>().FirstOrDefault();
        }

        private string LogMessageTypeToString(LogMessageTypes MessageType)
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
