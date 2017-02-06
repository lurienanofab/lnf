using System;
using System.Security.Principal;

namespace OnlineServices.Api
{
    public interface IApiContextProvider
    {
        IPrincipal User { get; }
        ApiClientOptions GetClientOptions();
        object GetSessionValue(string key);
        void SetSessionValue(string key, object value);
        void RemoveSessionValue(string key);
        object GetContextItem(string key);
        void SetContextItem(string key, object value);
        void RemoveContextItem(string key);
        object GetCacheItem(string key);
        void SetCacheItem(string key, object value, DateTimeOffset absoluteExpiration);
        void RemoveCacheItem(string key);
    }
}
