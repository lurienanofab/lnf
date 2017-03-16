using LNF.Repository;
using NHibernate;
using NHibernate.Context;
using NHibernate.Linq;
using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl
{
    public class NHibernateRepository<TContext> : IRepository where TContext : ICurrentSessionContext
    {
        internal NHibernateRepository() { }

        protected ISession Session
        {
            get
            {
                return SessionManager<TContext>.Current.Session;
            }
        }

        public int GetTransactionHashCode()
        {
            return SessionManager<TContext>.Current.Session.Transaction.GetHashCode();
        }

        public void Insert(IEnumerable<IDataItem> items)
        {
            foreach (IDataItem i in items)
                Session.Save(i);
        }

        public void Delete(IEnumerable<IDataItem> items)
        {
            foreach (IDataItem i in items)
                Session.Delete(i);
        }

        public void SaveOrUpdate(IDataItem item)
        {
            Session.SaveOrUpdate(item);
        }

        public T Single<T>(object id) where T : IDataItem
        {
            return Session.Get<T>(id);
        }

        public IQueryable<T> Query<T>() where T : IDataItem
        {
            return Session.Query<T>();
        }

        public IQueryable<T> Cache<T>(Repository.CacheMode mode = Repository.CacheMode.Normal) where T : IDataItem
        {
            NHibernate.CacheMode cm = (NHibernate.CacheMode)mode;
            return Session.Query<T>().Cacheable().CacheMode(cm);
        }

        public int Count<T>() where T : IDataItem
        {
            return Query<T>().Count();
        }

        public int Count<T>(Func<T, bool> filter) where T : IDataItem
        {
            return Query<T>().Count(filter);
        }

        public bool Exists<T>(Func<T, bool> filter) where T : IDataItem
        {
            return Query<T>().Any(filter);
        }

        public T Merge<T>(T item) where T : class, IDataItem
        {
            return Session.Merge(item);
        }

        public QueryBuilder QueryBuilder()
        {
            return new NHibernateQueryBuilder(Session);
        }

        public UnitOfWorkAdapter GetAdapter()
        {
            return new NHibernateUnitOfWorkAdapter(Session);
        }

        public IBulkCopy GetBulkCopy(string destinationTableName)
        {
            return new NHibernateBulkCopy(Session, destinationTableName);
        }

        public void Flush()
        {
            Session.Flush();
        }

        public void Evict(IDataItem item)
        {
            Session.Evict(item);
        }

        public T SqlResult<T>(string sql, object parameters)
        {
            var query = Session.CreateSQLQuery(sql);
            foreach (var prop in parameters.GetType().GetProperties())
                query.SetParameter(prop.Name, prop.GetValue(parameters), NHibernateUtil.GuessType(prop.PropertyType));
            T result = query.UniqueResult<T>();
            return result;
        }

        public IList<T> SqlQuery<T>(string sql, object parameters) where T : IDataItem
        {
            var query = Session.CreateSQLQuery(sql);
            foreach (var prop in parameters.GetType().GetProperties())
                query.SetParameter(prop.Name, prop.GetValue(parameters), NHibernateUtil.GuessType(prop.PropertyType));
            IList<T> result = query.SetResultTransformer(Transformers.AliasToBean<T>()).List<T>();
            return result;
        }

        private ICriteria CreateCriteria<T>() where T : class
        {
            return Session.CreateCriteria<T>().SetCacheable(true);
        }

        //http://stackoverflow.com/questions/5229510/nhibernate-get-concrete-type-of-referenced-abstract-entity#5333880
        public object Unproxy(IDataItem proxy)
        {
            if (!NHibernateUtil.IsInitialized(proxy))
            {
                NHibernateUtil.Initialize(proxy);
            }

            if (proxy is NHibernate.Proxy.INHibernateProxy)
            {
                return Session.GetSessionImplementation().PersistenceContext.Unproxy(proxy);
            }

            return proxy;
        }
    }

    public class NHibernateSqlQuery : SqlQuery
    {
        IQuery query;

        internal NHibernateSqlQuery(IQuery query)
        {
            this.query = query;
        }

        public override IList<T> List<T>()
        {
            return query.SetResultTransformer(Transformers.AliasToBean<T>()).List<T>();
        }

        public override IList<IDictionary> List()
        {
            return query.SetResultTransformer(Transformers.AliasToEntityMap).List<IDictionary>();
        }

        public override T Result<T>()
        {
            return query.UniqueResult<T>();
        }

        public override int Update()
        {
            return query.ExecuteUpdate();
        }
    }

    public class NHibernateNamedQuery : NamedQuery
    {
        private IQuery query;

        internal NHibernateNamedQuery(IQuery query)
        {
            this.query = query;
        }

        public override IList<T> List<T>()
        {
            return query.List<T>();
        }

        public override T Result<T>()
        {
            return query.UniqueResult<T>();
        }

        public override int Update()
        {
            return query.ExecuteUpdate();
        }
    }

    public class NHibernateQueryBuilder : QueryBuilder
    {
        private ISession session;
        private ICriteria criteria;

        internal NHibernateQueryBuilder(ISession session)
        {
            this.session = session;
        }

        private ICriteria GetCriteria<T>() where T : class
        {
            if (criteria == null)
                criteria = session.CreateCriteria<T>().SetCacheable(true);
            return criteria;
        }

        public override NamedQuery NamedQuery(string name)
        {
            IQuery query = session.GetNamedQuery(name);
            query.ApplyParameters(queryParams);
            return new NHibernateNamedQuery(query);
        }

        public override SqlQuery SqlQuery(string sql)
        {
            IQuery query = session.CreateSQLQuery(sql);
            query.ApplyParameters(queryParams);
            return new NHibernateSqlQuery(query);
        }
    }

    public class NHibernateBulkCopy : IBulkCopy, IDisposable
    {
        private SqlBulkCopy _bcp;

        public string DestinationTableName
        {
            get { return _bcp.DestinationTableName; }
        }

        public NHibernateBulkCopy(ISession session, string destinationTableName)
        {
            SqlConnection conn = session.Connection as SqlConnection;

            if (conn == null)
                throw new InvalidCastException("Only SqlConnection type is supported.");

            var cmd = conn.CreateCommand();

            session.Transaction.Enlist(cmd);

            _bcp = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, cmd.Transaction);
            _bcp.DestinationTableName = destinationTableName;
        }

        public void AddColumnMapping(string sourceColumn, string destinationColumn)
        {
            _bcp.ColumnMappings.Add(sourceColumn, destinationColumn);
        }

        public void AddColumnMapping(string columnName)
        {
            AddColumnMapping(columnName, columnName);
        }

        public void WriteToServer(DataTable dt)
        {
            _bcp.WriteToServer(dt);
        }

        public void Dispose()
        {
            IDisposable disposable = _bcp;
            disposable.Dispose();
        }
    }
}
