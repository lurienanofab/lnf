using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IServiceLogRepository
    {
        IEnumerable<IServiceLog> GetServiceLogs(int limit, int skip = 0, Guid? id = null, string service = null, string subject = null);
        void InsertServiceLog(IServiceLog model);
        bool UpdateServiceLog(Guid id, string data);
    }
}
