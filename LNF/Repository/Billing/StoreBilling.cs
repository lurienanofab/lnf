using System;

namespace LNF.Repository.Billing
{
    public class StoreBilling : IDataItem
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
}
