using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Data;
using LNF.Logging;
using NHibernate;
using System;

namespace LNF.Impl.Logging
{
    public class LoggingService : LoggingServiceBase
    {
        protected readonly ISession _session;

        public LoggingService(ISessionManager mgr) : base(mgr) { }

        public override int Purge(DateTime cutoff)
        {
            string sql = "DELETE dbo.ServiceLog WHERE LogDateTime < :cutoff";
            var query = _session.CreateSQLQuery(sql);
            query.SetParameter("cutoff", cutoff);
            var result = query.ExecuteUpdate();
            return result;
        }

        protected override void AddMessage(LogMessage message)
        {
            //save it to the database
            var serviceLog = new ServiceLog()
            {
                MessageID = message.MessageID,
                ServiceName = Name,
                LogDateTime = DateTime.Now,
                LogLevel = message.Level.ToString(),
                LogSubject = message.Subject,
                LogMessage = message.Body
            };

            serviceLog.SetData(message.Data);

            _session.Save(serviceLog);
        }
    }
}
