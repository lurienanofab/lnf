using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class TieredSubsidyBillingDetailMap : ClassMap<TieredSubsidyBillingDetail>
    {
        public TieredSubsidyBillingDetailMap()
        {
            Id(x => x.TierBillingDetailID);
            Map(x => x.Period);
            References(x => x.TieredSubsidyBilling).Column("TierBillingID");
            Map(x => x.FloorAmount);
            Map(x => x.UserPaymentPercentage);
            Map(x => x.OriginalApplyAmount);
            Map(x => x.UserPayment);
        }
    }
}
