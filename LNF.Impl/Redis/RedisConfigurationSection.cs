using System.Configuration;

namespace LNF.Impl.Redis
{
    // Use the following web.config file (optional).
    //<?xml version="1.0" encoding="utf-8" ?>
    //<configuration>
    //  <configSections>
    //    <section name="redisCache" type="RedisObjectCache.RedisCacheConfiguration, RedisObjectCache" />
    //  </configSections>
    //  <redisCache>
    //    <connection host = "127.0.0.1" [String]
    //          port = "" [number]
    //          accessKey = "" [String]
    //          ssl = "false" [true|false]
    //          databaseId = "0" [number]
    //          connectionTimeoutInMilliseconds = "5000" [number]
    //          operationTimeoutInMilliseconds = "5000" [number] />
    //  </redisCache>
    //</configuration>

    public sealed class RedisConfigurationSection : ConfigurationSection
    {
        private readonly static RedisConfigurationSection _Current;

        static RedisConfigurationSection()
        {
            object obj = ConfigurationManager.GetSection("lnf/redis");

            if (obj != null)
                _Current = (RedisConfigurationSection)obj;
            else
                _Current = new RedisConfigurationSection();
        }

        public static RedisConfigurationSection Current
        {
            get { return _Current; }
        }

        private RedisConnectionConfiguration _defaultConnectionConfig;

        private RedisConnectionConfiguration GetDefaultConnectionConfiguration()
        {
            if (_defaultConnectionConfig == null)
                _defaultConnectionConfig = new RedisConnectionConfiguration();
            return _defaultConnectionConfig;
        }

        [ConfigurationProperty("connection", IsRequired = false)]
        public RedisConnectionConfiguration Connection
        {
            get
            {
                if (base["connection"] == null)
                    return GetDefaultConnectionConfiguration();
                else
                    return (RedisConnectionConfiguration)base["connection"];
            }
        }
    }

    public class RedisConnectionConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("host", DefaultValue = "127.0.0.1", IsRequired = false)]
        public string Host
        {
            get { return (string)base["host"]; }
            set { base["host"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue = 6379, IsRequired = false)]
        public int Port
        {
            get { return (int)base["port"]; }
            set { base["port"] = value; }
        }

        [ConfigurationProperty("accessKey", DefaultValue = "", IsRequired = false)]
        public string AccessKey
        {
            get { return (string)base["accessKey"]; }
            set { base["accessKey"] = value; }
        }

        [ConfigurationProperty("ssl", DefaultValue = false, IsRequired = false)]
        public bool Ssl
        {
            get { return (bool)base["ssl"]; }
            set { base["ssl"] = value; }
        }

        [ConfigurationProperty("databaseId", DefaultValue = 0, IsRequired = false)]
        public int DatabaseId
        {
            get { return (int)base["databaseId"]; }
            set { base["databaseId"] = value; }
        }

        [ConfigurationProperty("connectionTimeoutInMilliseconds", DefaultValue = 5000, IsRequired = false)]
        public int ConnectionTimeoutInMilliseconds
        {
            get { return (int)base["connectionTimeoutInMilliseconds"]; }
            set { base["connectionTimeoutInMilliseconds"] = value; }
        }

        [ConfigurationProperty("operationTimeoutInMilliseconds", DefaultValue = 5000, IsRequired = false)]
        public int OperationTimeoutInMilliseconds
        {
            get { return (int)base["operationTimeoutInMilliseconds"]; }
            set { base["operationTimeoutInMilliseconds"] = value; }
        }

        [ConfigurationProperty("allowAdmin", DefaultValue = true, IsRequired = false)]
        public bool AllowAdmin
        {
            get { return (bool)base["allowAdmin"]; }
            set { base["allowAdmin"] = value; }
        }
    }
}
