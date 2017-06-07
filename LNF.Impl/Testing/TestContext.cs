using LNF.Data;
using LNF.Models.Data;
using LNF.Repository.Data;
using System.Security.Principal;
using System.Web.Security;

namespace LNF.Impl.Testing
{
    public class TestContext : DefaultContext
    {
        public TestContext(string username, string[] roles)
        {
            var authCookie = FormsAuthentication.GetAuthCookie(username, true);
            FormsAuthenticationTicket formsAuthTicket = FormsAuthentication.Decrypt(authCookie.Value);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(formsAuthTicket.Version, formsAuthTicket.Name, formsAuthTicket.IssueDate, formsAuthTicket.Expiration, formsAuthTicket.IsPersistent, string.Join("|", roles), formsAuthTicket.CookiePath);
            authCookie.Value = FormsAuthentication.Encrypt(ticket);
            authCookie.Expires = formsAuthTicket.Expiration;

            SetRequestCookie(authCookie.Name, authCookie.Value, authCookie.Expires, authCookie.Domain, authCookie.Path, authCookie.HttpOnly, authCookie.Secure, authCookie.Shareable);
            SetResponseCookie(authCookie.Name, authCookie.Value, authCookie.Expires, authCookie.Domain, authCookie.Path, authCookie.HttpOnly, authCookie.Secure, authCookie.Shareable);

            User = new GenericPrincipal(new GenericIdentity(username), roles);
        }

        public override string GetAbsolutePath(string virtualPath)
        {
            string result = virtualPath.Replace("~", GetAppSetting("CurrentAppRoot"));
            return result;
        }
    }
}
