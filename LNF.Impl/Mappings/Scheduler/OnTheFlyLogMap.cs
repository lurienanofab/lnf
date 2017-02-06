using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class OnTheFlyLogMap : ClassMap<OnTheFlyLog>
    {
        public OnTheFlyLogMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.OnTheFlyLogID);
            Map(x => x.LogGuid);
            Map(x => x.LogTimeStamp);
            Map(x => x.ActionName);
            Map(x => x.ActionData);
            Map(x => x.ResourceID);
            Map(x => x.IPAddress);
        }
    }
}
