using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class ApproverInfoMap : ClassMap<ApproverInfo>
    {
        internal ApproverInfoMap()
        {
            Schema("IOF.dbo");
            Table("v_ApproverInfo");
            ReadOnly();
            CompositeId()
                .KeyProperty(x => x.ApproverID)
                .KeyProperty(x => x.ClientID);
            Map(x => x.DisplayName);
            Map(x => x.Email);
            Map(x => x.ApproverDisplayName);
            Map(x => x.ApproverEmail);
            Map(x => x.IsPrimary);
            Map(x => x.Active);
        }
    }
}