using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class DemEthnicMap : ClassMap<DemEthnic>
    {
        public DemEthnicMap()
        {
            Schema("sselData.dbo");
            Id(x => x.DemEthnicID);
            Map(x => x.DemEthnicValue, "DemEthnic");
        }
    }
}
