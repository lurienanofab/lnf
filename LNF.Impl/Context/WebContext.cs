using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace LNF.Impl.Context
{
    public class WebContext : ContextBase
    {
        private IHttpContextFactory _factory;

        public virtual HttpContextBase ContextBase => _factory.CreateContext();

        public WebContext(IHttpContextFactory factory)
        {
            _factory = factory;
        }

        public override object GetSessionValue(string key)
        {
            if (ContextBase.Session == null)
                throw new InvalidOperationException("Session is null.");

            return ContextBase.Session[key];
        }

        public override void SetSessionValue(string key, object obj)
        {
            if (ContextBase.Session == null)
                throw new InvalidOperationException("Session is null.");

            ContextBase.Session[key] = obj;
        }

        public override void RemoveSessionValue(string key)
        {
            if (ContextBase.Session == null)
                throw new InvalidOperationException("Session is null.");

            ContextBase.Session.Remove(key);
        }

        public override void AbandonSession()
        {
            if (ContextBase.Session == null)
                throw new InvalidOperationException("Session is null.");

            ContextBase.Session.Abandon();
        }

        public override void ClearSession()
        {
            if (ContextBase.Session == null)
                throw new InvalidOperationException("Session is null.");

            ContextBase.Session.Clear();
        }

        public override NameValueCollection ServerVariables => ContextBase.Request.ServerVariables;

        public override string GetServerPath(string path) => ContextBase.Server.MapPath(path);

        public override string GetAbsolutePath(string virtualPath) => VirtualPathUtility.ToAbsolute(virtualPath);

        public override int GetScriptTimeout() => ContextBase.Server.ScriptTimeout;

        public override void SetScriptTimeout(int value) => ContextBase.Server.ScriptTimeout = value;

        public override object GetApplicationVariable(string key) => ContextBase.Application[key];

        public override void SetApplicationVariable(string key, object value) => ContextBase.Application[key] = value;

        public override Uri GetRequestUrl() => ContextBase.Request.Url;

        public override bool GetRequestIsAuthenticated() => ContextBase.Request.IsAuthenticated;

        public override bool GetRequestIsSecureConnection() => ContextBase.Request.IsSecureConnection;

        public override string GetAuthCookieName() => FormsAuthentication.FormsCookieName;

        public override string GetAuthCookiePath() => FormsAuthentication.FormsCookiePath;

        public override string GetAuthCookieDomain() => FormsAuthentication.CookieDomain;

        public override string GetRequestUserAgent() => ContextBase.Request.UserAgent;

        public override string GetRequestPhysicalApplicationPath() => ContextBase.Request.PhysicalApplicationPath;

        public override object GetRequestValue(string key) => ContextBase.Request[key];

        public override NameValueCollection QueryString => ContextBase.Request.QueryString;

        public override NameValueCollection PostData => ContextBase.Request.Form;

        public override int GetRequestFileCount() => ContextBase.Request.Files.Count;

        public override int GetRequestFileContentLength(int index) => ContextBase.Request.Files[index].ContentLength;

        public override Stream GetRequestFileInputStream(int index) => ContextBase.Request.Files[index].InputStream;

        public override IDictionary Items => ContextBase.Items;

        public override string GetRequestCookieValue(string name) => GetCookieValue(ContextBase.Request.Cookies, name);

        public override void SetRequestCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            AddOrUpdateCookie(ContextBase.Request.Cookies, name, value, expires, domain, path, httpOnly, secure, shareable);
        }

        public override string GetResponseCookieValue(string name) => GetCookieValue(ContextBase.Response.Cookies, name);

        public override void SetResponseCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            AddOrUpdateCookie(ContextBase.Response.Cookies, name, value, expires, domain, path, httpOnly, secure, shareable);
        }

        private void AddOrUpdateCookie(HttpCookieCollection cookies, string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            if (cookies[name] == null)
                cookies.Add(new HttpCookie(name));

            cookies[name].Value = value;
            cookies[name].Expires = expires;
            cookies[name].Domain = domain;
            cookies[name].HttpOnly = httpOnly;
            cookies[name].Path = path;
            cookies[name].Secure = secure;
            cookies[name].Shareable = shareable;
        }

        private string GetCookieValue(HttpCookieCollection cookies, string name)
        {
            if (cookies[name] != null)
                return cookies[name].Value;
            else
                return null;
        }

        public override IPrincipal User
        {
            get { return ContextBase.User; }
            set { ContextBase.User = value; }
        }

        public override string UserHostAddress => ContextBase.Request.UserHostAddress;

        public virtual Client LogIn(string username, string password)
        {
            string pw;

            if (!string.IsNullOrEmpty(ServiceProvider.Current.DataAccess.UniversalPassword) && password.Equals(ServiceProvider.Current.DataAccess.UniversalPassword))
                pw = null;
            else
                pw = ServiceProvider.Current.Encryption.EncryptText(password);

            string ip = UserHostAddress;

            string sql = "EXEC sselData.dbo.Client_Select @Action='LoginCheck', @UserName=:username, @Password=:pw, @IPAddress=:ip";
            Client client = DA.Current.SqlQuery(sql).SetParameters(new { username, pw, ip }).List<Client>().FirstOrDefault();

            return client;
        }

        public override void SignOut() => FormsAuthentication.RedirectToLoginPage();

        public override void AddResponseHeader(string name, string value) => ContextBase.Response.AddHeader(name, value);

        public override string GetResponseCharset() => ContextBase.Response.Charset;

        public override void SetResponseCharset(string value) => ContextBase.Response.Charset = value;

        public override void Redirect(string url) => ContextBase.Response.Redirect(url);

        public override string UrlEncode(string text) => HttpUtility.UrlEncode(text);
    }
}
