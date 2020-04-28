using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Meter;

namespace LNF.Impl.Mappings.Meter
{
    internal class FileImportMap : ClassMap<FileImport>
    {
        internal FileImportMap()
        {
            Schema("Meter.dbo");
            Id(x => x.FileImportID);
            Map(x => x.ImportFileName);
            Map(x => x.ImportDate);
            Map(x => x.ImportResult);
            Map(x => x.TotalRows);
        }
    }
}
