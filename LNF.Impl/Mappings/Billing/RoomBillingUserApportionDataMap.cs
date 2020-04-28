using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomBillingUserApportionDataMap : ClassMap<RoomBillingUserApportionData>
    {
        internal RoomBillingUserApportionDataMap()
        {
            Id(x => x.RoomBillingUserApportionDataID, "AppID");
            Map(x => x.Period);
            References(x => x.Client);
            References(x => x.Room);
            References(x => x.Account);
            Map(x => x.ChargeDays);
            Map(x => x.Entries);
        }
    }
}
