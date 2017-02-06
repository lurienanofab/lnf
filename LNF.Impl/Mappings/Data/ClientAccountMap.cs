using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ClientAccountMap : ClassMap<ClientAccount>
    {
        public ClientAccountMap()
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
