using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class ApiClient : IDisposable
    {
        protected HttpClient HttpClient;

        internal ApiClient(ApiClientOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options", "Null ApiClientOptions passed to constructor in OnlineServices.Api.ApiClient.");

            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromMinutes(10);
            HttpClient.BaseAddress = options.Host;
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(options.TokenType, options.AccessToken);
        }

        public async Task<string> Get(string path)
        {
            string result = await HttpClient.GetStringAsync(path);
            return result;
        }

        public async Task<string> Post(string path, Dictionary<string, string> data)
        {
            HttpContent content;

            if (data != null && data.Count > 0)
                content = new FormUrlEncodedContent(data);
            else
                content = new StringContent(string.Empty);

            var msg = await HttpClient.PostAsync(path, content);
            var result = await msg.Content.ReadAsStringAsync();
            return result;
        }

        protected async Task<T> Get<T>(string path)
        {
            var msg = await HttpClient.GetAsync(path);

            await CheckForError(msg);

            var result = await msg.Content.ReadAsAsync<T>();
            return result;
        }

        protected async Task<T> Post<T>(string path, object model)
        {
            var msg = await HttpClient.PostAsJsonAsync(path, model);

            await CheckForError(msg);

            var result = await msg.Content.ReadAsAsync<T>();
            return result;
        }

        protected async Task<T> Post<T>(string path, HttpContent content)
        {
            var msg = await HttpClient.PostAsync(path, content);

            await CheckForError(msg);

            var result = await msg.Content.ReadAsAsync<T>();
            return result;
        }

        protected async Task<bool> Put<T>(string path, object model)
        {
            var msg = await HttpClient.PutAsJsonAsync(path, model);

            await CheckForError(msg);

            var result = await msg.Content.ReadAsAsync<bool>();
            return result;
        }

        protected async Task<T> Put<T>(string path, HttpContent content)
        {
            var msg = await HttpClient.PutAsync(path, content);

            await CheckForError(msg);

            var result = await msg.Content.ReadAsAsync<T>();
            return result;
        }

        protected async Task<bool> Delete(string path)
        {
            var msg = await HttpClient.DeleteAsync(path);

            await CheckForError(msg);

            var result = await msg.Content.ReadAsAsync<bool>();
            return result;
        }

        protected async Task CheckForError(HttpResponseMessage msg)
        {
            if (!msg.IsSuccessStatusCode)
            {
                throw await ApiHttpRequestException.Create(msg);
            }
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}
