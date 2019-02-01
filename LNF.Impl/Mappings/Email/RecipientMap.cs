using FluentNHibernate.Mapping;
using LNF.Repository.Mail;

namespace LNF.Impl.Mappings.Email
{
    internal class RecipientMap : ClassMap<Recipient>
    {
        internal RecipientMap()
        {
            Schema("Email.dbo");
            Id(x => x.RecipientID);
            References(x => x.Message);
            Map(x => x.ClientID);
            Map(x => x.AddressType);
            Map(x => x.AddressText);
            Map(x => x.AddressTimestamp);
        }
    }
}
