using LNF.Cache;
using LNF.Impl.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace LNF.Impl.Cache
{
    public class RedisCache
    {
        public static RedisCache Default { get; }

        static RedisCache()
        {
            Default = new RedisCache();
        }

        private IServer _server;
        private IDatabase _database;

        public RedisCache()
        {
            _server = RedisConnection.Multiplexer.GetServer(RedisConnection.Configuration.Host, RedisConnection.Configuration.Port);
            _database = RedisConnection.Multiplexer.GetDatabase(RedisConnection.Configuration.DatabaseId);
        }

        public T Get<T>(string key)
        {
            var json = _database.StringGet(key);
            if (json.IsNull) return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Set(string key, object value, TimeSpan? expiry = default(TimeSpan?))
        {
            string json = JsonConvert.SerializeObject(value);
            _database.StringSet(key, json, expiry);
        }

        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }

        public void Expire(string key, TimeSpan? expiry)
        {
            _database.KeyExpire(key, expiry);
        }

        public TimeSpan? TTL(string key)
        {
            return _database.KeyTimeToLive(key);
        }

        public TimeSpan? Ping()
        {
            return _database.Ping();
        }

        public IDatabase GetDatabase()
        {
            return _database;
        }
    }
}
