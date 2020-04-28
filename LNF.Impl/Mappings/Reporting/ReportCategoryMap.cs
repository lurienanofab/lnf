using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class ReportCategoryMap : ClassMap<ReportCategory>
    {
        internal ReportCategoryMap()
        {
            Schema("Reporting.dbo");
            Table("Category");
            Id(x => x.CategoryID);
            Map(x => x.Slug);
            Map(x => x.Name);
            Map(x => x.Active);
        }
    }
}
