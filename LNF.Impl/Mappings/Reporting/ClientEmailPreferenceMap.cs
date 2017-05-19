using FluentNHibernate.Mapping;
using LNF.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class ClientEmailPreferenceMap : ClassMap<ClientEmailPreference>
    {
        internal ClientEmailPreferenceMap()
        {
            Schema("Reporting.dbo");
            Id(x => x.ClientEmailPreferenceID);
            Map(x => x.EmailPreferenceID);
            Map(x => x.ClientID);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }
    }
}
