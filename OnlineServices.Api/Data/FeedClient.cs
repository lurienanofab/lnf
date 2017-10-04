using LNF.Models.Data;
using System.Configuration;
using System.Threading.Tasks;

namespace OnlineServices.Api.Data
{
    public class FeedClient : ApiClient
    {
        public FeedClient() : base(ConfigurationManager.AppSettings["FeedHost"]) { }

        public async Task<DataFeedModel<T>> GetDataFeedResult<T>(string feed)
        {
            var result = await Get<DataFeedModel<T>>(feed + "/json");
            return result;
        }
    }
}
