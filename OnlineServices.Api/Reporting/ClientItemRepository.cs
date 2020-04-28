using LNF.Reporting;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Reporting
{
    public class ClientItemRepository : ApiClient, IClientItemRepository
    {
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
