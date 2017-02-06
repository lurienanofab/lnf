using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ClientManagerMap : ClassMap<ClientManager>
    {
        public ClientManagerMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientManagerID);
            References(x => x.ClientOrg);
            References(x => x.ManagerOrg);
            Map(x => x.Active);
        }
    }
}
