using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Services;
using Newtonsoft.Json;

namespace LNF.GoogleApi
{
    public class GoogleAuthorization
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        public DateTime Expires { get; set; }

        public bool IsExpired()
        {
            return DateTime.Now >= Expires;
        }
    }
}
