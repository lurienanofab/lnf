using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web;

namespace LNF.Impl
{
    public class SessionManager<T> : IDisposable where T : ICurrentSessionContext
    {
        private readonly object _locker = new object();
        private readonly ISessionFactory _sessionFactory;
        private readonly Guid _factoryId;

        static SessionManager()
        {
            Current = new SessionManager<T>();
        }

        public static SessionManager<T> Current { get; }

        public SessionManager()
        {
            lock (_locker)
            {
                var sw = Stopwatch.StartNew();

                SessionLog.AddLogMessage("### New session factory created at {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

                try
                {
                    SessionLog.AddLogMessage("RequestUri: {0}", (HttpContext.Current == null || HttpContext.Current.Request == null) ? "N/A" : HttpContext.Current.Request.Url.ToString());
                }
                catch (Exception ex)
                {
                    SessionLog.AddLogMessage("RequestUri: {0}", ex.Message);
                }

                var config = SessionConfiguration.GetConfiguration()
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

                bool printSessionLog = false;

                if (!Providers.IsProduction() && printSessionLog)
                {
                    foreach (string line in GetLogMessages())
                        Debug.WriteLine(line);
                }
            }
        }

        private void HandleMappings(MappingConfiguration cfg)
        {
            cfg.HbmMappings
                .AddFromAssembly(GetType().Assembly);

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

        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        public void OpenSession()
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory))
            {
                ISession session = _sessionFactory.OpenSession();
                CurrentSessionContext.Bind(session);
                var printSessionLog = false;
                if (!Providers.IsProduction() && printSessionLog)
                   Debug.WriteLine("Started new session!");
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

        public ISession Session
        {
            get
            {
                //OpenSession();
                return _sessionFactory.GetCurrentSession();
            }
        }

        public IEnumerable<string> GetLogMessages()
        {
            return SessionLog.GetLogMessages();
        }

        public Guid GetFactoryID()
        {
            return _factoryId;
        }

        public void Dispose()
        {
            if (_sessionFactory != null)
                _sessionFactory.Dispose();
        }
    }
}
