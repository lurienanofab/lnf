using LNF.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Models.Reporting.Individual
{
    public class ManagerUsageSummaryAccount : ManagerUsageSummaryItem
    {
        public IEnumerable<ClientItem> Clients { get; set; }

        public override string Members
        {
            get
            {
                if (Clients == null || Clients.Count() == 0)
                    return string.Empty;
                else
                    return string.Join("; ", Clients.Select(x => ClientModel.GetDisplayName(x.LName, x.FName)));
            }
        }
    }
}