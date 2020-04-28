using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class ApproverMap : ClassMap<Approver>
    {
        internal ApproverMap()
        {
            Schema("IOF.dbo");
            CompositeId()
                .KeyProperty(x => x.ApproverID)
                .KeyProperty(x => x.ClientID);
            Map(x => x.IsPrimary);
            Map(x => x.Active);
        }
    }
}
