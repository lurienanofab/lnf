using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class StoreBilling : StoreBillingBase
    {
        public override bool IsTemp => false;
    }

    public class StoreBillingTemp : StoreBillingBase
    {
        public override bool IsTemp => true;
    }

    public abstract class StoreBillingBase : IStoreBilling
    {
        public virtual int StoreBillingID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual int ItemID { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual decimal UnitCost { get; set; }
        public virtual decimal CostMultiplier { get; set; }
        public virtual decimal LineCost { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
        public abstract bool IsTemp { get; }

        public virtual decimal GetTotalCharge()
        {
            // Using the same formula as LineCost computed column.
            // Each billing table has the same TotalCharge method for consistency.

            return UnitCost * Quantity * CostMultiplier;
        }

        /// <summary>
        /// The final billed amount based on BillingType and Room
        /// </summary>
        public virtual decimal GetLineCost()
        {
            // [2015-11-13 jg] same as TotalCharge - added to be consistent with ToolBilling and RoomBilling
            return GetTotalCharge();
        }
    }

    public interface IStoreBilling : IDataItem
    {
        int StoreBillingID { get; set; }
        DateTime Period { get; set; }
        int ClientID { get; set; }
        int AccountID { get; set; }
        int ChargeTypeID { get; set; }
        int ItemID { get; set; }
        int CategoryID { get; set; }
        decimal Quantity { get; set; }
        decimal UnitCost { get; set; }
        decimal CostMultiplier { get; set; }
        decimal LineCost { get; set; }
        DateTime StatusChangeDate { get; set; }
        bool IsTemp { get; }
    }

}
