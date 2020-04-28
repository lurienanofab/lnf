using System.Collections.Generic;

namespace LNF.Reporting
{
    public class ReportingAccountItemEqualityComparer : IEqualityComparer<ReportingAccountItem>
    {
        public bool Equals(ReportingAccountItem x, ReportingAccountItem y)
        {
            return x.AccountID == y.AccountID;
        }

        public int GetHashCode(ReportingAccountItem obj)
        {
            return obj.AccountID.GetHashCode();
        }
    }
}
