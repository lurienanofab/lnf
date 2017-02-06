using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ProcessInfoLineParamMap : ClassMap<ProcessInfoLineParam>
    {
        public ProcessInfoLineParamMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ProcessInfoLineParamID);
            References(x => x.Resource).Not.Nullable();
            Map(x => x.ParameterName).Not.Nullable(); 
            Map(x => x.ParameterType).Not.Nullable();
        }
    }
}
