using LNF.Models.Authorization.Credentials;

namespace LNF.Models.Authorization
{
    public interface IAuthorizationService
    {
        IAuthorizationAccess Authorize(ICredentials credentials);
    }
}
