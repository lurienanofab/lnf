using LNF.Authorization;
using LNF.Authorization.Credentials;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using System;

namespace LNF.Impl.Authorization
{
    public class AuthorizationService : RepositoryBase, IAuthorizationService
    {
        public AuthorizationService(ISessionManager mgr) : base(mgr) { }

        public IAuthorizationAccess Authorize(ICredentials credentials)
        {
            throw new NotImplementedException();
        }
    }
}
