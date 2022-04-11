using LNF.DataAccess;
using NHibernate.Context;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace LNF.Impl.DataAccess
{
    public class NHibernateDataAccess<T> : IDataAccessService where T : ICurrentSessionContext
    {   
        public NHibernateDataAccess(ISessionManager mgr)
        {
            Session = new NHibernateSession(mgr);
        }

        public IUnitOfWork StartUnitOfWork() => new NHibernateUnitOfWork(SessionManager<T>.Current);

        public ISession Session { get; }

        public virtual SqlConnection NewConnection()
        {
            if (ConfigurationManager.ConnectionStrings["cnSselData"] == null)
                throw new Exception("Missing connectionString: cnSselData");

            var result = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);

            return result;
        }

        public bool ShowSql => SessionManager<T>.Current.ShowSql;

        public string UniversalPassword => SessionManager<T>.Current.UniversalPassword;

        public bool IsProduction() => SessionManager<T>.Current.IsProduction();
    }
}
