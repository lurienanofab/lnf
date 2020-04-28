using LNF.Billing;
using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class MiscBillingCharge : IMiscBillingCharge, IDataItem
    {
        public virtual int ExpID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string SUBType { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual DateTime ActDate { get; set; }
        public virtual string Description { get; set; }
        public virtual double Quantity { get; set; }
        public virtual decimal UnitCost { get; set; }
        public virtual decimal SubsidyDiscount { get; set; }
        public virtual bool Active { get; set; }

        public virtual decimal TotalCost
        {
            get
            {
                // this matches the stored procedure TieredSubsidyBilling_Select @Action = 'ForSubsidyDiscountDistribution'
                return Convert.ToDecimal(Quantity) * UnitCost;
            }
        }

        public virtual decimal UserPayment => TotalCost - SubsidyDiscount;

        /// <summary>
        /// The final billed amount based on BillingType and Room
        /// </summary>
        public virtual decimal GetLineCost() => TotalCost; //[2015-11-13 jg] same as TotalCharge - added to be consistent with ToolBilling and RoomBilling
    }
}
