using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomDataWithToolUsageMap : ClassMap<RoomDataWithToolUsage>
    {
        internal RoomDataWithToolUsageMap()
        {
            Schema("Billing.dbo");
            Table("v_RoomDataWithToolUsage");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyProperty(x => x.ClientID)
                .KeyProperty(x => x.RoomID);
            Map(x => x.TotalEntriesPerMonth);
            Map(x => x.TotalHoursPerMonth);
            Map(x => x.PhysicalDays);
        }
    }
}
