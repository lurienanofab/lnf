using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientAccountMap : ClassMap<ClientAccount>
    {
        internal ClientAccountMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientAccountID);
            References(x => x.ClientOrg);
            References(x => x.Account);
            Map(x => x.Manager);
            Map(x => x.Active);
            Map(x => x.IsDefault);
        }
    }
}
