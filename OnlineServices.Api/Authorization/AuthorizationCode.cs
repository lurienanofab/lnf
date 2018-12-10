using Newtonsoft.Json;

namespace OnlineServices.Api.Authorization
{
    public class AuthorizationCode
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
    }
}
