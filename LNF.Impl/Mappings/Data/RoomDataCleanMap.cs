using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
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
