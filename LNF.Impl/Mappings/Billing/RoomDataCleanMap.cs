using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomDataCleanMap : ClassMap<RoomDataClean>
    {
        internal RoomDataCleanMap()
        {
            Schema("sselData.dbo");
            Id(x => x.RoomDataID);
            Map(x => x.ClientID);
            Map(x => x.RoomID);
            Map(x => x.EntryDT);
            Map(x => x.ExitDT);
            Map(x => x.Duration);
        }
    }
}
