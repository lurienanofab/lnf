using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class CostMap : ClassMap<Cost>
    {
        internal CostMap()
        {
            Schema("sselData.dbo");
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
