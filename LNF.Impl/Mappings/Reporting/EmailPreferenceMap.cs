using FluentNHibernate.Mapping;
using LNF.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class EmailPreferenceMap:ClassMap<EmailPreference>
    {
        public EmailPreferenceMap()
        {
            Schema("Reporting.dbo");
            Id(x => x.EmailPreferenceID);
            Map(x => x.ReportName);
            Map(x => x.Description);
        }
    }
}
