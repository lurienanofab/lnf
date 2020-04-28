using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class ServiceLogRepository : RepositoryBase, IServiceLogRepository
    {
        public ServiceLogRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IServiceLog> GetServiceLogs(int limit, int skip = 0, Guid? id = null, string service = null, string subject = null)
        {
            if (limit > 100)
                throw new ArgumentOutOfRangeException("The parameter 'limit' must not be greater than 100.");

            var query = Session.Query<ServiceLog>().Where(x =>
                (service != null ? x.ServiceName == service : true)
                && (subject != null ? x.LogSubject == subject : true)
                && (id.HasValue ? x.MessageID == id.Value : true)
            );

            return query.Skip(skip).Take(limit).CreateModels<IServiceLog>();
        }

        public void InsertServiceLog(IServiceLog model)
        {
            var item = new ServiceLog()
            {
                ServiceName = model.ServiceName,
                LogDateTime = model.LogDateTime,
                LogSubject = model.LogSubject,
                LogLevel = model.LogLevel,
                LogMessage = model.LogMessage,
                MessageID = model.MessageID,
                Data = model.Data
            };

            Session.Save(item);

            model.ServiceLogID = item.ServiceLogID;
        }

        public bool UpdateServiceLog(Guid id, string data)
        {
            var item = Session.Query<ServiceLog>().FirstOrDefault(x => x.MessageID == id);

            if (item != null)
            {
                item.AppendData(data);
                Session.SaveOrUpdate(item);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
