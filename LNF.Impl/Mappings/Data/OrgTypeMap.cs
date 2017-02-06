using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class OrgTypeMap : ClassMap<OrgType>
    {
        public OrgTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.OrgTypeID);
            Map(x => x.OrgTypeName, "OrgType");
            References(x => x.ChargeType, "ChargeTypeID");
        }
    }
}
