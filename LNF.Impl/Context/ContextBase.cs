using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Security.Principal;

namespace LNF.Impl.Context
{
    public abstract class ContextBase : RepositoryBase, IContext
    {
        public ContextBase(ISessionManager mgr) : base(mgr)
        {
            LoginUrl = Configuration.Current.Context.LoginUrl;
        }

        public string LoginUrl { get; }

        public abstract IPrincipal User { get; set; }

        public abstract string UserHostAddress { get; }

        public abstract void AbandonSession();

        public abstract void AddResponseHeader(string name, string value);

        public abstract void ClearSession();

        public abstract string GetAbsolutePath(string virtualPath);

        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public abstract object GetApplicationVariable(string key);

        public abstract void SetApplicationVariable(string key, object value);

        public abstract string GetAuthCookieDomain();

        public abstract string GetAuthCookieName();

        public abstract string GetAuthCookiePath();

        public abstract IDictionary Items { get; }

        public virtual T GetItem<T>(string key)
        {
            object result = Items[key];
            if (result == null) return default(T);
            else return (T)result;
        }

        public virtual void SetItem<T>(string key, T item)
        {
            if (Items.Contains(key))
                Items[key] = item;
            else
                Items.Add(key, item);
        }

        public abstract NameValueCollection PostData { get; }

        public abstract NameValueCollection QueryString { get; }

        public abstract string GetRequestCookieValue(string name);

        public abstract int GetRequestFileContentLength(int index);

        public abstract int GetRequestFileCount();

        public abstract Stream GetRequestFileInputStream(int index);

        public abstract bool GetRequestIsAuthenticated();

        public abstract bool GetRequestIsSecureConnection();

        public abstract string GetRequestPhysicalApplicationPath();

        public abstract Uri GetRequestUrl();

        public abstract string GetRequestUserAgent();

        public abstract object GetRequestValue(string key);

        public abstract string GetResponseCharset();

        public abstract string GetResponseCookieValue(string name);

        public abstract int GetScriptTimeout();

        public abstract string GetServerPath(string path);

        public abstract NameValueCollection ServerVariables { get; }

        public abstract object GetSessionValue(string key);

        public abstract void Redirect(string url);

        public abstract void RemoveSessionValue(string key);

        public abstract void SetRequestCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable);

        public abstract void SetResponseCharset(string value);

        public abstract void SetResponseCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable);

        public abstract void SetScriptTimeout(int value);

        public abstract void SetSessionValue(string key, object obj);

        public abstract void SignOut();

        public abstract string UrlEncode(string text);
    }

}
