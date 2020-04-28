using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class OrgMap : ClassMap<Org>
    {
        internal OrgMap()
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
