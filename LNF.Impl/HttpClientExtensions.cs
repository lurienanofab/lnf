using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Impl
{
    // This class was added so that the System.Net.Http.Formatting.Extension nuget package is no longer needed.

    public static class HttpClientExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            T result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T model)
        {
            string json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(requestUri, content);
            return result;
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string requestUri, T model)
        {
            string json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PutAsync(requestUri, content);
            return result;
        }
    }
}
