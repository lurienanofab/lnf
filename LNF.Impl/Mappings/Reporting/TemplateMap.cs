using FluentNHibernate.Mapping;
using LNF.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class TemplateMap : ClassMap<Template>
    {
        internal TemplateMap()
        {
            Schema("Reporting.dbo");
            Table("Template");
            Id(x => x.TemplateID);
            Map(x => x.TemplateName);
            Map(x => x.TemplateContent).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.Report);
        }
    }
}
