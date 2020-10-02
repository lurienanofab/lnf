using RestSharp;
using RestSharp.Authenticators;
using System.Configuration;

namespace LNF.Impl
{
    public static class Rest
    {
        private static IRestClient GetClient()
        {
            var apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            var basicAuthUsername = ConfigurationManager.AppSettings["BasicAuthUsername"];
            var basicAuthPassword = ConfigurationManager.AppSettings["BasicAuthPassword"];

            var client = new RestClient(apiBaseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(basicAuthUsername, basicAuthPassword)
            };

            return client;
        }

        public static string Post(string url, object args)
        {
            var client = GetClient();
            var request = new RestRequest(url).AddJsonBody(args);
            var response = client.Post(request);
            var result = response.Content;
            return result;
        }
    }
}
