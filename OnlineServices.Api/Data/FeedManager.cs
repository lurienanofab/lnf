using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class FeedManager : ApiClient, IFeedManager
    {
        public FeedManager() : base(GetApiBaseUrl()) { }

        public DataFeedResult GetDataFeedResult(string feed, string key = null, IDictionary<object, object> parameters = null)
        {
            var result = Get<DataFeedResult>("data/feed/{feed}/json/{key}", UrlSegments(new { feed, key }) & QueryStrings(Replace(parameters)));
            return result;
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
