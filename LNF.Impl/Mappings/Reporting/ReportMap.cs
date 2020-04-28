using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class ReportMap : ClassMap<Report>
    {
        internal ReportMap()
        {
            Schema("Reporting.dbo");
            Table("Report");
            Id(x => x.ReportID);
            References(x => x.Category).Not.Nullable();
            Map(x => x.Slug).Not.Nullable();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.FeedAlias).Not.Nullable();
            Map(x => x.Active).Not.Nullable();
        }
    }
}
