using LNF;
using LNF.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Caching;
using System.Web;

namespace OnlineServices.Api
{
    public class ApiContext
    {
        private readonly ObjectCache _cache = new MemoryCache("ApiContext");
        private readonly Func<HttpContextBase> _getHttpContext;

        public IProvider Provider { get; }

        public HttpContextBase HttpContext => _getHttpContext();

        public ApiContext(IProvider provider, Func<HttpContextBase> fn)
        {
            Provider = provider;
            _getHttpContext = fn;
        }

        public IClient GetCurrentUser()
        {
            var result = GetContextItem<IClient>("CurrentUser");

            if (result == null)
            {
                string un = HttpContext.User.Identity.Name;
                result = Provider.Data.Client.GetClient(un);
                SetContextItem("CurrentUser", result);
            }

            return result;
        }

        public IEnumerable<IClientAccount> GetCurrentUserClientAccounts()
        {
            var result = GetContextItem<IEnumerable<IClientAccount>>("CurrentUserClientAccounts");

            if (result == null)
            {
                var clientId = GetCurrentUser().ClientID;
                result = Provider.Data.Client.GetActiveClientAccounts(clientId);
                SetContextItem("CurrentUserClientAccounts", result);
            }

            return result;
        }

        public T GetSessionItem<T>(string key, T defval)
        {
            var obj = HttpContext.Session[key];

            if (obj == null)
                return defval;
            else
                return (T)obj;
        }

        public void SetSessionItem(string key, object value)
        {
            HttpContext.Session[key] = value;
        }

        public void RemoveSessionItem(string key)
        {
            HttpContext.Session.Remove(key);
        }

        public T GetContextItem<T>(string key)
        {
            var obj = HttpContext.Items[key];

            if (obj == null)
                return default;
            else
                return (T)obj;
        }

        public void SetContextItem(string key, object value)
        {
            HttpContext.Items[key] = value;
        }        

        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public void RemoveCacheItem(string key)
        {
            _cache.Remove(key);
        }

        public object GetCacheItem(string key)
        {
            return _cache[key];
        }

        public void SetCacheItem(string key, object value)
        {
            _cache[key] = value;
        }


        public CurrentDryBoxAssignmentCollection GetCurrentDryBoxAssignments()
        {
            IEnumerable<DryBoxAssignmentInfo> source;

            var obj = GetCacheItem("CurrentDryBoxAssignments");

            if (obj == null)
            {
                source  = Provider.Data.DryBox.GetCurrentDryBoxAssignments();
                SetCacheItem("CurrentDryBoxAssignments", source);
            }
            else
            {
                source = (IEnumerable<DryBoxAssignmentInfo>)obj;
            }

            var result = new CurrentDryBoxAssignmentCollection(source);

            return result;
        }
    }
}
