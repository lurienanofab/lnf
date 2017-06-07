using System;

namespace LNF.Repository.Billing
{
    public class RoomBillingByRoomOrg : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual int RoomID { get; set; }
        public virtual string RoomName { get; set; }
        public virtual string RoomDisplayName { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual string ChargeTypeName { get; set; }
        public virtual int BillingTypeID { get; set; }
        public virtual string BillingTypeName { get; set; }
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
                && item.ClientID == ClientID
                && item.OrgID == OrgID
                && item.RoomID == RoomID;
        }

        public override int GetHashCode()
        {
            return new { Period, ClientID, OrgID, RoomID }.GetHashCode();
        }
    }
}
