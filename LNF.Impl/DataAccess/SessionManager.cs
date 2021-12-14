using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using LNF.CommonTools;
using LNF.DataAccess;
using LNF.Impl.DataAccess.ModelFactory;
using NHibernate;
using NHibernate.Context;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace LNF.Impl.DataAccess
{
    public interface ISessionManager : IDisposable
    {
        void OpenSession();
        void CloseSession();
        NHibernate.ISession Session { get; }
        ISessionFactory GetSessionFactory();
        bool ShowSql { get; }
        string UniversalPassword { get; }
        bool IsProduction();
    }

    public class ThreadStaticSession : SessionBase
    {
        public static SessionBase Current { get; }

        static ThreadStaticSession()
        {
            Current = new ThreadStaticSession();
        }

        private ThreadStaticSession() { }

        public override ISessionManager SessionManager => SessionManager<ThreadStaticSessionContext>.Current;
        protected override IDataAccessService NewDataAccessService() => new NHibernateDataAccess<ThreadStaticSessionContext>(SessionManager);
    }

    public class WebSession : SessionBase
    {
        public static SessionBase Current { get; }

        static WebSession()
        {
            Current = new WebSession();
        }

        private WebSession() { }

        public override ISessionManager SessionManager => SessionManager<WebSessionContext>.Current;
        protected override IDataAccessService NewDataAccessService() => new NHibernateDataAccess<WebSessionContext>(SessionManager);
    }

    public abstract class SessionBase
    {
        private IDataAccessService _service;

        public abstract ISessionManager SessionManager { get; }

        public IDataAccessService GetDataAccessService()
        {
            if (_service == null)
                _service = NewDataAccessService(); //new NHibernateDataAccess<WebSessionContext>(SessionManager);
            return _service;
        }

        protected abstract IDataAccessService NewDataAccessService();

        public LNF.DataAccess.ISession GetSession() => GetDataAccessService().Session;
        public NHibernate.ISession GetNHibernateSession() => SessionManager.Session;

        public IUnitOfWork StartUnitOfWork() => new NHibernateUnitOfWork(SessionManager);
    }

    public class SessionManager<T> : ISessionManager where T : ICurrentSessionContext
    {
        public static ISessionManager Current { get; }

        //private readonly ServiceProviderSection _config;
        private readonly object _locker = new object();
        private readonly ISessionFactory _sessionFactory;
        private readonly Guid _factoryId;

        static SessionManager()
        {
            try
            {
                // this forces the dlls to be copied.
                // see https://stackoverflow.com/questions/15816769/dependent-dll-is-not-getting-copied-to-the-build-output-folder-in-visual-studio
                Remotion.Linq.QueryModel q = null;
                if (q != null) throw new Exception("q is not null");
                Remotion.Linq.EagerFetching.FetchManyRequest x = null;
                if (x != null) throw new Exception("x is not null");

                Current = new SessionManager<T>();

                ModelFactoryProvider.Setup(new ValueInjecterModelFactory(Current));
            }
            catch (Exception ex)
            {
                var securePath = Utility.GetRequiredAppSetting("SecurePath");
                if (Directory.Exists(securePath))
                {
                    var logsPath = Path.Combine(securePath, "logs");

                    if (!Directory.Exists(logsPath))
                        Directory.CreateDirectory(logsPath);

                    var filePath = Path.Combine(logsPath, "SessionManagerError.log");

                    using (var writer = File.AppendText(filePath))
                    {
                        writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                        writer.WriteLine(ex.ToString());
                    }
                }

                throw ex;
            }
        }

        protected SessionManager()
        {
            lock (_locker)
            {
                string logName = Configuration.Current.Log.Name;

                var sw = Stopwatch.StartNew();

                SessionLog.AddLogMessage("### New session factory created at {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

                SessionLog.AddLogMessage("IsProduction: {0}", IsProduction());

                MsSqlConfiguration mssql = MsSqlConfiguration.MsSql2012.ConnectionString(cs => cs.FromConnectionStringWithKey("cnSselData"));

                if (ShowSql)
                {
                    mssql.ShowSql();
                    SessionLog.AddLogMessage("ShowSql: true");
                }
                else
                {
                    SessionLog.AddLogMessage("ShowSql: false");
                }

                var config = Fluently.Configure()
                    .Database(mssql)
                    .Mappings(HandleMappings)
                    .CurrentSessionContext<T>(); //this determines what CurrentSessionContext will be

                _sessionFactory = config.BuildSessionFactory();
                _factoryId = Guid.NewGuid();
                sw.Stop();

                SessionLog.AddLogMessage("FactoryID: {0}", _factoryId);
                SessionLog.AddLogMessage("### New session factory completed at {0:yyyy-MM-dd HH:mm:ss} ({1:#0.0000} seconds)", DateTime.Now, sw.Elapsed.TotalMilliseconds / 1000.0);

                SessionLog.WriteAll(logName, !IsProduction());
            }
        }

        private void HandleMappings(MappingConfiguration cfg)
        {
            cfg.HbmMappings.AddFromAssemblyOf<SessionManager<T>>();

            cfg.FluentMappings
                .AddFromAssembly(GetType().Assembly)
                .Conventions.AddAssembly(GetType().Assembly)
                .Conventions.Add(
                    AutoImport.Always(),
                    ForeignKey.EndsWith("ID"),
                    LazyLoad.Always(),
                    DefaultCascade.None(),
                    DynamicInsert.AlwaysTrue(),
                    DynamicUpdate.AlwaysTrue());
        }

        public void OpenSession()
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory))
            {
                NHibernate.ISession session = _sessionFactory.OpenSession();
                CurrentSessionContext.Bind(session);

                if (!IsProduction())
                {
                    Debug.WriteLine($"Started new session!");
                }
            }
        }

        public void CloseSession()
        {
            if (CurrentSessionContext.HasBind(_sessionFactory))
            {
                NHibernate.ISession session = CurrentSessionContext.Unbind(_sessionFactory);
                session.Close();
                session.Dispose();
            }
        }

        public NHibernate.ISession Session => _sessionFactory.GetCurrentSession();

        public ISessionFactory GetSessionFactory() => _sessionFactory;

        public bool ShowSql => Configuration.Current.DataAccess.ShowSql;

        public string UniversalPassword => Configuration.Current.DataAccess.UniversalPassword;

        public bool IsProduction() => Configuration.Current.Production;

        public void Dispose()
        {
            if (_sessionFactory != null)
                _sessionFactory.Dispose();
        }
    }
}
