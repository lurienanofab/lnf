using System;
using System.Net.Http.Headers;

namespace OnlineServices.Api
{
    public class ApiClientOptions
    {
        public Uri Host { get; set; }
        public string TokenType { get; set; }
        public string AccessToken { get; set; }

        public AuthenticationHeaderValue GetHeaderValue()
        {
            return new AuthenticationHeaderValue(TokenType, AccessToken);
        }
    }
}
