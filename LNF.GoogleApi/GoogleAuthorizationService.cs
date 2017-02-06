using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace LNF.GoogleApi
{
    public class GoogleAuthorizationService
    {
        private const string BASE_ADDRESS = "https://www.googleapis.com";
        private const string REQUEST_PATH = "/oauth2/v3/token";
        private const string AUTH_GRANT_TYPE = "authorization_code";
        private const string REFRESH_GRANT_TYPE = "refresh_token";

        public GoogleAuthorizationService(GoogleAuthorizationOptions options)
        {
            Options = options;
        }

        public GoogleAuthorizationOptions Options { get; private set; }

        public async Task<GoogleAuthorization> Refresh(string refreshToken)
        {
            return await Post(new
            {
                refresh_token = refreshToken,
                client_id = Options.ClientID,
                client_secret = Options.ClientSecret,
                grant_type = REFRESH_GRANT_TYPE
            });
        }

        public async Task<GoogleAuthorization> Authorize(string code)
        {
            return await Post(new
            {
                code,
                client_id = Options.ClientID,
                client_secret = Options.ClientSecret,
                redirect_uri = Options.RedirectUri,
                grant_type = AUTH_GRANT_TYPE
            });
        }

        private async Task<GoogleAuthorization> Post(object postData)
        {
            var dict = postData.GetType().GetProperties().ToDictionary(k => k.Name, v => Convert.ToString(v.GetValue(postData, null)));

            using (var hc = new HttpClient())
            {
                hc.BaseAddress = new Uri(BASE_ADDRESS);

                var body = new FormUrlEncodedContent(dict);

                var msg = await hc.PostAsync(REQUEST_PATH, body);

                var content = await msg.Content.ReadAsStringAsync();

                if (!msg.IsSuccessStatusCode)
                    throw new System.Web.HttpException((int)msg.StatusCode, content);

                var result = JsonConvert.DeserializeObject<GoogleAuthorization>(content);

                result.Expires = DateTime.Now.AddSeconds(result.ExpiresIn);

                return result;
            }
        }
    }
}
