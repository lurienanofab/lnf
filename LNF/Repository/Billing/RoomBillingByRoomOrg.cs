using LNF.Repository.Data;
using System;

namespace LNF.Repository.Billing
{
    public class RoomBillingByRoomOrg : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Org Org { get; set; }
        public virtual Room Room { get; set; }
        public virtual ChargeType ChargeType { get; set; }
        public virtual BillingType BillingType { get; set; }
        public virtual decimal ChargeDays { get; set; }
        public virtual decimal Entries { get; set; }
        public virtual decimal Hours { get; set; }
        public virtual decimal RoomCharge { get; set; }
        public virtual decimal EntryCharge { get; set; }
        public virtual decimal SubsidyDiscount { get; set; }
        public virtual decimal TotalCharge { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var item = obj as RoomBillingByRoomOrg;

            if (item == null) return false;

            return item.Period == Period
                && item.Client.ClientID == Client.ClientID
                && item.Org.OrgID == Org.OrgID
                && item.Room.RoomID == Room.RoomID;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}|{3}", Period.GetHashCode(), Client.ClientID, Org.OrgID, Room.RoomID).GetHashCode();
        }
    }
}
