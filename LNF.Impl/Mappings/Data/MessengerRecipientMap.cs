using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class MessengerRecipientMap : ClassMap<MessengerRecipient>
    {
        internal MessengerRecipientMap()
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
