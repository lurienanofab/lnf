using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class EmailPreferenceMap:ClassMap<EmailPreference>
    {
        internal EmailPreferenceMap()
        {
            Schema("Reporting.dbo");
            Id(x => x.EmailPreferenceID);
            Map(x => x.ReportName);
            Map(x => x.Description);
        }
    }
}
