using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Mail;

namespace LNF.Impl.Mappings.Email
{
    internal class MassEmailMap : ClassMap<MassEmail>
    {
        internal MassEmailMap()
        {
            Schema("Email.dbo");
            Id(x => x.MassEmailID);
            Map(x => x.ClientID);
            Map(x => x.EmailId).Unique();
            Map(x => x.EmailFolder);
            Map(x => x.CreatedOn).CustomSqlType("datetime2").CustomType("datetime2");
            Map(x => x.ModifiedOn).CustomSqlType("datetime2").CustomType("datetime2");
            Map(x => x.RecipientGroup);
            Map(x => x.RecipientCriteria);
            Map(x => x.FromAddress);
            Map(x => x.CCAddress);
            Map(x => x.DisplayName);
            Map(x => x.Subject);
            Map(x => x.Body);
        }
    }
}
