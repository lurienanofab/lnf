using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Meter;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Meter
{
    public class FileImportMap : ClassMap<FileImport>
    {
        public FileImportMap()
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
