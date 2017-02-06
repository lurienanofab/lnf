using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class MessengerMessageMap : ClassMap<MessengerMessage>
    {
        public MessengerMessageMap()
        {
            Id(x => x.MessageID);
            Map(x => x.ParentID);
            References(x => x.Client);
            Map(x => x.Status);
            Map(x => x.Subject);
            Map(x => x.Body);
            Map(x => x.Created);
            Map(x => x.Sent);
            Map(x => x.DisableReply);
            Map(x => x.Exclusive);
            Map(x => x.AcknowledgeRequired);
            Map(x => x.BlockAccess);
            Map(x => x.AccessCutoff);
        }
    }
}
