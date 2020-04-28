using LNF.Reporting;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class ClientManagerLogRepository : ApiClient, IClientManagerLogRepository
    {
        public IEnumerable<IClientManagerLog> SelectByManager(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientManagerLog> SelectByPeriod(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }
    }
}
