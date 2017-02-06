using StackExchange.Redis;

namespace LNF.Impl.Redis
{
    public static class RedisConnection
    {
        private static readonly ConnectionMultiplexer _redis;
        private static readonly RedisConnectionConfiguration _config;

        static RedisConnection()
        {
            _config = RedisConfigurationSection.Current.Connection;

            var options = new ConfigurationOptions
            {
                EndPoints = { { _config.Host, _config.Port } },
                ConnectTimeout = _config.ConnectionTimeoutInMilliseconds,
                Password = _config.AccessKey,
                Ssl = _config.Ssl,
                SyncTimeout = _config.OperationTimeoutInMilliseconds,
                AllowAdmin = _config.AllowAdmin,
                DefaultDatabase = _config.DatabaseId
            };

            _redis = ConnectionMultiplexer.Connect(options);
        }

        public static ConnectionMultiplexer Multiplexer
        {
            get { return _redis; }
        }

        public static RedisConnectionConfiguration Configuration
        {
            get { return _config; }
        }
    }
}
