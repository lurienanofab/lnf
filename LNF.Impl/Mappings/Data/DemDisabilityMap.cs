using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class DemDisabilityMap : ClassMap<DemDisability>
    {
        public DemDisabilityMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemDisabilityID);
            Map(x => x.DemDisabilityValue, "DemDisability");
        }
    }
}
