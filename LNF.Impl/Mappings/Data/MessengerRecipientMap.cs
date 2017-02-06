using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class MessengerRecipientMap : ClassMap<MessengerRecipient>
    {
        public MessengerRecipientMap()
        {
            Id(x => x.RecipientID);
            References(x => x.Message);
            References(x => x.Client);
            Map(x => x.Folder);
            Map(x => x.Received);
            Map(x => x.Acknowledged);
            Map(x => x.AccessCount);
        }
    }
}
