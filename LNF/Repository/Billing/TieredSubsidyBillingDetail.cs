using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Billing
{
    public class TieredSubsidyBillingDetail : IDataItem
    {
        public virtual int TierBillingDetailID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual TieredSubsidyBilling TieredSubsidyBilling { get;set;}
        public virtual decimal FloorAmount { get;set;}
        public virtual decimal UserPaymentPercentage { get; set; }
        public virtual decimal OriginalApplyAmount { get; set; }
        public virtual decimal UserPayment { get; set; }
    }
}
