using Newtonsoft.Json;
using System;

namespace OnlineServices.Api.Authorization
{
    public class AuthorizationAccess
    {
        /*
        Response from auth server looks like this:
        {
            "access_token": "xxxxxxxxxxxxxxxx",
            "token_type": "bearer",
            "expires_in": 299,
            "refresh_token": "xxxxxx"
        } 
        */

        public AuthorizationAccess()
        {
            Created = DateTime.Now;
        }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime Created { get; }

        [JsonIgnore]
        public DateTime ExpirationDate
        {
            get
            {
                return Created.AddSeconds(ExpiresIn);
            }
        }

        [JsonIgnore]
        public bool Expired
        {
            get
            {
                return ExpirationDate < DateTime.Now;
            }
        }
    }
}
