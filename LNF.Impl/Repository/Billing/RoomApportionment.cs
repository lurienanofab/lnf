using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class RoomApportionment : IDataItem
    {
        public virtual int AppID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Room Room { get; set; }
        public virtual Account Account { get; set; }
        public virtual ChargeType ChargeType { get; set; }
        public virtual BillingType BillingType { get; set; }
        public virtual Org Org { get; set; }
        public virtual decimal ChargeDays { get; set; }
        public virtual decimal PhysicalDays { get; set; }
        public virtual decimal AccountDays { get; set; }
        public virtual decimal Entries { get; set; }
        public virtual decimal Hours { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual decimal RoomRate { get; set; }
        public virtual decimal EntryRate { get; set; }
        public virtual decimal MonthlyRoomCharge { get; set; }
        public virtual decimal RoomCharge { get; set; }
        public virtual decimal EntryCharge { get; set; }
        public virtual decimal SubsidyDiscount { get; set; }

        public virtual decimal GetLineTotal()
        {
            return RoomCharge + EntryCharge;
        }
    }
}
