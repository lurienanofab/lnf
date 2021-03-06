using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class ToolBillingByOrg : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual decimal UsageFeeCharged { get; set; }
        public virtual decimal OverTimePenaltyFee { get; set; }
        public virtual decimal UncancelledPenaltyFee { get; set; }
        public virtual decimal ReservationFee { get; set; }
        public virtual decimal BookingFee { get; set; }
        public virtual decimal ForgivenFee { get; set; }
        public virtual decimal TransferredFee { get; set; }
        public virtual decimal SubsidyDiscount { get; set; }
        public virtual decimal TotalCharge { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is ToolBillingByOrg item)) return false;

            return item.Period == Period
                && item.ClientID == ClientID
                && item.OrgID == OrgID;
        }

        public override int GetHashCode()
        {
            return new { Period, ClientID, OrgID }.GetHashCode();
        }
    }
}
