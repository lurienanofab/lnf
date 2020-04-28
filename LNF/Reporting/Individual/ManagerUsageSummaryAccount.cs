using System.Collections.Generic;
using System.Linq;

namespace LNF.Reporting.Individual
{
    public class ManagerUsageSummaryAccount : ManagerUsageSummaryItem
    {
        public IEnumerable<IReportingClient> Clients { get; set; }

        public override string Members
        {
            get
            {
                if (Clients == null || Clients.Count() == 0)
                    return string.Empty;
                else
                    return string.Join("; ", Clients.Select(x => Data.Clients.GetDisplayName(x.LName, x.FName)));
            }
        }
    }
}