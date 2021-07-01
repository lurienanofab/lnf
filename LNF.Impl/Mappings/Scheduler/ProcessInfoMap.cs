using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ProcessInfoMap : ClassMap<ProcessInfo>
    {
        internal ProcessInfoMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ProcessInfoID);
            Map(x => x.ResourceID).Not.Nullable();
            Map(x => x.ProcessInfoName).Not.Nullable();
            Map(x => x.ParamName).Not.Nullable();
            Map(x => x.ValueName).Not.Nullable();
            Map(x => x.Special).Not.Nullable();
            Map(x => x.AllowNone).Not.Nullable();
            Map(x => x.Order, "[Order]").Not.Nullable();
            Map(x => x.RequireValue).Not.Nullable();
            Map(x => x.RequireSelection).Not.Nullable();
            Map(x => x.MaxAllowed).Not.Nullable();
            Map(x => x.Deleted).Not.Nullable().Default("0");
        }
    }
}
