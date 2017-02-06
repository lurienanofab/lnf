using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class GlobalActivityAuthMap : ClassMap<GlobalActivityAuth>
    {
        public GlobalActivityAuthMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.GlobalActivityAuthID);
            References(x => x.Activity);
            References(x => x.ActivityAuthType);
            Map(x => x.DefaultAuth);
            Map(x => x.LockedAuth);
        }
    }
}
