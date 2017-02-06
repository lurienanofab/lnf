using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Schema("sselData.dbo");
            Id(x => x.RoleID);
            Map(x => x.RoleName, "Role");
        }
    }
}
