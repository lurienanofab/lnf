using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web;

namespace LNF.Impl.DataAccess
{
    public interface ISessionManager : IDisposable
    {
        void OpenSession();
        void CloseSession();
        ISession Session { get; }
        ISessionFactory GetSessionFactory();
        IEnumerable<string> GetLogMessages();
    }

    public class SessionManager<T> : ISessionManager where T : ICurrentSessionContext
    {
        private readonly ServiceProviderSection _config;
        private readonly object _locker = new object();
        private readonly ISessionFactory _sessionFactory;
        private readonly Guid _factoryId;

        static SessionManager()
        {
            // this forces the dlls to be copied.
            // see https://stackoverflow.com/questions/15816769/dependent-dll-is-not-getting-copied-to-the-build-output-folder-in-visual-studio
            Remotion.Linq.QueryModel q = null;
            if (q != null) throw new Exception();
            Remotion.Linq.EagerFetching.FetchManyRequest x = null;
            if (x != null) throw new Exception();
        }

        public SessionManager()
        {
            lock (_locker)
            {
                var sw = Stopwatch.StartNew();

                SessionLog.AddLogMessage("### New session factory created at {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

                _config = ServiceProvider.GetConfigurationSection();

                if (_config == null)
                    throw new InvalidOperationException("The configuration section 'lnf/settings' is missing.");

                try
                {
                    SessionLog.AddLogMessage("RequestUri: {0}", (HttpContext.Current == null || HttpContext.Current.Request == null) ? "N/A" : HttpContext.Current.Request.Url.ToString());
                }
                catch (Exception ex)
                {
                    SessionLog.AddLogMessage("RequestUri: {0}", ex.Message);
                }

                MsSqlConfiguration mssql = MsSqlConfiguration.MsSql2012.ConnectionString(cs => cs.FromConnectionStringWithKey("cnSselData"));

                if (ShowSql())
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

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["NHibernate.Glimpse.Loggers"]))
                {
                    config.ExposeConfiguration(cfg =>
                    {
                        cfg.SetProperty("format_sql", "true");
                        cfg.SetProperty("generate_statistics", "true");
                    });
                }

                _sessionFactory = config.BuildSessionFactory();
                _factoryId = Guid.NewGuid();
                sw.Stop();

                SessionLog.AddLogMessage("FactoryID: {0}", _factoryId);
                SessionLog.AddLogMessage("### New session factory completed at {0:yyyy-MM-dd HH:mm:ss} ({1:#0.0000} seconds)", DateTime.Now, sw.Elapsed.TotalMilliseconds / 1000.0);

                if (!IsProduction())
                {
                    foreach (string line in GetLogMessages())
                        Debug.WriteLine(line);
                }
            }
        }

        private void HandleMappings(MappingConfiguration cfg)
        {
            cfg.HbmMappings.AddFromAssemblyOf<ISessionManager>();

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
                ISession session = _sessionFactory.OpenSession();
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
                ISession session = CurrentSessionContext.Unbind(_sessionFactory);
                session.Close();
                session.Dispose();
            }
        }

        public ISession Session => _sessionFactory.GetCurrentSession();

        public ISessionFactory GetSessionFactory() => _sessionFactory;

        public IEnumerable<string> GetLogMessages()
        {
            return SessionLog.GetLogMessages();
        }

        public bool ShowSql()
        {
            return _config.DataAccess.ShowSql;
        }

        public bool IsProduction()
        {
            return _config.Production;
        }

        public void Dispose()
        {
            if (_sessionFactory != null)
                _sessionFactory.Dispose();
        }
    }
}
