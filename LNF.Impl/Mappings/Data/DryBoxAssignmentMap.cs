using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DryBoxAssignmentMap : ClassMap<DryBoxAssignment>
    {
        internal DryBoxAssignmentMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DryBoxAssignmentID);
            References(x => x.DryBox);
            References(x => x.ClientAccount);
            Map(x => x.ReservedDate);
            Map(x => x.ApprovedDate);
            Map(x => x.RemovedDate);
            Map(x => x.PendingApproval);
            Map(x => x.PendingRemoval);
            Map(x => x.Rejected);
        }
    }
}
