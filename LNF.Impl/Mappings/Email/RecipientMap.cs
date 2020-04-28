using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Mail;

namespace LNF.Impl.Mappings.Email
{
    internal class RecipientMap : ClassMap<Recipient>
    {
        internal RecipientMap()
        {
            Schema("Email.dbo");
            Id(x => x.RecipientID);
            Map(x => x.MessageID);
            Map(x => x.ClientID);
            Map(x => x.AddressType);
            Map(x => x.AddressText);
            Map(x => x.AddressTimestamp);
        }
    }
}
