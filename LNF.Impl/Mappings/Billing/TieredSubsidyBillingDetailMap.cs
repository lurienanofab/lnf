using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class TieredSubsidyBillingDetailMap : ClassMap<TieredSubsidyBillingDetail>
    {
        internal TieredSubsidyBillingDetailMap()
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
