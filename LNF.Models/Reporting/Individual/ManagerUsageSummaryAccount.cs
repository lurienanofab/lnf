using System.Collections.Generic;

namespace LNF.Models.Reporting.Individual
{
    public class ManagerUsageSummaryAccount : ManagerUsageSummaryItem
    {
        public IEnumerable<ClientItem> Clients { get; set; }
    }
}