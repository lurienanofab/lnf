using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF.Repository.Billing
{
    public class TieredSubsidyBilling : IDataItem
    {
        public virtual int TierBillingID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Org Org { get; set; }
        public virtual decimal RoomSum { get; set; }
        public virtual decimal RoomMiscSum { get; set; }
        public virtual decimal ToolSum { get; set; }
        public virtual decimal ToolMiscSum { get; set; }
        public virtual decimal UserTotalSum { get; set; }
        public virtual decimal UserPaymentSum { get; set; }
        public virtual DateTime StartingPeriod { get; set; }
        public virtual decimal Accumulated { get; set; }
        public virtual bool IsNewStudent { get; set; }
        public virtual bool IsNewFacultyUser { get; set; }
        public virtual IList<TieredSubsidyBillingDetail> Details { get; set; }

        public TieredSubsidyBilling()
        {
            Details = new List<TieredSubsidyBillingDetail>();
        }
    }
}
