using Newtonsoft.Json;
using OnlineServices.Api.Authorization;
using OnlineServices.Api.Authorization.Credentials;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class AuthorizationClient : IDisposable
    {
        private HttpClient _httpClient;

        public Uri Host { get; }

        public AuthorizationClient()
        {
            Host = new Uri(ConfigurationManager.AppSettings["ApiHost"]);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = Host;
        }

        public async Task<AuthorizationAccess> Authorize(ICredentials credentials)
        {
            HttpResponseMessage msg = await _httpClient.PostAsync("auth/token", credentials.CreateContent());

            if (!msg.IsSuccessStatusCode)
                throw await ApiHttpRequestException.Create(msg);

            var result = await msg.Content.ReadAsAsync<AuthorizationAccess>();
            return result;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
