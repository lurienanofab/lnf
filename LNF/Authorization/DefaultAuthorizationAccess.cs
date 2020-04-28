using System;

namespace LNF.Authorization
{
    public class DefaultAuthorizationAccess : IAuthorizationAccess
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

        public DefaultAuthorizationAccess()
        {
            Created = DateTime.Now;
        }

        public virtual string AccessToken { get; set; }

        public virtual string TokenType { get; set; }

        public virtual int ExpiresIn { get; set; }

        public virtual string RefreshToken { get; set; }

        public virtual DateTime Created { get; }

        public virtual DateTime ExpirationDate => Created.AddSeconds(ExpiresIn);

        public virtual bool Expired => ExpirationDate < DateTime.Now;
    }
}
