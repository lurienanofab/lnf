using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class StoreBillingByOrg : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Org Org { get; set; }
        public virtual decimal TotalCharge { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is StoreBillingByOrg item)) return false;

            return item.Period == Period
                && item.Client.ClientID == Client.ClientID
                && item.Org.OrgID == Org.OrgID;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}", Period.GetHashCode(), Client.ClientID, Org.OrgID).GetHashCode();
        }
    }
}
