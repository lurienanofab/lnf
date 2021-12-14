using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class FeedRepository : ApiClient, IFeedRepository
    {
        internal FeedRepository(IRestClient rc) : base(rc) { }

        public IScriptEngine ScriptEngine => throw new NotImplementedException();

        public IFeedsLog AddFeedsLogEntry(string requestIp, string requestUrl, string userAgent)
        {
            throw new NotImplementedException();
        }

        public IDataFeed GetDataFeed(int feedId)
        {
            throw new NotImplementedException();
        }

        public IDataFeed GetDataFeed(string alias)
        {
            throw new NotImplementedException();
        }

        public DataFeedResult GetDataFeedResult(string feed, string key = null, IDictionary<object, object> parameters = null)
        {
            var result = Get<DataFeedResult>("data/feed/{feed}/json/{key}", UrlSegments(new { feed, key }) & QueryStrings(Replace(parameters)));
            return result;
        }

        public IEnumerable<IReservationFeed> GetReservationFeeds(DateTime sd, DateTime ed, int resourceId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationFeed> GetReservationFeeds(string username, DateTime sd, DateTime ed, int resourceId = 0)
        {
            throw new NotImplementedException();
        }

        private IDictionary<object, object> Replace(IDictionary<object, object> dict)
        {
            var result = new Dictionary<object, object>();

            foreach(var kvp in dict)
            {
                result.Add(kvp.Key, DataFeedItem.GetParamValue(kvp.Value.ToString()));
            }

            return result;
        }
    }
}
