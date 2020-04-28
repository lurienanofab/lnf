using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceLogPropertyMap : ClassMap<ResourceLogProperty>
    {
        internal ResourceLogPropertyMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceLogPropertyID);
            References(x => x.LogProperty);
            References(x => x.Resource);
            Map(x => x.PropertyType);
        }
    }
}