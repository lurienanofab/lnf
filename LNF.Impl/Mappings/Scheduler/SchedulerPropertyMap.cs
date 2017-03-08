﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class SchedulerPropertyMap : ClassMap<SchedulerProperty>
    {
        public SchedulerPropertyMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.PropertyID);
            Map(x => x.PropertyName);
            Map(x => x.PropertyValue);
        }
    }
}