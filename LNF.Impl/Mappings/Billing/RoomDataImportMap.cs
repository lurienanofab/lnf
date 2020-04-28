using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomDataImportMap:ClassMap<RoomDataImport>
    {
        internal RoomDataImportMap()
        {
            Schema("Billing.dbo");
            Id(x => x.RoomDataImportID);
            Map(x => x.RID).Unique();
            Map(x => x.ClientID);
            Map(x => x.RoomName);
            Map(x => x.EventDate);
            Map(x => x.EventDescription);
        }
    }
}
