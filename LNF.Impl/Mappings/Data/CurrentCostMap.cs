using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Impl.Mappings.Data
{
    internal class CurrentCostMap : ClassMap<CurrentCost>
    {
        internal CurrentCostMap()
        {
            Schema("sselData.dbo");
            Table("v_CurrentCost");
            ReadOnly();
            Id(x => x.CostID);
            Map(x => x.ChargeTypeID);
            Map(x => x.TableNameOrDescription, "TableNameOrDescript");
            Map(x => x.RecordID).Default("0");
            Map(x => x.AcctPer);
            Map(x => x.AddVal);
            Map(x => x.MulVal);
            Map(x => x.EffDate);
            Map(x => x.CreatedDate);
        }
    }
}
