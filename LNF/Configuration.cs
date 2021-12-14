using System;
using System.Configuration;

namespace LNF
{
    public class Configuration : ConfigurationSection
    {
        public static Configuration Current { get; }

        static Configuration()
        {
            if (!(ConfigurationManager.GetSection("lnf/provider") is Configuration current))
                throw new InvalidOperationException("The configuration section 'lnf/provider' is missing.");

            Current = current;
        }

        private Configuration() { }

        [ConfigurationProperty("production", IsRequired = true)]
        public bool Production
        {
            get { return Convert.ToBoolean(this["production"]); }
            set { this["production"] = value; }
        }

        [ConfigurationProperty("context")]
        public ContextElement Context
        {
            get { return this["context"] as ContextElement; }
            set { this["context"] = value; }
        }

        [ConfigurationProperty("dataAccess")]
        public DataAccessServiceElement DataAccess
        {
            get { return this["dataAccess"] as DataAccessServiceElement; }
            set { this["dataAccess"] = value; }
        }

        [ConfigurationProperty("email")]
        public EmailServiceElement Email
        {
            get { return this["email"] as EmailServiceElement; }
            set { this["email"] = value; }
        }

        [ConfigurationProperty("log")]
        public LogServiceElement Log
        {
            get { return this["log"] as LogServiceElement; }
            set { this["log"] = value; }
        }

        [ConfigurationProperty("control")]
        public ControlServiceElement Control
        {
            get { return this["control"] as ControlServiceElement; }
            set { this["control"] = value; }
        }
    }
}
