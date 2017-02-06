using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class TasksClient
    {
        public ApiClientOptions Options { get; }

        public TasksClient(ApiClientOptions options)
        {
            Options = options;
        }

        public async Task<T> RunJob<T>(Dictionary<string, string> args, T result)
        {
            using (var hc = new HttpClient())
            {
                hc.BaseAddress = Options.Host;
                hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Options.TokenType, Options.AccessToken);
                FormUrlEncodedContent content = new FormUrlEncodedContent(args);
                var msg = await hc.PostAsync("tasks/api/runjob", content);
                var json = await msg.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, result);
            }
        }
    }
}
