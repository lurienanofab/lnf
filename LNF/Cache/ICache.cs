using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Cache
{
    public interface ICache
    {
        object this[string key] { get; set; }
        bool Contains(string key);
        object GetValue(string key);
        T GetValue<T>(string key, Func<IProvider, T> defval, DateTimeOffset? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
        void SetValue(string key, object value, DateTimeOffset? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
        object RemoveValue(string key);
        void ClearCache();
        long GetApproximateSize();
        bool ShowCanceledForModification { get; }
        bool UseStartReservationPage { get; }
        bool WagoEnabled { get; }
        IEnumerable<IClient> Clients();
        IClient GetClient(int clientId);
        IClient GetClient(string username);
        bool IsProduction();

        //void AbandonSession();
        //IClient CheckSession();
        //IClient CheckSession(IClient client);
        //IClient CurrentUser { get; }
        //string GetCurrentUserName();
        //T GetSessionValue<T>(string key, Func<IProvider, T> defval);
        //T GetContextItem<T>(string key);
        //void RemoveContextItem(string key);
        //void RemoveSessionValue(string key);
        //void SetContextItem<T>(string key, T item);
        //void SetSessionValue(string key, object value);
    }
}