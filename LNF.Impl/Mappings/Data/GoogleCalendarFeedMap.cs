using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class GoogleCalendarFeedMap : ClassMap<GoogleCalendarFeed>
    {
        public GoogleCalendarFeedMap()
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
