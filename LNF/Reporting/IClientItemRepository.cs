using System;
using System.Collections.Generic;

namespace LNF.Reporting
{
    public interface IClientItemRepository
    {
        IEnumerable<IReportingClient> SelectCurrentActiveClients();
        IEnumerable<IReportingClient> SelectActiveClients(DateTime period);
        IEnumerable<IReportingClient> SelectActiveManagers(DateTime period);
        IReportingClient GetManagerFor(int clientId, DateTime period);
        IReportingClient CreateClientItem(int clientId);
    }
}
