using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class TieredSubsidyBillingMap : ClassMap<TieredSubsidyBilling>
    {
        internal TieredSubsidyBillingMap()
        {
            Schema("sselData.dbo");
            Id(x => x.TierBillingID);
            Map(x => x.Period).Not.Nullable();
            References(x => x.Client).Not.Nullable();
            References(x => x.Org).Not.Nullable();
            Map(x => x.RoomSum).Not.Nullable();
            Map(x => x.RoomMiscSum).Not.Nullable();
            Map(x => x.ToolSum).Not.Nullable();
            Map(x => x.ToolMiscSum).Not.Nullable();
            Map(x => x.UserTotalSum).Generated.Always();
            Map(x => x.UserPaymentSum).Not.Nullable();
            Map(x => x.StartingPeriod).Not.Nullable();
            Map(x => x.Accumulated).Not.Nullable();
            Map(x => x.IsNewStudent).Not.Nullable();
            Map(x => x.IsNewFacultyUser).Not.Nullable();
            HasMany(x => x.Details).KeyColumn("TierBillingDetailID");
        }
    }
}
