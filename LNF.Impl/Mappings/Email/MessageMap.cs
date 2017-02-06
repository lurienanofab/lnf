using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Email;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Email
{
    public class MessageMap : ClassMap<Message>
    {
        public MessageMap()
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
            HasMany(x => x.Recipients).KeyColumn("MessageID");
        }
    }
}
