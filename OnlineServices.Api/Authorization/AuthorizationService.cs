using LNF.Models.Authorization;
using LNF.Models.Authorization.Credentials;
using RestSharp;

namespace OnlineServices.Api.Authorization
{
    public class AuthorizationService : ApiClient, IAuthorizationService
    {
        public AuthorizationService() : base(GetApiBaseUrl()) { }

        public IAuthorizationAccess Authorize(ICredentials credentials)
        {
            var req = CreateRestRequest("webapi/auth/token", Method.POST);

            credentials.ApplyParameters(new RestSharpAuthRequest(req));

            var resp = HttpClient.Execute<AuthorizationAccess>(req);

            return Result(resp);
        }
    }

    public class RestSharpAuthRequest : IRequest
    {
        private readonly IRestRequest _restReq;

        public RestSharpAuthRequest(IRestRequest restReq)
        {
            _restReq = restReq;
        }

        public void AddParameter(string name, object value) => _restReq.AddParameter(name, value);
    }
}
