using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ResourceLogPropertyMap : ClassMap<ResourceLogProperty>
    {
        public ResourceLogPropertyMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceLogPropertyID);
            References(x => x.LogProperty);
            References(x => x.Resource);
            Map(x => x.PropertyType);
        }
    }
}