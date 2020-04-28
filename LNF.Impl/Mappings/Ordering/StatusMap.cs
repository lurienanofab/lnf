using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class StatusMap : ClassMap<Status>
    {
        internal StatusMap()
        {
            Schema("IOF.dbo");
            Id(x => x.StatusID);
            Map(x => x.StatusName, "Status");
        }
    }
}
