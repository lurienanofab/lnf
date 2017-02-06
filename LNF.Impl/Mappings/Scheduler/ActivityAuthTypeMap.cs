using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ActivityAuthTypeMap : ClassMap<ActivityAuthType>
    {
        public ActivityAuthTypeMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ActivityAuthTypeID);
            Map(x => x.AuthTypeName);
            Map(x => x.AuthTypeDescription);
        }
    }
}
