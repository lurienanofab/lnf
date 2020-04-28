using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomBillingByOrgMap : ClassMap<RoomBillingByOrg>
    {
        internal RoomBillingByOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_RoomBillingByOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyProperty(x => x.ClientID)
                .KeyProperty(x => x.OrgID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.OrgName);
            Map(x => x.RoomCharge);
            Map(x => x.EntryCharge);
            Map(x => x.SubsidyDiscount);
            Map(x => x.TotalCharge);
        }
    }
}
