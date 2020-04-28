using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class RoleMap : ClassMap<Role>
    {
        internal RoleMap()
        {
            Schema("sselData.dbo");
            Id(x => x.RoleID);
            Map(x => x.RoleName, "Role");
        }
    }
}
