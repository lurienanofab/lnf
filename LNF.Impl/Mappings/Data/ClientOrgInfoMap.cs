using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientOrgInfoMap : ClientOrgInfoBaseMap<ClientOrgInfo>
    {
        protected override void MapClientID()
        {
            Map(x => x.ClientID);
        }

        protected override void MapClientOrgID()
        {
            Id(x => x.ClientOrgID);
        }

        protected override void SetTable()
        {
            Table("v_ClientOrgInfo");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
