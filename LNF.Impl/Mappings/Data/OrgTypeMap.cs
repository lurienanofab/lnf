using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class OrgTypeMap : ClassMap<OrgType>
    {
        internal OrgTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.OrgTypeID);
            Map(x => x.OrgTypeName, "OrgType");
            References(x => x.ChargeType, "ChargeTypeID");
        }
    }
}
