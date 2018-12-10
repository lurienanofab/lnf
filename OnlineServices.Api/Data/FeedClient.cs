using LNF.Models.Data;

namespace OnlineServices.Api.Data
{
    public class FeedClient : ApiClient
    {
        public FeedClient() : base(GetApiBaseUrl()) { }

        public DataFeedModel<T> GetDataFeedResult<T>(string feed)
        {
            var result = Get<DataFeedModel<T>>("data/feed/{feed}/json", UrlSegments(new { feed }));
            return result;
        }
    }
}
