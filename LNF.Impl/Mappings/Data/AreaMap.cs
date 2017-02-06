using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class AreaMap : ClassMap<Area>
    {
        public AreaMap()
        {
            Schema("sselData.dbo");
            Table("v_Area");
            ReadOnly();
            Id(x => x.AreaID);
            Map(x => x.AreaName);
        }
    }
}
