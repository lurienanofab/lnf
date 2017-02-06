using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class OrgMap : ClassMap<Org>
    {
        public OrgMap()
        {
            Schema("sselData.dbo");
            Id(x => x.OrgID);
            Map(x => x.OrgName);
            References(x => x.OrgType, "OrgTypeID");
            Map(x => x.DefClientAddressID);
            Map(x => x.DefBillAddressID);
            Map(x => x.DefShipAddressID);
            Map(x => x.NNINOrg);
            Map(x => x.PrimaryOrg);
            Map(x => x.Active);
        }
    }
}
