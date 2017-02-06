using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Meter;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Meter
{
    public class MeterDataMap : ClassMap<MeterData>
    {
        public MeterDataMap()
        {
            Schema("Meter.dbo");
            Id(x => x.MeterDataID);
            References(x => x.FileImport);
            Map(x => x.ImportFileName);
            Map(x => x.LineIndex);
            Map(x => x.TimeStamp);
            Map(x => x.Header);
            Map(x => x.Value);
        }
    }
}
