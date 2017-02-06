using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class DemGenderMap : ClassMap<DemGender>
    {
        public DemGenderMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemGenderID);
            Map(x => x.DemGenderValue, "DemGender");
        }
    }
}
