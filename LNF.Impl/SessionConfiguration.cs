using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace LNF.Impl
{
    public static class SessionConfiguration
    {
        private static readonly object _locker = new object();
        private static FluentConfiguration _config;

        static SessionConfiguration()
        {
            lock (_locker)
            {
                MsSqlConfiguration mssql = MsSqlConfiguration.MsSql2012.ConnectionString(cs => cs.FromConnectionStringWithKey("cnSselData"));

                if (Providers.DataAccess.ShowSql)
                {
                    mssql.ShowSql();
                    SessionLog.AddLogMessage("ShowSql: true");
                }
                else
                {
                    SessionLog.AddLogMessage("ShowSql: false");
                }

                _config = Fluently.Configure().Database(mssql);
            }
        }

        public static FluentConfiguration GetConfiguration()
        {
            return _config;
        }
    }
}
