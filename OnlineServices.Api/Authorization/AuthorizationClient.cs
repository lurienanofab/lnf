using OnlineServices.Api.Authorization;
using OnlineServices.Api.Authorization.Credentials;
using RestSharp;

namespace OnlineServices.Api.Authorization
{
    public class AuthorizationClient : ApiClient
    {
        public AuthorizationClient() : base(GetApiBaseUrl()) { }

        public AuthorizationAccess Authorize(ICredentials credentials)
        {
            var req = CreateRestRequest("webapi/auth/token", Method.POST);

            credentials.ApplyParameters(req);

            var resp = HttpClient.Execute<AuthorizationAccess>(req);

            return Result(resp);
        }
    }
}
