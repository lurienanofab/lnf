using LNF.Reporting;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class ClientItemRepository : ApiClient, IClientItemRepository
    {
        internal ClientItemRepository(IRestClient rc) : base(rc) { }

        public IReportingClient CreateClientItem(int clientId)
        {
            throw new NotImplementedException();
        }

        public IReportingClient GetManagerFor(int clientId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReportingClient> SelectActiveClients(DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReportingClient> SelectActiveManagers(DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReportingClient> SelectCurrentActiveClients()
        {
            throw new NotImplementedException();
        }
    }
}
