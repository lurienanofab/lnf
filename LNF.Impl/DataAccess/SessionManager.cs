﻿using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace LNF.Impl.DataAccess
{
    public interface ISessionManager : IDisposable
    {
        void OpenSession();
        void CloseSession();
        ISession Session { get; }
        ISessionFactory GetSessionFactory();
        bool ShowSql { get; }
        string UniversalPassword { get; }
        bool IsProduction();
    }

    public class SessionManager<T> : ISessionManager where T : ICurrentSessionContext
    {
        public static SessionManager<T> Current { get; }

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

            Current = new SessionManager<T>();
        }

        private SessionManager()
        {
            lock (_locker)
            {
                var sw = Stopwatch.StartNew();

                SessionLog.AddLogMessage("### New session factory created at {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

                _config = ServiceProvider.GetConfigurationSection();

                SessionLog.AddLogMessage("IsProduction: {0}", IsProduction());

                if (_config == null)
                    throw new InvalidOperationException("The configuration section 'lnf/settings' is missing.");

                try
                {
                    var requestUrl = HttpContext.Current.Request.Url.ToString();
                    SessionLog.AddLogMessage("RequestUri: {0}", requestUrl);
                }
                catch (Exception ex)
                {
                    SessionLog.AddLogMessage("RequestUri: {0}", ex.Message);
                }

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

                SessionLog.WriteAll(_config.Log.Name, !IsProduction());
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

        public bool ShowSql => _config.DataAccess.ShowSql;

        public string UniversalPassword => _config.DataAccess.UniversalPassword;

        public bool IsProduction() => _config.Production;

        public void Dispose()
        {
            if (_sessionFactory != null)
                _sessionFactory.Dispose();
        }
    }
}
