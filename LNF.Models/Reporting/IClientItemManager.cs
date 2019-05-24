using System;
using System.Collections.Generic;

namespace LNF.Models.Reporting
{
    public interface IClientItemManager
    {
        IEnumerable<IReportingClient> SelectCurrentActiveClients();
        IEnumerable<IReportingClient> SelectActiveClients(DateTime period);
        IEnumerable<IReportingClient> SelectActiveManagers(DateTime period);
        IReportingClient GetManagerFor(int clientId, DateTime period);
        IReportingClient CreateClientItem(int clientId);
    }
}
