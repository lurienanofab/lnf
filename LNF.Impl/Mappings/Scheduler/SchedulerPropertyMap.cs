using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class SchedulerPropertyMap : ClassMap<SchedulerProperty>
    {
        internal SchedulerPropertyMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.PropertyID);
            Map(x => x.PropertyName);
            Map(x => x.PropertyValue);
        }
    }
}
