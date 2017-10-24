using LNF.Logging;
using LNF.Repository;
using LNF.Repository.Data;
using System;

namespace LNF.Impl.Logging
{
    public class ServiceLogProvider : LogProviderBase
    {
        protected override LogBase NewLog()
        {
            return new Log();
        }
    }

    public class Log : LogBase
    {
        protected override void OnWrite(LogMessage message)
        {
            //save it to the database
            var serviceLog = new ServiceLog()
            {
                LogDateTime = DateTime.Now,
                LogLevel = message.Level.ToString(),
                LogSubject = message.Subject,
                LogMessage = message.Body,
                ServiceName = Providers.Log.Name,
                MessageID = message.MessageID
            };

            serviceLog.SetData(message.Data);

            DA.Current.Insert(serviceLog);
        }

        protected override int OnPurge(DateTime cutoff)
        {
            string sql = "DELETE dbo.ServiceLog WHERE LogDateTime < @cutoff";
            var count = DA.Current.SqlQuery(sql, new { cutoff }).Update();
            return count;
        }
    }
}
