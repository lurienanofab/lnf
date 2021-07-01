using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ProcessInfoLineParamMap : ClassMap<ProcessInfoLineParam>
    {
        internal ProcessInfoLineParamMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ProcessInfoLineParamID);
            Map(x => x.ResourceID).Not.Nullable();
            Map(x => x.ParameterName).Not.Nullable();
            Map(x => x.ParameterUnit).Nullable();
            Map(x => x.ParameterType).Not.Nullable();
        }
    }
}
