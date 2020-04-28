using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class GoogleCalendarFeedMap : ClassMap<GoogleCalendarFeed>
    {
        internal GoogleCalendarFeedMap()
        {
            Schema("sselData.dbo");
            Id(x => x.GoogleCalendarFeedID);
            Map(x => x.GoogleCalendarID);
            References(x => x.Client);
            Map(x => x.Created);
            Map(x => x.LastUsed);
            Map(x => x.Active);
        }
    }
}
