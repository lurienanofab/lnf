using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class DemRaceMap : ClassMap<DemRace>
    {
        public DemRaceMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemRaceID);
            Map(x => x.DemRaceValue, "DemRace");
        }
    }
}
