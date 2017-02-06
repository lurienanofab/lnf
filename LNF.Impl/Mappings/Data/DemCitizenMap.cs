using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class DemCitizenMap : ClassMap<DemCitizen>
    {
        public DemCitizenMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemCitizenID);
            Map(x => x.DemCitizenValue, "DemCitizen");
        }
    }
}
