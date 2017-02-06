using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class FundingSourceMap : ClassMap<FundingSource>
    {
        public FundingSourceMap()
        {
            Schema("sselData.dbo");
            Id(x => x.FundingSourceID);
            Map(x => x.FundingSourceName, "FundingSource");
        }
    }
}
