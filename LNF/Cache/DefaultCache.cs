using LNF.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;

namespace LNF.Cache
{
    public class DefaultCache : ICache
    {
        private readonly IProvider _provider;
        private MemoryCache _cache;

        public DefaultCache(IProvider provider)
        {
            _provider = provider;
            _cache = new MemoryCache("DefaultCache");
        }

        public object this[string key]
        {
            get => _cache[key];
            set => _cache[key] = value;
        }

        public bool Contains(string key) => _cache.Contains(key);

        public object GetValue(string key) => _cache.Get(key);

        public T GetValue<T>(string key, Func<IProvider, T> defval, DateTimeOffset? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            object value = GetValue(key);

            T result;

            if (value == null || !(value is T))
            {
                result = defval(_provider);
                SetValue(key, result, absoluteExpiration, slidingExpiration);
            }
            else
            {
                result = (T)value;
            }

            return result;
        }

        public void SetValue(string key, object value, DateTimeOffset? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            _cache.Set(key, value, new CacheItemPolicy()
            {
                AbsoluteExpiration = absoluteExpiration.GetValueOrDefault(ObjectCache.InfiniteAbsoluteExpiration),
                SlidingExpiration = slidingExpiration.GetValueOrDefault()
            });
        }

        public object RemoveValue(string key) => _cache.Remove(key);

        public void ClearCache()
        {
            _cache.Dispose();
            _cache = new MemoryCache("DefaultCache");
        }

        /// <summary>
        /// Gets the approximate size of the cache. See https://stackoverflow.com/questions/22392634/how-to-measure-current-size-of-net-memory-cache-4-0
        /// </summary>
        public long GetApproximateSize()
        {
            try
            {
                var statsField = typeof(MemoryCache).GetField("_stats", BindingFlags.NonPublic | BindingFlags.Instance);
                var statsValue = statsField.GetValue(_cache);
                var monitorField = statsValue.GetType().GetField("_cacheMemoryMonitor", BindingFlags.NonPublic | BindingFlags.Instance);
                var monitorValue = monitorField.GetValue(statsValue);
                var sizeField = monitorValue.GetType().GetField("_sizedRefMultiple", BindingFlags.NonPublic | BindingFlags.Instance);
                var sizeValue = sizeField.GetValue(monitorValue);
                var approxProp = sizeValue.GetType().GetProperty("ApproximateSize", BindingFlags.NonPublic | BindingFlags.Instance);
                return (long)approxProp.GetValue(sizeValue, null);
            }
            catch
            {
                return -1;
            }
        }

        public bool ShowCanceledForModification => GetBooleanOptionalAppSetting("ShowCanceledForModification");

        public bool UseStartReservationPage => GetBooleanOptionalAppSetting("UseStartReservationPage");

        public bool WagoEnabled => GetBooleanOptionalAppSetting("WagoEnabled");

        public bool IsProduction() => ServiceProvider.Current.IsProduction();

        private bool GetBooleanOptionalAppSetting(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            bool.TryParse(setting, out bool result);
            return result;
        }
    }
}
