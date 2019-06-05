using LNF.Models.Authorization;
using LNF.Models.Authorization.Credentials;
using LNF.Repository;
using System;

namespace LNF.Impl.Authorization
{
    public class AuthorizationService : ManagerBase, IAuthorizationService
    {
        public AuthorizationService(IProvider provider) : base(provider) { }

        public IAuthorizationAccess Authorize(ICredentials credentials)
        {
            throw new NotImplementedException();
        }
    }
}
