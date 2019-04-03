using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Hooks;
using LNF.Models;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Models.Mail;
using LNF.Repository;
using LNF.Scheduler;
using System;
using System.Configuration;

namespace LNF
{
    public interface IProvider
    {
        IContext Context { get; }
        IDataAccessService DataAccess { get; }
        ILogService Log { get; }
        IControlService Control { get; }
        IEncryptionService Encryption { get; }
        ISerializationService Serialization { get; }
        IScriptingService Scripting { get; }
        IDataService Data { get; }
        IBillingService Billing { get; }
        IMailApi Mail { get; }
        IPhysicalAccessService PhysicalAccess { get; }
        ISchedulerService Scheduler { get; }
        IWorkerService Worker { get; }

        IModelFactory ModelFactory { get; }

        IClientOrgManager ClientOrgManager { get; }
        IActiveLogManager ActiveLogManager { get; }
        IActiveDataItemManager ActiveDataItemManager { get; }
        ICostManager CostManager { get; }
        IDryBoxManager DryBoxManager { get; }

        IToolBillingManager ToolBillingManager { get; }
        IBillingTypeManager BillingTypeManager { get; }

        ISchedulerRepository SchedulerRepository { get; }
        IResourceManager ResourceManager { get; }
        IReservationManager ReservationManager { get; }
        IReservationInviteeManager ReservationInviteeManager { get; }
        IProcessInfoManager ProcessInfoManager { get; }
        IEmailManager EmailManager { get; }

        IReadRoomDataManager ReadRoomDataManager { get; }
        IReadToolDataManager ReadToolDataManager { get; }
        IReadStoreDataManager ReadStoreDataManager { get; }
        IReadMiscDataManager ReadMiscDataManager { get; }
        IAdministrativeHelper AdministrativeHelper { get; }
        string[] Hooks { get; }
        void BuildUp(object target);
        bool IsProduction();
    }

    public class ServiceProvider : IProvider
    {
        private IDependencyResolver _resolver;

        public IContext Context => Use<IContext>();
        public IDataAccessService DataAccess => Use<IDataAccessService>();
        public ILogService Log => Use<ILogService>();
        public IControlService Control => Use<IControlService>();
        public IEncryptionService Encryption => Use<IEncryptionService>();
        public ISerializationService Serialization => Use<ISerializationService>();
        public IScriptingService Scripting => Use<IScriptingService>();

        public IDataService Data => Use<IDataService>();
        public IBillingService Billing => Use<IBillingService>();
        public IMailApi Mail => Use<IMailApi>();
        public IPhysicalAccessService PhysicalAccess => Use<IPhysicalAccessService>();
        public ISchedulerService Scheduler => Use<ISchedulerService>();
        public IWorkerService Worker => Use<IWorkerService>();

        public IClientOrgManager ClientOrgManager => Use<IClientOrgManager>();
        public IActiveLogManager ActiveLogManager => Use<IActiveLogManager>();
        public IActiveDataItemManager ActiveDataItemManager => Use<IActiveDataItemManager>();
        public ICostManager CostManager => Use<ICostManager>();
        public IDryBoxManager DryBoxManager => Use<IDryBoxManager>();

        public IToolBillingManager ToolBillingManager => Use<IToolBillingManager>();
        public IBillingTypeManager BillingTypeManager => Use<IBillingTypeManager>();

        public ISchedulerRepository SchedulerRepository => Use<ISchedulerRepository>();
        public IResourceManager ResourceManager => Use<IResourceManager>();
        public IReservationManager ReservationManager => Use<IReservationManager>();
        public IReservationInviteeManager ReservationInviteeManager => Use<IReservationInviteeManager>();
        public IProcessInfoManager ProcessInfoManager => Use<IProcessInfoManager>();
        public IEmailManager EmailManager => Use<IEmailManager>();

        public IReadRoomDataManager ReadRoomDataManager => Use<IReadRoomDataManager>();
        public IReadToolDataManager ReadToolDataManager => Use<IReadToolDataManager>();
        public IReadStoreDataManager ReadStoreDataManager => Use<IReadStoreDataManager>();
        public IReadMiscDataManager ReadMiscDataManager => Use<IReadMiscDataManager>();
        public IAdministrativeHelper AdministrativeHelper => Use<IAdministrativeHelper>();

        public IModelFactory ModelFactory => Use<IModelFactory>();

        public ServiceProvider(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public static IProvider Current { get; set; }

        private T Use<T>() => _resolver.GetInstance<T>();

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
