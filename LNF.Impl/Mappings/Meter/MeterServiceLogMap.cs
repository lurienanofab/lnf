using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Meter;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Meter
{
    public class MeterServiceLogMap : ClassMap<MeterServiceLog>
    {
        public MeterServiceLogMap()
        {
            Schema("Meter.dbo");
            Table("ServiceLog");
            Id(x => x.ServiceLogID);
            Map(x => x.LogMessageType);
            Map(x => x.LogMessage);
            Map(x => x.EntryDateTime);
            Map(x => x.EntryData);
        }
    }
}
