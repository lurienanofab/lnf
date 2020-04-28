using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientPeferenceMap : ClassMap<ClientPreference>
    {
        internal ClientPeferenceMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ClientPreferenceID);
            Map(x => x.ClientID);
            Map(x => x.Preferences);
            Map(x => x.ApplicationName);
        }
    }
}
