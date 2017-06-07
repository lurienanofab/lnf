using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Models.Reporting.Individual
{
    public class ManagerUsageSummary
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public object LName { get; set; }
        public object FName { get; set; }
        public DateTime Period { get; set; }
        public IEnumerable<ManagerUsageSummaryAccount> Accounts { get; set; }
        public IEnumerable<ManagerUsageSummaryClient> Clients { get; set; }
        public bool ShowSubsidyColumn { get; set; }

        public bool HasData
        {
            get
            {
                return Accounts != null && Accounts.Count() > 0;
            }
        }

        public double TotalNetCharge
        {
            get
            {
                if (Accounts != null && Accounts.Count() > 0)
                    return Accounts.Sum(x => x.NetCharge);
                else
                    return 0;
            }
        }

        public double TotalUsageCharge
        {
            get
            {
                if (Accounts != null && Accounts.Count() > 0)
                    return Accounts.Sum(x => x.UsageCharge);
                else
                    return 0;
            }
        }

        public double TotalSubsidy
        {
            get
            {
                if (Accounts != null && Accounts.Count() > 0)
                    return Accounts.Sum(x => x.Subsidy);
                else
                    return 0;
            }
        }
    }
}