using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Data
{
    public class ChargeTypeMap : ClassMap<ChargeType>
    {
        public ChargeTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ChargeTypeID).GeneratedBy.Assigned();
            Map(x => x.ChargeTypeName, "ChargeType");
            Map(x => x.AccountID);
        }
    }
}
