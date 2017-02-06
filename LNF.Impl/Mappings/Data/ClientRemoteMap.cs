using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ClientRemoteMap : ClassMap<ClientRemote>
    {
        public ClientRemoteMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientRemoteID);
            References(x => x.Client);
            References(x => x.Account);
            References(x => x.RemoteClient);
        }
    }
}
