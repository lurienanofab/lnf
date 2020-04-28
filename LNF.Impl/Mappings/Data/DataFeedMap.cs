using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class DataFeedMap : ClassMap<DataFeed>
    {
        internal DataFeedMap()
        {
            Schema("sselData.dbo");
            Id(x => x.FeedID);
            Map(x => x.FeedGUID);
            Map(x => x.FeedAlias);
            Map(x => x.FeedName);
            Map(x => x.FeedQuery).Length(int.MaxValue);
            Map(x => x.DefaultParameters);
            Map(x => x.Private);
            Map(x => x.Active);
            Map(x => x.Deleted);
            Map(x => x.FeedDescription);
            Map(x => x.FeedLink);
            Map(x => x.FeedType);
        }
    }
}
