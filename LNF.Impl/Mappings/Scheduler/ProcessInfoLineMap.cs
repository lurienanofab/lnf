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
            Map(x => x.Param); //<-- field will be deleted from the Database
            Map(x => x.MinValue);
            Map(x => x.MaxValue);
            References(x => x.ProcessInfoLineParam);//.NotFound.Ignore();
            //Map(x => x.ProcessInfoUnitID);
        }
    }
}
