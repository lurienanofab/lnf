using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Meter;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Meter
{
    public class MeterReportMap : ClassMap<MeterReport>
    {
        public MeterReportMap()
        {
            Schema("Meter.dbo");
            Table("Report");
            Id(x => x.ReportID);
            Map(x => x.ReportName);
            Map(x => x.Header);
            Map(x => x.UnitCost);
            Map(x => x.Active);
            Map(x => x.Deleted);
        }
    }
}
