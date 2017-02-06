using LNF.Models.Data;
using System;
using System.Threading.Tasks;

namespace OnlineServices.Api.Data
{
    public class FeedClient : ApiClient
    {
        internal FeedClient(ApiClientOptions options)
            : base(new ApiClientOptions()
            {
                AccessToken = options.AccessToken,
                TokenType = options.TokenType,
                Host = new Uri("https://ssel-apps.eecs.umich.edu/data/feed/")
            })
        { }

        public async Task<DataFeedModel<T>> GetDataFeedResult<T>(string feed)
        {
            return await Get<DataFeedModel<T>>(feed + "/json");
        }
    }
}
