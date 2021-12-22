using FluentNHibernate.Mapping;
using LNF.Mail;

namespace LNF.Impl.Mappings.Email
{
    internal class MessageMap : ClassMap<Message>
    {
        internal MessageMap()
        {
            Schema("Email.dbo");
            Id(x => x.MessageID);
            Map(x => x.ClientID);
            Map(x => x.FromAddress);
            Map(x => x.Subject).Length(4001);
            Map(x => x.Body).Length(4001);
            Map(x => x.Error);
            Map(x => x.Caller);
            Map(x => x.CreatedOn);
            Map(x => x.SentOn);
        }
    }
}
