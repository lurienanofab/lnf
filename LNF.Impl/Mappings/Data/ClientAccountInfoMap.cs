using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientAccountInfoMap : ClientAccountInfoBaseMap<ClientAccountInfo>
    {
        protected override void MapClientAccountID()
        {
            Id(x => x.ClientAccountID);
        }

        protected override void SetTable()
        {
            Table("v_ClientAccountInfo");
        }

        protected override void SetCache()
        {
            // don't use cache, it only causes pain
        }
    }
}
