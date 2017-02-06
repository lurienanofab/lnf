using LNF.Data;
using LNF.Models.Data;
using LNF.Repository.Data;
using System.Security.Principal;
using System.Web.Security;

namespace LNF.Impl.Testing
{
    public class TestContext : DefaultContext
    {
        private Client _currentClient;

        public TestContext()
        {
            Client currentClient = GetCurrentClient();
            var authCookie = FormsAuthentication.GetAuthCookie(currentClient.UserName, true);
            FormsAuthenticationTicket formsAuthTicket = FormsAuthentication.Decrypt(authCookie.Value);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(formsAuthTicket.Version, formsAuthTicket.Name, formsAuthTicket.IssueDate, formsAuthTicket.Expiration, formsAuthTicket.IsPersistent, string.Join("|", currentClient.Roles()), formsAuthTicket.CookiePath);
            authCookie.Value = FormsAuthentication.Encrypt(ticket);
            authCookie.Expires = formsAuthTicket.Expiration;

            SetRequestCookie(authCookie.Name, authCookie.Value, authCookie.Expires, authCookie.Domain, authCookie.Path, authCookie.HttpOnly, authCookie.Secure, authCookie.Shareable);
            SetResponseCookie(authCookie.Name, authCookie.Value, authCookie.Expires, authCookie.Domain, authCookie.Path, authCookie.HttpOnly, authCookie.Secure, authCookie.Shareable);

            User = new GenericPrincipal(new GenericIdentity(currentClient.UserName), currentClient.Roles());
        }

        public virtual Client GetCurrentClient()
        {
            if (_currentClient == null)
                _currentClient = ClientUtility.NewClient(1301, "jgett", "lnf123", "Getty", "James", ClientPrivilege.Staff | ClientPrivilege.StoreUser | ClientPrivilege.Administrator | ClientPrivilege.WebSiteAdmin | ClientPrivilege.StoreManager | ClientPrivilege.PhysicalAccess | ClientPrivilege.OnlineAccess | ClientPrivilege.Developer, true);
            return _currentClient;
        }
    }
}
