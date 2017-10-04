using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class ApiClient : IDisposable
    {
        protected HttpClient HttpClient;

        public ApiClient(string host)
        {
            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromMinutes(10);

            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            string username = ConfigurationManager.AppSettings["BasicAuthUsername"];
            string password = ConfigurationManager.AppSettings["BasicAuthPassword"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password));
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            HttpClient.BaseAddress = new Uri(host);
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
