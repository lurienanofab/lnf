using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientRemoteMap : ClassMap<ClientRemote>
    {
        internal ClientRemoteMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientRemoteID);
            References(x => x.Client);
            References(x => x.Account);
            References(x => x.RemoteClient);
        }
    }
}
