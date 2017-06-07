using System.Collections.Generic;

namespace LNF.Models.Reporting.Individual
{
    public class ManagerUsageSummaryClient : ManagerUsageSummaryItem
    {
        public IEnumerable<AccountItem> Accounts { get; set; }
    }
}