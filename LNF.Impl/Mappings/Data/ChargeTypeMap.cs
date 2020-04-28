using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ChargeTypeMap : ClassMap<ChargeType>
    {
        internal ChargeTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ChargeTypeID).GeneratedBy.Assigned();
            Map(x => x.ChargeTypeName, "ChargeType");
            Map(x => x.AccountID);
            Map(x => x.IsInternal);
        }
    }
}
