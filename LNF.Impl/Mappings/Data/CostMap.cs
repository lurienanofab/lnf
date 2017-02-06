using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class CostMap : ClassMap<Cost>
    {
        public CostMap()
        {
            Schema("sselData.dbo");
            Id(x => x.CostID);
            References(x => x.ChargeType);
            Map(x => x.TableNameOrDescription, "TableNameOrDescript");
            Map(x => x.RecordID);
            Map(x => x.AcctPer);
            Map(x => x.AddVal);
            Map(x => x.MulVal);
            Map(x => x.EffDate);
            Map(x => x.CreatedDate);
        }
    }
}
