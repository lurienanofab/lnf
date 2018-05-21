using System.Collections.Generic;

namespace LNF.Models.Reporting
{
    public class ReportingClientItemEqualityComparer : IEqualityComparer<ReportingClientItem>
    {
        public bool Equals(ReportingClientItem x, ReportingClientItem y)
        {
            return x.ClientID == y.ClientID && x.Email == y.Email;
        }

        public int GetHashCode(ReportingClientItem obj)
        {
            return new { obj.ClientID, obj.Email }.GetHashCode();
        }
    }
}
