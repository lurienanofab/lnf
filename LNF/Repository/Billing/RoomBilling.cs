using System;

namespace LNF.Repository.Billing
{
    public abstract class RoomBillingBase : IDataItem, IRoomBilling
    {
        public virtual int RoomBillingID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int RoomID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual int BillingTypeID { get; set; }
        public virtual int OrgID { get; set; }
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
        public abstract bool IsTemp { get; }

        /// <summary>
        /// The total charge used to calculate subsidy.
        /// </summary>
        public virtual decimal GetTotalCharge()
        {
            // this matches the stored procedure TieredSubsidyBilling_Select @Action = 'ForSubsidyDiscountDistribution'
            return RoomCharge + EntryCharge;
        }
    }

    public class RoomBilling : RoomBillingBase
    {
        public override bool IsTemp
        {
            get { return false; }
        }
    }

    public class RoomBillingTemp : RoomBillingBase
    {
        public override bool IsTemp
        {
            get { return true; }
        }
    }
}