using LNF.Hooks;
using System;
using System.ComponentModel;
using System.Configuration;

namespace LNF
{
    //just a label
    public interface ITypeProvider { }

    public interface IServiceTypeProvider : ITypeProvider
    {
        Uri Host { get; set; }
    }

    public static class Providers
    {
        private static readonly IContextProvider _contextProvider;
        private static readonly IDataAccessProvider _dataAccessProvider;
        private static readonly ILogProvider _logProvider;
        private static readonly IEmailProvider _emailProvider;
        private static readonly IControlProvider _controlProvider;
        private static readonly IEncryptionProvider _encryptionProvider;
        private static readonly ISerializationProvider _serializationProvider;
        private static readonly IPhysicalAccessProvider _physicalAccessProvider;
        private static readonly IScriptingProvider _scriptingProvider;
        private static readonly IModelFactory _modelFactory;


        static Providers()
        {
            _contextProvider = GetContextProvider();
            _dataAccessProvider = GetDataAccessProvider();
            _logProvider = GetLogProvider();
            _emailProvider = GetEmailProvider();
            _controlProvider = GetServiceProvider(cfg => cfg.Control);
            _encryptionProvider = GetProvider(cfg => cfg.Encryption);
            _serializationProvider = GetProvider(cfg => cfg.Serialization);
            _physicalAccessProvider = GetProvider(cfg => cfg.PhysicalAccess);
            _scriptingProvider = GetProvider(cfg => cfg.Scripting);
            _modelFactory = GetProvider(cfg => cfg.ModelFactory);
            IsInitialized = true;
        }

        public static bool IsProduction()
        {
            return GetConfigurationSection().Production;
        }

        public static IContextProvider Context
        {
            get
            {
                if (_contextProvider == null)
                    throw new Exception("Context provider is null. The context element is probably missing from lnf/providers in web.config.");

                return _contextProvider;
            }
        }

        public static IDataAccessProvider DataAccess { get { return _dataAccessProvider; } }
        public static ILogProvider Log { get { return _logProvider; } }
        public static IEmailProvider Email { get { return _emailProvider; } }
        public static IControlProvider Control { get { return _controlProvider; } }
        public static IEncryptionProvider Encryption { get { return _encryptionProvider; } }
        public static ISerializationProvider Serialization { get { return _serializationProvider; } }
        public static IPhysicalAccessProvider PhysicalAccess { get { return _physicalAccessProvider; } }
        public static IScriptingProvider Scripting { get { return _scriptingProvider; } }
        public static IModelFactory ModelFactory { get { return _modelFactory; } }

        public static string[] Hooks { get { return HookManager.GetHookTypes(); } }
        public static bool IsInitialized { get; }

        public static ProvidersSection GetConfigurationSection()
        {
            ProvidersSection result = ConfigurationManager.GetSection("lnf/providers") as ProvidersSection;

            if (result == null)
                throw new InvalidOperationException("The configuration section 'lnf/providers' is missing.");

            return result;
        }

        private static IContextProvider GetContextProvider()
        {
            // this is optional now

            ContextProviderTypeElement element = GetConfigurationSection().Context;

            if (element == null)
                return null;

            IContextProvider result = element.CreateInstance();

            if (result != null)
                result.LoginUrl = element.LoginUrl;

            return result;
        }

        private static IDataAccessProvider GetDataAccessProvider()
        {
            // this is optional now
            DataAccessTypeElement element = GetConfigurationSection().DataAccess;

            if (element == null)
                return null;

            IDataAccessProvider result = element.CreateInstance();

            return result;
        }

        private static ILogProvider GetLogProvider()
        {
            ILogProvider result = null;

            LogProviderTypeElement element = GetConfigurationSection().Log;

            if (element != null)
            {
                result = element.CreateInstance();
                if (result != null)
                {
                    result.Name = element.Name;
                    result.Enabled = element.Enabled;
                }
            }

            return result;
        }

        //does not use IServiceTypeProvider because host is a string
        private static IEmailProvider GetEmailProvider()
        {
            IEmailProvider result = null;

            EmailProviderTypeElement element = GetConfigurationSection().Email;

            if (element != null)
            {
                result = element.CreateInstance();
                if (result != null)
                {
                    result.Host = element.Host;
                    result.Port = element.Port;
                    result.Username = element.Username;
                    result.Password = element.Password;
                    result.EnableSsl = element.EnableSsl;
                    result.Log = element.Log;
                }
            }

            return result;
        }

        private static T GetServiceProvider<T>(Func<ProvidersSection, ServiceProviderTypeElement<T>> fn) where T : class, IServiceTypeProvider
        {
            T result = null;

            ServiceProviderTypeElement<T> element = fn(GetConfigurationSection());

            if (element != null)
            {
                result = element.CreateInstance();
                if (result != null)
                {
                    result.Host = element.Host;
                }
            }

            return result;
        }

        private static T GetProvider<T>(Func<ProvidersSection, ProviderTypeElement<T>> fn) where T : class, ITypeProvider
        {
            T result = null;

            ProviderTypeElement<T> element = fn(GetConfigurationSection());

            if (element != null)
            {
                result = element.CreateInstance();
            }

            return result;
        }

        //public static void Use<T>(T provider) where T : class, ITypeProvider
        //{
        //    string key = GetKey<T>();
        //    Context.Current.SetItem(key, provider);
        //}
    }

    public class ProvidersSection : ConfigurationSection
    {
        [ConfigurationProperty("production", IsRequired = true)]
        public bool Production
        {
            get { return Convert.ToBoolean(this["production"]); }
            set { this["production"] = value; }
        }

        [ConfigurationProperty("context")]
        public ContextProviderTypeElement Context
        {
            get { return this["context"] as ContextProviderTypeElement; }
            set { this["context"] = value; }
        }

        [ConfigurationProperty("dataAccess")]
        public DataAccessTypeElement DataAccess
        {
            get { return this["dataAccess"] as DataAccessTypeElement; }
            set { this["dataAccess"] = value; }
        }

        [ConfigurationProperty("control")]
        public ServiceProviderTypeElement<IControlProvider> Control
        {
            get { return this["control"] as ServiceProviderTypeElement<IControlProvider>; }
            set { this["control"] = value; }
        }

        [ConfigurationProperty("email")]
        public EmailProviderTypeElement Email
        {
            get { return this["email"] as EmailProviderTypeElement; }
            set { this["email"] = value; }
        }

        [ConfigurationProperty("log")]
        public LogProviderTypeElement Log
        {
            get { return this["log"] as LogProviderTypeElement; }
            set { this["log"] = value; }
        }

        [ConfigurationProperty("serialization")]
        public ProviderTypeElement<ISerializationProvider> Serialization
        {
            get { return this["serialization"] as ProviderTypeElement<ISerializationProvider>; }
            set { this["serialization"] = value; }
        }

        [ConfigurationProperty("scripting")]
        public ProviderTypeElement<IScriptingProvider> Scripting
        {
            get { return this["scripting"] as ProviderTypeElement<IScriptingProvider>; }
            set { this["scripting"] = value; }
        }

        [ConfigurationProperty("modelFactory")]
        public ProviderTypeElement<IModelFactory> ModelFactory
        {
            get { return this["modelFactory"] as ProviderTypeElement<IModelFactory>; }
            set { this["modelFactory"] = value; }
        }

        [ConfigurationProperty("physicalAccess")]
        public ProviderTypeElement<IPhysicalAccessProvider> PhysicalAccess
        {
            get { return this["physicalAccess"] as ProviderTypeElement<IPhysicalAccessProvider>; }
            set { this["physicalAccess"] = value; }
        }

        [ConfigurationProperty("encryption")]
        public ProviderTypeElement<IEncryptionProvider> Encryption
        {
            get { return this["encryption"] as ProviderTypeElement<IEncryptionProvider>; }
            set { this["encryption"] = value; }
        }
    }

    public class ProviderTypeElement<T> : ConfigurationElement
    {
        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("type", IsRequired = true)]
        public Type Type
        {
            get { return this["type"] as Type; }
            set { this["type"] = value; }
        }

        public virtual T CreateInstance()
        {
            if (Type == null)
                return default(T);

            T result = (T)Activator.CreateInstance(Type);

            return result;
        }

        protected TProperty GetProperty<TProperty>(string propertyName, TProperty defval)
        {
            if (this[propertyName] == null)
                return defval;
            else
            {
                TProperty result = (TProperty)this[propertyName];
                if (result == null)
                    return defval;
                else
                    return result;
            }
        }
    }

    public class ContextProviderTypeElement : ProviderTypeElement<IContextProvider>
    {
        [ConfigurationProperty("loginUrl", IsRequired = true)]
        public string LoginUrl
        {
            get { return this["loginUrl"].ToString(); }
            set { this["loginUrl"] = value; }
        }
    }

    public class DataAccessTypeElement : ProviderTypeElement<IDataAccessProvider>
    {
        [ConfigurationProperty("showSql", IsRequired = false, DefaultValue = false)]
        public bool ShowSql
        {
            get { return Convert.ToBoolean(this["showSql"]); }
            set { this["showSql"] = value; }
        }

        [ConfigurationProperty("universalPassword", IsRequired = false, DefaultValue = "")]
        public string UniversalPassword
        {
            get { return Convert.ToString(this["universalPassword"]); }
            set { this["universalPassword"] = value; }
        }

        public override IDataAccessProvider CreateInstance()
        {
            var result = base.CreateInstance();
            result.ShowSql = ShowSql;
            return result;
        }
    }

    public class LogProviderTypeElement : ProviderTypeElement<ILogProvider>
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get { return Convert.ToBoolean(this["enabled"]); }
            set { this["enabled"] = value; }
        }
    }

    public class ServiceProviderTypeElement<T> : ProviderTypeElement<T> where T : class, IServiceTypeProvider
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public Uri Host
        {
            get { return new Uri(this["host"].ToString()); }
            set { this["host"] = value; }
        }
    }

    public class EmailProviderTypeElement : ProviderTypeElement<IEmailProvider>
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return this["host"].ToString(); }
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
}
