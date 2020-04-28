using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Security.Principal;

namespace LNF
{
    /// <summary>
    /// This is a use-once-and-throw-away wrapper for the current request context
    /// </summary>
    public interface IContext
    {
        string LoginUrl { get; }
        object GetSessionValue(string key);
        void SetSessionValue(string key, object obj);
        void RemoveSessionValue(string key);
        void AbandonSession();
        void ClearSession();
        NameValueCollection ServerVariables { get; }
        string GetServerPath(string path);
        string GetAbsolutePath(string virtualPath);
        int GetScriptTimeout();
        void SetScriptTimeout(int value);
        object GetApplicationVariable(string key);
        void SetApplicationVariable(string key, object value);
        string GetAppSetting(string key);
        Uri GetRequestUrl();
        bool GetRequestIsAuthenticated();
        bool GetRequestIsSecureConnection();
        string GetAuthCookieName();
        string GetAuthCookiePath();
        string GetAuthCookieDomain();
        string GetRequestUserAgent();
        string GetRequestPhysicalApplicationPath();
        object GetRequestValue(string key);
        NameValueCollection QueryString { get; }
        NameValueCollection PostData { get; }
        int GetRequestFileCount();
        int GetRequestFileContentLength(int index);
        Stream GetRequestFileInputStream(int index);
        IPrincipal User { get; set; }
        string UserHostAddress { get; }
        IDictionary Items { get; }
        T GetItem<T>(string key);
        void SetItem<T>(string key, T item);
        string GetRequestCookieValue(string name);
        void SetRequestCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable);
        string GetResponseCookieValue(string name);
        void SetResponseCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable);
        void SignOut();
        void AddResponseHeader(string name, string value);
        string GetResponseCharset();
        void SetResponseCharset(string value);
        void Redirect(string url);
        string UrlEncode(string text);
    }
}
