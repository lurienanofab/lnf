using FluentNHibernate.Cfg;
using LNF.Impl.Redis;
using NHibernate.Caches.Redis;
using System;
using System.Linq;
using System.Net;

namespace LNF.Impl
{
    public static class NHibernateRedisCacheExtensions
    {
        public static IDisposable ConfigureRedisCache(this FluentConfiguration cfg)
        {
            var opts = new RedisCacheProviderOptions()
            {
                Database = 1,
                CacheConfigurations = new[]
                {
                    new RedisCacheConfiguration(string.Empty) { SlidingExpiration = TimeSpan.FromDays(7), Expiration = TimeSpan.FromHours(24) },
                    new RedisCacheConfiguration("FiveSecondExpiration") { Expiration = TimeSpan.FromSeconds(5) },
                    new RedisCacheConfiguration("FiveMinuteExpiration") { Expiration = TimeSpan.FromMinutes(5) }
                }
            };

            RedisCacheProvider.SetOptions(opts);
            RedisCacheProvider.SetConnectionMultiplexer(RedisConnection.Multiplexer);
            cfg.Cache(c => c.ProviderClass<RedisCacheProvider>().UseSecondLevelCache().UseQueryCache());
            var server = RedisConnection.Multiplexer.GetServer(RedisConnection.Multiplexer.GetEndPoints().First());
            SessionLog.AddLogMessage(string.Format("Cache Server: {0}", server.EndPoint.GetHostAndPort()));
            SessionLog.AddLogMessage(string.Format("Ping: {0}", server.Ping()));
            
            return null;
        }

        public static string GetHostAndPort(this EndPoint value)
        {
            if (typeof(DnsEndPoint).IsAssignableFrom(value.GetType()))
            {
                DnsEndPoint ep = (DnsEndPoint)value;
                return string.Format("{0}:{1}", ep.Host, ep.Port);
            }
            else if (typeof(IPEndPoint).IsAssignableFrom(value.GetType()))
            {
                IPEndPoint ep = (IPEndPoint)value;
                return string.Format("{0}:{1}", ep.Address, ep.Port);
            }
            else
                return value.ToString();
        }
    }
}
