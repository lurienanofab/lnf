using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class MessengerMessageMap : ClassMap<MessengerMessage>
    {
        internal MessengerMessageMap()
        {
            Id(x => x.MessageID);
            Map(x => x.ParentID);
            Map(x => x.ClientID);
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
