using LNF.Cache;
using LNF.Impl.Cache;
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

namespace LNF.Impl
{
    public class WebContext : ContextBase
    {
        public string LoginUrl { get; set; }

        public WebContext()
        {
            //if (HttpContext.Current == null)
            //    throw new NullReferenceException("HttpContext.Current");
        }

        public override object GetSessionValue(string key)
        {
            if (HttpContext.Current.Session == null)
                throw new InvalidOperationException("Session is null.");

            return HttpContext.Current.Session[key];
        }

        public override void SetSessionValue(string key, object obj)
        {
            if (HttpContext.Current.Session == null)
                throw new InvalidOperationException("Session is null.");

            HttpContext.Current.Session[key] = obj;
        }

        public override void RemoveSessionValue(string key)
        {
            if (HttpContext.Current.Session == null)
                throw new InvalidOperationException("Session is null.");

            HttpContext.Current.Session.Remove(key);
        }

        public override void AbandonSession()
        {
            if (HttpContext.Current.Session == null)
                throw new InvalidOperationException("Session is null.");

            HttpContext.Current.Session.Abandon();
        }

        public override void ClearSession()
        {
            if (HttpContext.Current.Session == null)
                throw new InvalidOperationException("Session is null.");

            HttpContext.Current.Session.Clear();
        }

        public override NameValueCollection ServerVariables
        {
            get { return HttpContext.Current.Request.ServerVariables; }
        }

        public override string GetServerPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }

        public override string GetAbsolutePath(string virtualPath)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        public override int GetScriptTimeout()
        {
            return HttpContext.Current.Server.ScriptTimeout;
        }

        public override void SetScriptTimeout(int value)
        {
            HttpContext.Current.Server.ScriptTimeout = value;
        }

        public override object GetApplicationVariable(string key)
        {
            return HttpContext.Current.Application[key];
        }

        public override void SetApplicationVariable(string key, object value)
        {
            HttpContext.Current.Application[key] = value;
        }

        public override Uri GetRequestUrl()
        {
            return HttpContext.Current.Request.Url;
        }

        public override bool GetRequestIsAuthenticated()
        {
            return HttpContext.Current.Request.IsAuthenticated;
        }

        public override bool GetRequestIsSecureConnection()
        {
            return HttpContext.Current.Request.IsSecureConnection;
        }

        public override string GetAuthCookieName()
        {
            return FormsAuthentication.FormsCookieName;
        }

        public override string GetAuthCookiePath()
        {
            return FormsAuthentication.FormsCookiePath;
        }

        public override string GetAuthCookieDomain()
        {
            return FormsAuthentication.CookieDomain;
        }

        public override string GetRequestUserAgent()
        {
            return HttpContext.Current.Request.UserAgent;
        }

        public override string GetRequestPhysicalApplicationPath()
        {
            return HttpContext.Current.Request.PhysicalApplicationPath;
        }

        public override object GetRequestValue(string key)
        {
            return HttpContext.Current.Request[key];
        }

        public override NameValueCollection QueryString
        {
            get { return HttpContext.Current.Request.QueryString; }
        }

        public override NameValueCollection PostData
        {
            get { return HttpContext.Current.Request.Form; }
        }

        public override int GetRequestFileCount()
        {
            return HttpContext.Current.Request.Files.Count;
        }

        public override int GetRequestFileContentLength(int index)
        {
            return HttpContext.Current.Request.Files[index].ContentLength;
        }

        public override Stream GetRequestFileInputStream(int index)
        {
            return HttpContext.Current.Request.Files[index].InputStream;
        }

        public override IDictionary Items
        {
            get { return HttpContext.Current.Items; }
        }

        public override string GetRequestCookieValue(string name)
        {
            return GetCookieValue(HttpContext.Current.Request.Cookies, name);
        }

        public override void SetRequestCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            AddOrUpdateCookie(HttpContext.Current.Request.Cookies, name, value, expires, domain, path, httpOnly, secure, shareable);
        }

        public override string GetResponseCookieValue(string name)
        {
            return GetCookieValue(HttpContext.Current.Response.Cookies, name);
        }

        public override void SetResponseCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            AddOrUpdateCookie(HttpContext.Current.Response.Cookies, name, value, expires, domain, path, httpOnly, secure, shareable);
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
            get { return HttpContext.Current.User; }
            set { HttpContext.Current.User = value; }
        }

        public override string UserHostAddress
        {
            get
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        public virtual Client LogIn(string username, string password)
        {
            object pwobj = (password.Equals(Providers.DataAccess.UniversalPassword)) ? null : Providers.Encryption.EncryptText(password);
            Client client = DA.Current.QueryBuilder()
                .AddParameter("UserName", username)
                .AddParameter("Password", pwobj)
                .AddParameter("IPAddress", UserHostAddress)
                .SqlQuery("EXEC sselData.dbo.Client_Select @Action='LoginCheck', @UserName=:UserName, @Password=:Password, @IPAddress=:IPAddress")
                .List<Client>()
                .FirstOrDefault();
            return client;
        }

        public override void SignOut()
        {
            FormsAuthentication.RedirectToLoginPage();
        }

        public override void AddResponseHeader(string name, string value)
        {
            HttpContext.Current.Response.AddHeader(name, value);
        }

        public override string GetResponseCharset()
        {
            return HttpContext.Current.Response.Charset;
        }

        public override void SetResponseCharset(string value)
        {
            HttpContext.Current.Response.Charset = value;
        }

        public override void Redirect(string url)
        {
            HttpContext.Current.Response.Redirect(url);
        }

        public override string UrlEncode(string text)
        {
            return HttpUtility.UrlEncode(text);
        }
    }
}
