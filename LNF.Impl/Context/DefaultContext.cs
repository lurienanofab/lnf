using LNF.Impl.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace LNF.Impl.Context
{
    public class DefaultContext : ContextBase
    {
        public class CookieCollection
        {
            private List<KeyValuePair<string, Cookie>> _items;

            public CookieCollection()
            {
                _items = new List<KeyValuePair<string, Cookie>>();
            }

            public Cookie this[string name]
            {
                get
                {
                    return _items.FirstOrDefault(x => x.Key == name).Value;
                }
            }

            public void Set(Cookie item)
            {
                int index = -1;
                KeyValuePair<string, Cookie> kvp = new KeyValuePair<string, Cookie>();

                // first determine if item is already in the colleciton
                if (_items.Any(x => x.Value == item))
                {
                    kvp = _items.First(x => x.Value == item);
                    index = _items.IndexOf(kvp);
                }

                // next determine if item.Name is an existing key
                else if (_items.Any(x => x.Key == item.Name))
                {
                    kvp = _items.First(x => x.Key == item.Name);
                    index = _items.IndexOf(kvp);
                }

                if (index != -1)
                {
                    // replace the existing item, keep the same key (key and item.Name may be different at this point)
                    _items[index] = new KeyValuePair<string, Cookie>(kvp.Key, item);
                }
                else
                {
                    // this must be a new item
                    _items.Add(new KeyValuePair<string, Cookie>(item.Name, item));
                }
            }

            public void Add(Cookie item)
            {
                // duplicate keys are allowed
                _items.Add(new KeyValuePair<string, Cookie>(item.Name, item));
            }
        }

        public class Cookie
        {
            public Cookie() { }

            public Cookie(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
            public string Value { get; set; }
            public DateTime Expires { get; set; }
            public string Domain { get; set; }
            public string Path { get; set; }
            public bool HttpOnly { get; set; }
            public bool Secure { get; set; }
            public bool Shareable { get; set; }
        }

        private Dictionary<string, object> _session;
        private readonly NameValueCollection _server;
        private Dictionary<string, object> _application;
        private readonly NameValueCollection _queryString;
        private readonly NameValueCollection _post;
        private readonly Dictionary<object, object> _items;
        private readonly CookieCollection _requestCookies;
        private readonly CookieCollection _responseCookies;

        public Uri RequestUrl { get; set; }
        public bool RequestIsAuthenticated { get; set; }
        public bool RequestIsSecureConnection { get; set; }
        public string AuthCookieName { get; set; }
        public string AuthCookiePath { get; set; }
        public string AuthCookieDomain { get; set; }
        public string RequestUserAgent { get; set; }
        public string CurrentUserHostAddress { get; set; }
        public int ScriptTimeout { get; set; }

        public DefaultContext(ISessionManager mgr) : base(mgr)
        {
            RequestUrl = null;
            RequestIsAuthenticated = false;
            RequestIsSecureConnection = false;
            AuthCookieName = string.Empty;
            RequestUserAgent = string.Empty;
            CurrentUserHostAddress = string.Empty;
            ScriptTimeout = 30;
            _session = new Dictionary<string, object>();
            _server = new NameValueCollection();
            _application = new Dictionary<string, object>();
            _queryString = new NameValueCollection();
            _post = new NameValueCollection();
            _items = new Dictionary<object, object>();
            _requestCookies = new CookieCollection();
            _responseCookies = new CookieCollection();
        }

        public override object GetSessionValue(string key)
        {
            if (_session.ContainsKey(key))
                return _session[key];
            return null;
        }

        public override void SetSessionValue(string key, object obj)
        {
            if (_session.ContainsKey(key))
                _session[key] = obj;
            else
                _session.Add(key, obj);
        }

        public override void RemoveSessionValue(string key)
        {
            _session.Remove(key);
        }

        public override void AbandonSession()
        {
            _session = new Dictionary<string, object>();
        }

        public override void ClearSession()
        {
            _session.Clear();
        }

        public override NameValueCollection ServerVariables
        {
            get { return _server; }
        }

        public override string GetServerPath(string path)
        {
            string result = path.Replace(".", AppDomain.CurrentDomain.BaseDirectory);
            return result;
        }

        public override string GetAbsolutePath(string virtualPath)
        {
            string result = virtualPath.Replace("~", string.Empty);
            return result;
        }

        public override int GetScriptTimeout()
        {
            return ScriptTimeout;
        }

        public override void SetScriptTimeout(int value)
        {
            ScriptTimeout = value;
        }

        public override object GetApplicationVariable(string key)
        {
            return _application[key];
        }

        public override void SetApplicationVariable(string key, object value)
        {
            if (_application.ContainsKey(key))
                _application[key] = value;
            else
                _application.Add(key, value);
        }

        public override Uri GetRequestUrl()
        {
            return RequestUrl;
        }

        public override bool GetRequestIsAuthenticated()
        {
            return RequestIsAuthenticated;
        }

        public override bool GetRequestIsSecureConnection()
        {
            return RequestIsSecureConnection;
        }

        public override string GetAuthCookieName()
        {
            return AuthCookieName;
        }

        public override string GetAuthCookiePath()
        {
            return AuthCookiePath;
        }

        public override string GetAuthCookieDomain()
        {
            return AuthCookieDomain;
        }

        public override string GetRequestUserAgent()
        {
            return RequestUserAgent;
        }

        public override string GetRequestPhysicalApplicationPath()
        {
            return global::System.Reflection.Assembly.GetEntryAssembly().Location;
        }

        public override object GetRequestValue(string key)
        {
            // The System.Web.HttpRequest.QueryString, System.Web.HttpRequest.Form, System.Web.HttpRequest.Cookies,
            // or System.Web.HttpRequest.ServerVariables collection member specified in the
            // key parameter. If the specified key is not found, then null is returned.

            string value;

            value = QueryString[key];
            if (!string.IsNullOrEmpty(value))
                return value;

            value = PostData[key];
            if (!string.IsNullOrEmpty(value))
                return value;

            value = GetRequestCookieValue(key);
            if (!string.IsNullOrEmpty(value))
                return value;

            value = ServerVariables[key];
            if (!string.IsNullOrEmpty(value))
                return value;

            return null;
        }

        public override NameValueCollection QueryString
        {
            get { return _queryString; }
        }

        public override NameValueCollection PostData
        {
            get { return _post; }
        }

        public override int GetRequestFileCount()
        {
            return 0;
        }

        public override int GetRequestFileContentLength(int index)
        {
            return 0;
        }

        public override Stream GetRequestFileInputStream(int index)
        {
            return null;
        }

        public override IDictionary Items
        {
            get { return _items; }
        }

        public override string GetRequestCookieValue(string name)
        {
            return GetCookieValue(_requestCookies, name);
        }

        public override void SetRequestCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            AddOrUpdateCookie(_requestCookies, name, value, expires, domain, path, httpOnly, secure, shareable);
        }

        public override string GetResponseCookieValue(string name)
        {
            return GetCookieValue(_responseCookies, name);
        }

        public override void SetResponseCookie(string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            AddOrUpdateCookie(_responseCookies, name, value, expires, domain, path, httpOnly, secure, shareable);
        }

        private void AddOrUpdateCookie(CookieCollection cookies, string name, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure, bool shareable)
        {
            if (cookies[name] == null)
                cookies.Add(new Cookie(name));

            cookies[name].Value = value;
            cookies[name].Expires = expires;
            cookies[name].Domain = domain;
            cookies[name].HttpOnly = httpOnly;
            cookies[name].Path = path;
            cookies[name].Secure = secure;
            cookies[name].Shareable = shareable;
        }

        private string GetCookieValue(CookieCollection cookies, string name)
        {
            if (cookies[name] != null)
                return cookies[name].Value;
            else
                return null;
        }

        public override IPrincipal User
        {
            get { return Thread.CurrentPrincipal; }
            set { Thread.CurrentPrincipal = value; }
        }

        public override string UserHostAddress
        {
            get { return CurrentUserHostAddress; }
        }

        public override void SignOut()
        {
            throw new NotImplementedException();
        }

        public override void AddResponseHeader(string name, string value)
        {
            throw new NotImplementedException();
        }

        public override string GetResponseCharset()
        {
            throw new NotImplementedException();
        }

        public override void SetResponseCharset(string value)
        {
            throw new NotImplementedException();
        }

        public override void Redirect(string url)
        {
            throw new NotImplementedException();
        }

        public override string UrlEncode(string text)
        {
            return Uri.EscapeDataString(text);
        }
    }
}
