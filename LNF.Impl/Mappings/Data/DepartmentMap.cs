using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DepartmentMap : ClassMap<Department>
    {
        internal DepartmentMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DepartmentID);
            Map(x => x.DepartmentName, "Department");
            References(x => x.Org, "OrgID");
        }
    }
}
