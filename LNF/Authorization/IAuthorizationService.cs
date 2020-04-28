using LNF.Authorization.Credentials;

namespace LNF.Authorization
{
    public interface IAuthorizationService
    {
        IAuthorizationAccess Authorize(ICredentials credentials);
    }
}
