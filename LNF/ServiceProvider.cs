using LNF.Hooks;
using LNF.Models;
using LNF.Models.Billing;
using LNF.Models.Mail;
using LNF.Repository;
using System;
using System.Configuration;

namespace LNF
{
    public class ServiceProvider
    {
        private IDependencyResolver _resolver;

        public IContext Context => Use<IContext>();
        public IDataAccessService DataAccess => Use<IDataAccessService>();
        public ILogService Log => Use<ILogService>();
        public IControlService Control => Use<IControlService>();
        public IEncryptionService Encryption => Use<IEncryptionService>();
        public ISerializationService Serialization => Use<ISerializationService>();

        public IDataService Data => Use<IDataService>();
        public IBillingApi Billing => Use<IBillingApi>();
        public IMailApi Mail => Use<IMailApi>();
        public IPhysicalAccessService PhysicalAccess => Use<IPhysicalAccessService>();
        public ISchedulerService Scheduler => Use<ISchedulerService>();
        public IWorkerService Worker => Use<IWorkerService>();

        public IModelFactory ModelFactory => Use<IModelFactory>();

        public ServiceProvider(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public static ServiceProvider Current { get; set; }

        public T Use<T>() => _resolver.GetInstance<T>();

        public void BuildUp(object target) => _resolver.BuildUp(target);

        public bool IsProduction()
        {
            return GetConfigurationSection().Production;
        }

        public string[] Hooks { get { return HookManager.GetHookTypes(); } }

        public static ServiceProviderSection GetConfigurationSection()
        {
            ServiceProviderSection result = ConfigurationManager.GetSection("lnf/provider") as ServiceProviderSection;

            if (result == null)
                throw new InvalidOperationException("The configuration section 'lnf/provider' is missing.");

            return result;
        }
    }

    public abstract class ServiceElement : ConfigurationElement
    {
        protected T GetProperty<T>(string propertyName, T defval)
        {
            if (this[propertyName] == null)
                return defval;

            T result = (T)this[propertyName];

            if (result == null)
                return defval;
            else
                return result;
        }

        protected T RequireProperty<T>(string propertyName, T defval)
        {
            if (this[propertyName] == null)
                throw new InvalidOperationException($"The attribute '{propertyName}' is required.");

            var result = GetProperty(propertyName, defval);

            if (result.Equals(defval))
                throw new InvalidOperationException($"The attribute '{propertyName}' is required.");

            return result;
        }
    }

    public abstract class WebServiceElement : ServiceElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public Uri Host
        {
            get { return new Uri(this["host"].ToString()); }
            set { this["host"] = value; }
        }
    }

    public class ServiceProviderSection : ConfigurationSection
    {
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

    public class ContextElement : ServiceElement
    {
        [ConfigurationProperty("loginUrl", IsRequired = true)]
        public string LoginUrl
        {
            get { return RequireProperty("loginUrl", string.Empty); }
            set { this["loginUrl"] = value; }
        }
    }

    public class DataAccessServiceElement : ServiceElement
    {
        [ConfigurationProperty("showSql", IsRequired = false, DefaultValue = false)]
        public bool ShowSql
        {
            get { return GetProperty("showSql", false); }
            set { this["showSql"] = value; }
        }

        [ConfigurationProperty("universalPassword", IsRequired = false, DefaultValue = "")]
        public string UniversalPassword
        {
            get { return GetProperty("universalPassword", string.Empty); }
            set { this["universalPassword"] = value; }
        }
    }

    public class LogServiceElement : ServiceElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return RequireProperty("name", string.Empty); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get { return GetProperty("enabled", false); }
            set { this["enabled"] = value; }
        }
    }

    public class EmailServiceElement : ServiceElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return RequireProperty("host", string.Empty); }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = false)]
        public int Port
        {
            get { return GetProperty("port", 25); }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("username", IsRequired = false, DefaultValue = "")]
        public string Username
        {
            get { return GetProperty("username", string.Empty); }
            set { this["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false, DefaultValue = "")]
        public string Password
        {
            get { return GetProperty("password", string.Empty); }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("enableSsl", IsRequired = false, DefaultValue = false)]
        public bool EnableSsl
        {
            get { return GetProperty("enableSsl", false); }
            set { this["enableSsl"] = value; }
        }

        [ConfigurationProperty("log", IsRequired = false)]
        public bool Log
        {
            get { return GetProperty("log", true); }
            set { this["log"] = value; }
        }
    }

    public class ControlServiceElement : WebServiceElement { }
}
