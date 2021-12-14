using LNF.Reporting;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class ClientManagerLogRepository : ApiClient, IClientManagerLogRepository
    {
        internal ClientManagerLogRepository(IRestClient rc) : base(rc) { }

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
