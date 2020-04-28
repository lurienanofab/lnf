using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomDataImportLogMap : ClassMap<RoomDataImportLog>
    {
        internal RoomDataImportLogMap()
        {
            Schema("Billing.dbo");
            Id(x => x.RoomDataImportLogID);
            Map(x => x.ImportDateTime);
            Map(x => x.RowsImported);
            Map(x => x.PriorMaxEventDate);
            Map(x => x.NewMaxEventDate);
        }
    }
}
