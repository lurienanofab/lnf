using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Feedback;

namespace LNF.Impl.Mappings.Feedback
{
    internal class EmailTemplateMap : ClassMap<EmailTemplate>
    {
        internal EmailTemplateMap()
        {
            Schema("Feedback.dbo");
            Table("EmailTemplate");
            Id(x => x.EmailTemplateID);
            Map(x => x.TemplateName);
            Map(x => x.TemplateValue);
            Map(x => x.LastModified);
            Map(x => x.ClientID);
        }
    }
}
