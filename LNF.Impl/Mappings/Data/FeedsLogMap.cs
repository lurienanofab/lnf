using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class FeedsLogMap :ClassMap<FeedsLog>
    {
        public FeedsLogMap()
        {
            Schema("sselData.dbo");
            Id(x => x.FeedsLogID);
            Map(x => x.EntryDateTime);
            Map(x => x.RequestIP);
            Map(x => x.RequestURL);
            Map(x => x.RequestUserAgent);
        }
    }
}
