using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ProcessInfoLineMap : ClassMap<ProcessInfoLine>
    {
        internal ProcessInfoLineMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ProcessInfoLineID);
            Map(x => x.ProcessInfoID).Not.Nullable();
            Map(x => x.Param).Not.Nullable();
            Map(x => x.MinValue).Not.Nullable();
            Map(x => x.MaxValue).Not.Nullable();
            Map(x => x.ProcessInfoLineParamID).Not.Nullable();
            Map(x => x.Deleted).Not.Nullable().Default("0");
        }
    }
}
