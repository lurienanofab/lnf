using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class LogPropertyMap : ClassMap<LogProperty>
    {
        public LogPropertyMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.LogPropertyID);
            Map(x => x.PropertyName);
        }
    }
}