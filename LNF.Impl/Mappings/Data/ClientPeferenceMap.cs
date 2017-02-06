using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ClientPeferenceMap : ClassMap<ClientPreference>
    {
        public ClientPeferenceMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientPreferenceID);
            References(x => x.Client, "ClientID");
            Map(x => x.Preferences);
            Map(x => x.ApplicationName);
        }
    }
}
