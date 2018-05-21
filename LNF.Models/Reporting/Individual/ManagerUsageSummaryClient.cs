using System.Collections.Generic;
using System.Linq;

namespace LNF.Models.Reporting.Individual
{
    public class ManagerUsageSummaryClient : ManagerUsageSummaryItem
    {
        public IEnumerable<ReportingAccountItem> Accounts { get; set; }

        public override string Members
        {
            get
            {
                if (Accounts == null || Accounts.Count() == 0)
                    return string.Empty;
                else
                    return string.Join("; ", Accounts.Select(GetAccountName));
            }
        }

        private string GetAccountName(ReportingAccountItem acct)
        {
            if (acct.IsExternal)
                return acct.AccountName;
            else
                return string.Format("{0}/{1}", acct.ShortCode, acct.Project);
        }
    }
}