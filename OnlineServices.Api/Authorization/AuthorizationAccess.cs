using LNF.Models.Authorization;
using Newtonsoft.Json;
using System;

namespace OnlineServices.Api.Authorization
{
    public class AuthorizationAccess : DefaultAuthorizationAccess
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

        [JsonProperty(PropertyName = "access_token")]
        public override string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public override string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public override int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public override string RefreshToken { get; set; }

        [JsonIgnore]
        public override DateTime Created => base.Created;

        [JsonIgnore]
        public override DateTime ExpirationDate => base.ExpirationDate;

        [JsonIgnore]
        public override bool Expired => base.Expired;
    }
}
