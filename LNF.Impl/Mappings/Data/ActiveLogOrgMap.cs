using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ActiveLogOrgMap : OrgInfoBaseMap<ActiveLogOrg>
    {
        internal ActiveLogOrgMap()
        {
            Id(x => x.LogID);
            Map(x => x.Record);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }

        protected override void MapOrgID()
        {
            Map(x => x.OrgID);
        }

        protected override void SetTable()
        {
            Table("v_ActiveLogOrg");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
