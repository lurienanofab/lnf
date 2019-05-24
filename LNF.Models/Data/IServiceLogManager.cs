using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IServiceLogManager
    {
        IEnumerable<IServiceLog> GetServiceLogs(int limit, int skip = 0, Guid? id = null, string service = null, string subject = null);
        void InsertServiceLog(IServiceLog model);
        bool UpdateServiceLog(Guid id, string data);
    }
}
