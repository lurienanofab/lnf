using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class GlobalCostMap : ClassMap<GlobalCost>
    {
        public GlobalCostMap()
        {
            Schema("sselData.dbo");
            Id(x => x.GlobalID);
            Map(x => x.BusinessDay);
            References(x => x.LabAccount);
            References(x => x.LabCreditAccount);
            References(x => x.SubsidyCreditAccount);
            References(x => x.Admin);
            Map(x => x.AccessToOld);
            Map(x => x.EffDate);
        }
    }
}
