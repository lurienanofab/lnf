using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class LogPropertyValueMap : ClassMap<LogPropertyValue>
    {
        public LogPropertyValueMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.LogPropertyValueID);
            References(x => x.LogProperty);
            Map(x => x.Text);
            Map(x => x.Value);
        }
    }
}