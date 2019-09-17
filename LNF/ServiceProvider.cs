using LNF.CommonTools;
using LNF.Hooks;
using LNF.Models;
using LNF.Models.Authorization;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Models.Mail;
using LNF.Models.PhysicalAccess;
using LNF.Models.Scheduler;
using LNF.Models.Worker;
using LNF.Repository;
using LNF.Scheduler;
using System;
using System.Configuration;

namespace LNF
{
    public interface IProvider : Models.IProvider
    {
        IContext Context { get; }
        IAuthorizationService Authorization { get; }
        IDataAccessService DataAccess { get; }
        ILogService Log { get; }
        IControlService Control { get; }
        IEncryptionService Encryption { get; }
        ISerializationService Serialization { get; }
        IScriptingService Scripting { get; }
        IModelFactory ModelFactory { get; }
        ISchedulerRepository SchedulerRepository { get; }
        IEmailManager EmailManager { get; }
        IReadRoomDataManager ReadRoomDataManager { get; }
        IReadToolDataManager ReadToolDataManager { get; }
        IReadStoreDataManager ReadStoreDataManager { get; }
        IReadMiscDataManager ReadMiscDataManager { get; }
        IAdministrativeHelper AdministrativeHelper { get; }
        void BuildUp(object target);
        string[] Hooks { get; }

        T Resolve<T>();
    }

    public class ServiceProvider : IProvider
    {
        private IDependencyResolver _resolver;

        public IContext Context => Resolve<IContext>();
        public IAuthorizationService Authorization => Resolve<IAuthorizationService>();
        public IDataAccessService DataAccess => Resolve<IDataAccessService>();
        public ILogService Log => Resolve<ILogService>();
        public IControlService Control => Resolve<IControlService>();
        public IEncryptionService Encryption => Resolve<IEncryptionService>();
        public ISerializationService Serialization => Resolve<ISerializationService>();
        public IScriptingService Scripting => Resolve<IScriptingService>();

        public IDataService Data => Resolve<IDataService>();
        public IBillingService Billing => Resolve<IBillingService>();
        public IMailService Mail => Resolve<IMailService>();
        public IPhysicalAccessService PhysicalAccess => Resolve<IPhysicalAccessService>();
        public ISchedulerService Scheduler => Resolve<ISchedulerService>();
        public IWorkerService Worker => Resolve<IWorkerService>();

        public ISchedulerRepository SchedulerRepository => Resolve<ISchedulerRepository>();
        public IReservationManager ReservationManager => Resolve<IReservationManager>();
        public IEmailManager EmailManager => Resolve<IEmailManager>();

        public IReadRoomDataManager ReadRoomDataManager => Resolve<IReadRoomDataManager>();
        public IReadToolDataManager ReadToolDataManager => Resolve<IReadToolDataManager>();
        public IReadStoreDataManager ReadStoreDataManager => Resolve<IReadStoreDataManager>();
        public IReadMiscDataManager ReadMiscDataManager => Resolve<IReadMiscDataManager>();
        public IAdministrativeHelper AdministrativeHelper => Resolve<IAdministrativeHelper>();

        public IModelFactory ModelFactory => Resolve<IModelFactory>();

        public ServiceProvider(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public static IProvider Current { get; private set; }

        public static void Configure(IDependencyResolver resolver)
        {
            Current = resolver.GetInstance<IProvider>();
        }

        public T Resolve<T>() => _resolver.GetInstance<T>();

        public void BuildUp(object target) => _resolver.BuildUp(target);

        public bool IsProduction() => GetConfigurationSection().Production;

        public string[] Hooks => HookManager.GetHookTypes();

        public static ServiceProviderSection GetConfigurationSection()
        {
            if (!(ConfigurationManager.GetSection("lnf/provider") is ServiceProviderSection result))
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
                throw new InvalidOperationException($"The attribute '{propertyName}' is required. [{GetType().FullName}]");

            var result = GetProperty(propertyName, defval);

            if (result.Equals(defval))
                throw new InvalidOperationException($"The attribute '{propertyName}' is required. [{GetType().FullName}]");

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
