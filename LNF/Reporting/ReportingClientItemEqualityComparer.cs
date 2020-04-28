using System.Collections.Generic;

namespace LNF.Reporting
{
    public class ReportingClientItemEqualityComparer : IEqualityComparer<IReportingClient>
    {
        public bool Equals(IReportingClient x, IReportingClient y)
        {
            return x.ClientID == y.ClientID && x.Email == y.Email;
        }

        public int GetHashCode(IReportingClient obj)
        {
            return new { obj.ClientID, obj.Email }.GetHashCode();
        }
    }
}
