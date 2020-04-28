using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DryBoxAssignmentLogMap : ClassMap<DryBoxAssignmentLog>
    {
        internal DryBoxAssignmentLogMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DryBoxAssignmentLogID);
            References(x => x.DryBoxAssignment);
            References(x => x.ClientAccount);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
            References(x => x.ModifiedBy, "ModifiedByClientID");
        }
    }
}
