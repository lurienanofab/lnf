using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class OrgInfoMap : OrgInfoBaseMap<OrgInfo>
    {
        protected override void MapOrgID()
        {
            Id(x => x.OrgID);
        }

        protected override void SetTable()
        {
            Table("v_OrgInfo");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
