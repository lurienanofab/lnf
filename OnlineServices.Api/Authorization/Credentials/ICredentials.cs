using RestSharp;

namespace OnlineServices.Api.Authorization.Credentials
{
    public interface ICredentials
    {
        void ApplyParameters(IRestRequest req);
    }
}
