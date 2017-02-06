using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class DepartmentMap : ClassMap<Department>
    {
        public DepartmentMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DepartmentID);
            Map(x => x.DepartmentName, "Department");
            References(x => x.Org, "OrgID");
        }
    }
}
