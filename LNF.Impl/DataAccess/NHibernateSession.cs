using LNF.Repository;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Impl.DataAccess
{
    public class NHibernateSession : Repository.ISession
    {
        public ISessionManager SessionManager { get; }

        public NHibernateSession(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        protected NHibernate.ISession Session => SessionManager.Session;

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

            return Session.Query<T>().WithOptions(opts =>
            {
                opts.SetCacheable(true);
                opts.SetCacheMode(cm);
            });
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

        public void Evict<T>() where T : IDataItem
        {
            Session.SessionFactory.Evict(typeof(T));
        }

        public void Refresh(IDataItem item)
        {
            Session.Refresh(item);
        }

        public INamedQuery NamedQuery(string name)
        {
            var query = Session.GetNamedQuery(name);
            return new NHibernateNamedQuery(query);
        }

        public ISqlQuery SqlQuery(string sql)
        {
            var query = Session.CreateSQLQuery(sql);
            return new NHibernateSqlQuery(query);
        }

        public IQueryBuilder<T> QueryBuilder<T>() where T : class, IDataItem
        {
            var queryOver = Session.QueryOver<T>();
            var builder = new NHibernateQueryBuilder<T>(queryOver);
            return builder;
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

    public static class NHibernateRepositoryUtil
    {
        public static void ApplyParameters(IQuery query, object parameters)
        {
            var dict = ObjectToDictionary(parameters);
            ApplyParameters(query, dict);
        }

        public static void ApplyParameters(IQuery query, IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    var name = kvp.Key;
                    var value = kvp.Value;

                    if (value is string)
                        query.SetParameter(name, value);
                    else if (value is IEnumerable)
                        query.SetParameterList(name, (IEnumerable)value);
                    else
                        query.SetParameter(name, value);
                }
            }
        }

        public static void ApplyParameter(IQuery query, string name, object value)
        {
            query.SetParameter(name, value);
        }

        public static void ApplyParameterList(IQuery query, string name, IEnumerable values)
        {
            query.SetParameterList(name, values);
        }

        public static IDictionary<string, object> ObjectToDictionary(object parameters)
        {
            if (parameters == null)
                return null;

            var result = parameters.GetType()
                .GetProperties()
                .ToDictionary(k => k.Name, v => v.GetValue(parameters));

            return result;
        }
    }

    public class NHibernateSqlQuery : ISqlQuery
    {
        IQuery query;

        internal NHibernateSqlQuery(IQuery query)
        {
            this.query = query;
        }

        public IList<T> List<T>()
        {
            return query.SetResultTransformer(Transformers.AliasToBean<T>()).List<T>();
        }

        public IList<IDictionary> List()
        {
            return query.SetResultTransformer(Transformers.AliasToEntityMap).List<IDictionary>();
        }

        public T Result<T>() where T : struct
        {
            return query.UniqueResult<T>();
        }

        public int Update()
        {
            return query.ExecuteUpdate();
        }

        public ISqlQuery SetParameters(object parameters)
        {
            NHibernateRepositoryUtil.ApplyParameters(query, parameters);
            return this;
        }

        public ISqlQuery SetParameters(IDictionary<string, object> parameters)
        {
            NHibernateRepositoryUtil.ApplyParameters(query, parameters);
            return this;
        }

        public ISqlQuery SetParameter(string name, object value)
        {
            NHibernateRepositoryUtil.ApplyParameter(query, name, value);
            return this;
        }

        public ISqlQuery SetParameterList(string name, IEnumerable values)
        {
            NHibernateRepositoryUtil.ApplyParameterList(query, name, values);
            return this;
        }
    }

    public class NHibernateNamedQuery : INamedQuery
    {
        private IQuery query;

        internal NHibernateNamedQuery(IQuery query)
        {
            this.query = query;
        }

        public IList<T> List<T>() where T : IDataItem
        {
            return query.List<T>();
        }

        public T Result<T>() where T : struct
        {
            return query.UniqueResult<T>();
        }

        public int Update()
        {
            return query.ExecuteUpdate();
        }

        public INamedQuery SetParameters(object parameters)
        {
            NHibernateRepositoryUtil.ApplyParameters(query, parameters);
            return this;
        }

        public INamedQuery SetParameters(IDictionary<string, object> parameters)
        {
            NHibernateRepositoryUtil.ApplyParameters(query, parameters);
            return this;
        }

        public INamedQuery SetParameter(string name, object value)
        {
            NHibernateRepositoryUtil.ApplyParameter(query, name, value);
            return this;
        }

        public INamedQuery SetParameterList(string name, IEnumerable values)
        {
            NHibernateRepositoryUtil.ApplyParameterList(query, name, values);
            return this;
        }
    }

    public class NHibernateQueryBuilder<T> : IQueryBuilder<T> where T : class, IDataItem
    {
        private IQueryOver<T, T> _query;

        public NHibernateQueryBuilder(IQueryOver<T, T> query)
        {
            _query = query;
        }

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            _query.Where(expression);
            return this;
        }

        public IQueryBuilder<T> Where(IConjunction<T> conjunction)
        {
            var conj = QueryBuilderUtil.GetConjunction(conjunction);
            _query.Where(conj);
            return this;
        }

        public IQueryBuilder<T> Where(IDisjunction<T> disjunction)
        {
            var disj = QueryBuilderUtil.GetDisjunction(disjunction);
            _query.Where(disj);
            return this;
        }

        public IQueryBuilder<T> Where(ICriterion<T> criterion)
        {
            var crit = QueryBuilderUtil.GetCriterion(criterion);
            _query.Where(crit);
            return this;
        }

        public IQueryBuilder<T> And(Expression<Func<T, bool>> expression)
        {
            _query.And(expression);
            return this;
        }

        public IQueryBuilder<T> And(IConjunction<T> conjunction)
        {
            var conj = QueryBuilderUtil.GetConjunction(conjunction);
            _query.And(conj);
            return this;
        }

        public IQueryBuilder<T> And(IDisjunction<T> disjunction)
        {
            var disj = QueryBuilderUtil.GetDisjunction(disjunction);
            _query.And(disj);
            return this;
        }

        public IQueryBuilder<T> And(ICriterion<T> criterion)
        {
            var crit = QueryBuilderUtil.GetCriterion(criterion);
            _query.And(crit);
            return this;
        }

        public IQueryBuilder<T> OrderBy(Expression<Func<T, object>> expression)
        {
            _query.OrderBy(expression).Asc();
            return this;
        }

        public IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            _query.OrderBy(expression).Desc();
            return this;
        }

        public IQueryBuilder<T> ThenBy(Expression<Func<T, object>> expression)
        {
            _query.ThenBy(expression).Asc();
            return this;
        }

        public IQueryBuilder<T> ThenByDescending(Expression<Func<T, object>> expression)
        {
            _query.ThenBy(expression).Desc();
            return this;
        }

        public IQueryBuilder<T> Skip(int value)
        {
            _query.Skip(value);
            return this;
        }

        public IQueryBuilder<T> Take(int value)
        {
            _query.Take(value);
            return this;
        }

        public IConjunction<T> Conjunction()
        {
            return new NHibernateConjunction<T>(Restrictions.Conjunction());
        }

        public IDisjunction<T> Disjunction()
        {
            return new NHibernateDisjunction<T>(Restrictions.Disjunction());
        }

        public IRestriction<T, TValue> Restriction<TValue>(Expression<Func<T, TValue>> expression)
        {
            return new NHibernateRestriction<T, TValue>(expression);
        }

        public IList<T> List()
        {
            return _query.List();
        }

        public TResult Result<TResult>() where TResult : struct
        {
            return _query.SingleOrDefault<TResult>();
        }

        public int Count()
        {
            return _query.RowCount();
        }
    }

    public class NHibernateConjunction<T> : IConjunction<T> where T : class, IDataItem
    {
        internal Conjunction Conjunction { get; }

        internal NHibernateConjunction(Conjunction conj)
        {
            Conjunction = conj;
        }

        public IConjunction<T> Add(Expression<Func<T, bool>> expression)
        {
            Conjunction.Add(expression);
            return this;
        }

        public IConjunction<T> Add(ICriterion<T> criterion)
        {
            var crit = QueryBuilderUtil.GetCriterion(criterion);
            Conjunction.Add(crit);
            return this;
        }
    }

    public class NHibernateDisjunction<T> : IDisjunction<T> where T : class, IDataItem
    {
        internal Disjunction Disjunction { get; }

        internal NHibernateDisjunction(Disjunction disj)
        {
            Disjunction = disj;
        }

        public IDisjunction<T> Add(Expression<Func<T, bool>> expression)
        {
            Disjunction.Add(expression);
            return this;
        }

        public IDisjunction<T> Add(ICriterion<T> criterion)
        {
            var crit = QueryBuilderUtil.GetCriterion(criterion);
            Disjunction.Add(crit);
            return this;
        }
    }

    public class NHibernateRestriction<T, TValue> : IRestriction<T, TValue> where T : class, IDataItem
    {
        private Expression<Func<T, TValue>> _expression;

        internal NHibernateRestriction(Expression<Func<T, TValue>> expression)
        {
            _expression = expression;
        }

        public ICriterion<T> In(IEnumerable<TValue> values)
        {
            var exp = QueryBuilderUtil.ConvertExpression(_expression);
            return new NHibernateCriterion<T>(Restrictions.On(exp).IsIn(values.ToArray()));
        }

        public ICriterion<T> Like(string value)
        {
            var propertyName = NHibernate.Impl.ExpressionProcessor.FindMemberExpression(_expression.Body);

            string sql = string.Empty;

            if (typeof(TValue) == typeof(string))
                sql = $"{{alias}}.{propertyName} as LikeCompare";
            else
                sql = $"convert(nvarchar(max), {{alias}}.{propertyName}) as LikeCompare";


            var columnAliases = new[] { "LikeCompare" };
            var types = new NHibernate.Type.IType[] { NHibernateUtil.String };
            var proj = Projections.SqlProjection(sql, columnAliases, types);
            return new NHibernateCriterion<T>(Restrictions.Like(proj, value));
        }

        public ICriterion<T> StartsWith(string value)
        {
            return Like($"{value}%");
        }

        public ICriterion<T> EndsWith(string value)
        {
            return Like($"%{value}");
        }

        public ICriterion<T> Contains(string value)
        {
            return Like($"%{value}%");
        }

        public ICriterion<T> HasAny(TValue value)
        {
            var exp = QueryBuilderUtil.ConvertExpression(_expression);
            return new NHibernateCriterion<T>(BitwiseExpression.On(exp).HasAny(value));
        }

        public ICriterion<T> HasBit(TValue value)
        {
            var exp = QueryBuilderUtil.ConvertExpression(_expression);
            return new NHibernateCriterion<T>(BitwiseExpression.On(exp).HasBit(value));
        }
    }

    public class NHibernateCriterion<T> : ICriterion<T> where T : class, IDataItem
    {
        internal ICriterion Criterion { get; }

        internal NHibernateCriterion(ICriterion criterion)
        {
            Criterion = criterion;
        }
    }

    public static class QueryBuilderUtil
    {
        public static Conjunction GetConjunction<T>(IConjunction<T> conjunction) where T : class, IDataItem
        {
            if (conjunction == null)
                throw new ArgumentNullException("conjunction");

            if (conjunction is NHibernateConjunction<T>)
            {

                if (conjunction is NHibernateConjunction<T> conj)
                    return conj.Conjunction;

                throw new InvalidCastException("Unable to cast conjunction to NHibernateConjunction<T>.");
            }

            throw new ArgumentException("Argument must be an instance of NHibernateConjunction<T>", "conjunction");
        }

        public static Disjunction GetDisjunction<T>(IDisjunction<T> disjunction) where T : class, IDataItem
        {
            if (disjunction == null)
                throw new ArgumentNullException("disjunction");

            if (disjunction is NHibernateDisjunction<T>)
            {
                if (disjunction is NHibernateDisjunction<T> disj)
                    return disj.Disjunction;

                throw new InvalidCastException("Unable to cast disjunction to NHibernateDisjunction<T>.");
            }

            throw new ArgumentException("Argument must be an instance of NHibernateDisjunction<T>", "disjunction");
        }

        public static ICriterion GetCriterion<T>(ICriterion<T> criterion) where T : class, IDataItem
        {
            if (criterion == null)
                throw new ArgumentNullException("criterion");

            if (criterion is NHibernateCriterion<T>)
            {
                if (criterion is NHibernateCriterion<T> crit)
                    return crit.Criterion;

                throw new InvalidCastException("Unable to cast criterion to NHibernateCriterion<T>.");
            }

            throw new ArgumentException("Argument must be an instance of NHibernateCriterion<T>.", "criterion");
        }

        //https://stackoverflow.com/questions/729295/how-to-cast-expressionfunct-datetime-to-expressionfunct-object
        public static Expression<Func<TInput, object>> ConvertExpression<TInput, TOutput>(Expression<Func<TInput, TOutput>> expression)
        {
            // Add the boxing operation, but get a weakly typed expression
            System.Linq.Expressions.Expression converted = System.Linq.Expressions.Expression.Convert(expression.Body, typeof(object));

            // Use Expression.Lambda to get back to strong typing
            return System.Linq.Expressions.Expression.Lambda<Func<TInput, object>>(converted, expression.Parameters);
        }
    }

    public class NHibernateBulkCopy : IBulkCopy, IDisposable
    {
        private SqlBulkCopy _bcp;

        public string DestinationTableName
        {
            get { return _bcp.DestinationTableName; }
        }

        public NHibernateBulkCopy(NHibernate.ISession session, string destinationTableName)
        {
            SqlConnection conn = session.Connection as SqlConnection;

            if (conn == null)
                throw new NotSupportedException("Only SqlConnection type is supported.");

            var cmd = conn.CreateCommand();

            session.Transaction.Enlist(cmd);

            SqlBulkCopyOptions options = SqlBulkCopyOptions.TableLock;

            _bcp = new SqlBulkCopy(conn, options, cmd.Transaction)
            {
                DestinationTableName = destinationTableName,
                BatchSize = 5000
            };
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

        public void WriteToServer(DataTable dt, DataRowState state)
        {
            _bcp.WriteToServer(dt, state);
        }

        public void Dispose()
        {
            IDisposable disposable = _bcp;
            disposable.Dispose();
        }
    }
}
