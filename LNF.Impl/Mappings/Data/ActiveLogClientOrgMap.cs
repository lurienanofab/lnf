using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ActiveLogClientOrgMap : ClientOrgInfoBaseMap<ActiveLogClientOrg>
    {
        internal ActiveLogClientOrgMap()
        {
            Id(x => x.LogID);
            Map(x => x.Record);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }

        protected override void MapClientID()
        {
            Map(x => x.ClientID);
        }

        protected override void MapClientOrgID()
        {
            Map(x => x.ClientOrgID);
        }

        protected override void SetTable()
        {
            Table("v_ActiveLogClientOrg");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
