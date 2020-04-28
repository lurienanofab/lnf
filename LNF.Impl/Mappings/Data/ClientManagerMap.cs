using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientManagerMap : ClassMap<ClientManager>
    {
        internal ClientManagerMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientManagerID);
            References(x => x.ClientOrg);
            References(x => x.ManagerOrg);
            Map(x => x.Active);
        }
    }
}
