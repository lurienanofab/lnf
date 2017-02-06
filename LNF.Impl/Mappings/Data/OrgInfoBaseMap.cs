using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal abstract class OrgInfoBaseMap<T> : ClassMap<T> where T : OrgInfoBase
    {
        internal OrgInfoBaseMap()
        {
            SetCache();
            Schema("sselData.dbo");
            ReadOnly();
            SetTable();
            MapOrgID();
            Map(x => x.OrgName);
            Map(x => x.DefClientAddressID);
            Map(x => x.DefBillAddressID);
            Map(x => x.DefShipAddressID);
            Map(x => x.NNINOrg);
            Map(x => x.PrimaryOrg);
            Map(x => x.OrgActive);
            Map(x => x.OrgTypeID);
            Map(x => x.OrgTypeName);
            Map(x => x.ChargeTypeID);
            Map(x => x.ChargeTypeName);
            Map(x => x.ChargeTypeAccountID);
        }

        protected abstract void SetTable();

        protected abstract void MapOrgID();

        protected abstract void SetCache();
    }
}
