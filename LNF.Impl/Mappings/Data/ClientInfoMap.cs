using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientInfoMap : ClientOrgInfoBaseMap<ClientInfo>
    {
        protected override void MapClientID()
        {
            Id(x => x.ClientID);
        }

        protected override void MapClientOrgID()
        {
            Map(x => x.ClientOrgID);
        }

        protected override void SetTable()
        {
            Table("v_ClientInfo");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
