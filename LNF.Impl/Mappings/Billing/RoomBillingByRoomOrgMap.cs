using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomBillingByRoomOrgMap : ClassMap<RoomBillingByRoomOrg>
    {
        internal RoomBillingByRoomOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_RoomBillingByRoomOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyProperty(x => x.ClientID)
                .KeyProperty(x => x.OrgID)
                .KeyProperty(x => x.RoomID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.OrgName);
            Map(x => x.RoomName);
            Map(x => x.RoomDisplayName);
            Map(x => x.ChargeTypeID);
            Map(x => x.ChargeTypeName);
            Map(x => x.BillingTypeID);
            Map(x => x.BillingTypeName);
            Map(x => x.ChargeDays);
            Map(x => x.Entries);
            Map(x => x.Hours);
            Map(x => x.RoomCharge);
            Map(x => x.EntryCharge);
            Map(x => x.SubsidyDiscount);
            Map(x => x.TotalCharge);
        }
    }
}
