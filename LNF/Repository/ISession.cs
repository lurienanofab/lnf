using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Repository
{
    public static class SessionExtensions
    {
        public static void Insert(this ISession repo, IDataItem item)
        {
            repo.Insert(new[] { item });
        }

        public static void Delete(this ISession repo, IDataItem item)
        {
            repo.Delete(new[] { item });
        }
    }


    // This is an exact copy of NHiberante.CacheMode
    [Serializable, Flags]
    public enum CacheMode
    {
        Ignore = 0,
        Put = 1,
        Get = 2,
        Normal = 3,
        Refresh = 5
    }

    public interface ISession<T> where T : IDataItem
    {
        T Single(object id);
        IQueryable<T> Query();
    }

    public interface ISession
    {
        /// <summary>
        /// Insert a collection of entity objects into the database. Works on unattached entities.
        /// </summary>
        void Insert(IEnumerable<IDataItem> items);

        /// <summary>
        /// Delete an existing database record for each entity in the collection. Works on unattached entities
        /// </summary>
        void Delete(IEnumerable<IDataItem> items);

        /// <summary>
        /// Inserts a new database record if it does not already exist, or updates an existing record. Works on unattached entities.
        /// Also if an entity is modified this must be called so that any subsequent queries that are affected by the modification
        /// will return the exepected results.
        /// </summary>
        void SaveOrUpdate(IDataItem item);

        /// <summary>
        /// Retrieve a single entity from the database
        /// </summary>
        T Single<T>(object id) where T : IDataItem;

        /// <summary>
        /// Returns a queryable object for Linq support
        /// </summary>
        /// <typeparam name="T">The expected type of the return value</typeparam>
        /// <returns>A queryable instance for type T</returns>
        IQueryable<T> Query<T>() where T : IDataItem;

        /// <summary>
        /// Returns a queryable object for Linq support
        /// </summary>
        /// <typeparam name="T">The expected type of the return value</typeparam>
        /// <param name="mode">A value that indicates how to interact with cache during the query</param>
        /// <returns>A queryable instance for type T</returns>
        IQueryable<T> Cache<T>(CacheMode mode = CacheMode.Normal) where T : IDataItem;

        /// <summary>
        /// Gets the total number of items
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>An integer value</returns>
        int Count<T>() where T : IDataItem;

        /// <summary>
        /// Gets the total number of items that match the specified filter
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>An integer value</returns>
        int Count<T>(Func<T, bool> filter) where T : IDataItem;

        /// <summary>
        /// Gets a value that indicates if the item already exists
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="filter">The filter used to find the item</param>
        /// <returns>True if the item exists, otherwise false</returns>
        bool Exists<T>(Func<T, bool> filter) where T : IDataItem;

        /// <summary>
        /// Retrieve and update an attached entity using the detached item
        /// </summary>
        T Merge<T>(T item) where T : class, IDataItem;

        /// <summary>
        /// Retrieves a UnitOfWorkAdapter object that shares a transaction with this instance.
        /// </summary>
        UnitOfWorkAdapter GetAdapter();

        IBulkCopy GetBulkCopy(string destinationTableName);

        /// <summary>
        /// Perform all pending operations on the database.
        /// </summary>
        void Flush();

        /// <summary>
        /// Remove this instance from the session cache.
        /// </summary>
        void Evict(IDataItem item);

        /// <summary>
        /// Evict all entries from the process-level cache. This method occurs outside of
        /// any transaction; it performs an immediate "hard" remove, so does not respect
        /// any transaction isolation semantics of the usage strategy. Use with care.
        /// </summary>
        void Evict<T>() where T : IDataItem;

        /// <summary>
        /// Re-read the state of the given instance from the underlying database.
        /// </summary>
        void Refresh(IDataItem item);

        INamedQuery NamedQuery(string name);

        ISqlQuery SqlQuery(string sql);

        IQueryBuilder<T> QueryBuilder<T>() where T : class, IDataItem;

        /// <summary>
        /// Returns the base instance of a proxy class if the implementation uses proxy classes (as NHibernate does). If the item is not a proxy the object is simply returned.
        /// </summary>
        object Unproxy(IDataItem proxy);
    }

    public interface IQueryBase
    {
        /// <summary>
        /// Convenience method to return a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        T Result<T>() where T : struct;

        /// <summary>
        /// Execute the update or delete statement.
        /// </summary>
        int Update();
    }

    public interface INamedQuery : IQueryBase
    {
        IList<T> List<T>() where T : IDataItem;
        INamedQuery SetParameters(object parameters);
        INamedQuery SetParameters(IDictionary<string, object> parameters);
        INamedQuery SetParameter(string name, object value);
        INamedQuery SetParameterList(string name, IEnumerable values);
    }

    public interface ISqlQuery : IQueryBase
    {
        IList<T> List<T>();
        IList<IDictionary> List();
        ISqlQuery SetParameters(object parameters);
        ISqlQuery SetParameters(IDictionary<string, object> parameters);
        ISqlQuery SetParameter(string name, object value);
        ISqlQuery SetParameterList(string name, IEnumerable values);
    }

    public interface IQueryBuilder<T> where T : class, IDataItem
    {
        IQueryBuilder<T> Where(Expression<Func<T, bool>> expression);
        IQueryBuilder<T> Where(IConjunction<T> conjunction);
        IQueryBuilder<T> Where(IDisjunction<T> disjunction);
        IQueryBuilder<T> Where(ICriterion<T> criterion);

        IQueryBuilder<T> And(Expression<Func<T, bool>> expression);
        IQueryBuilder<T> And(IConjunction<T> conjunction);
        IQueryBuilder<T> And(IDisjunction<T> disjunction);
        IQueryBuilder<T> And(ICriterion<T> criterion);

        IQueryBuilder<T> OrderBy(Expression<Func<T, object>> expression);
        IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> expression);
        IQueryBuilder<T> ThenBy(Expression<Func<T, object>> expression);
        IQueryBuilder<T> ThenByDescending(Expression<Func<T, object>> expression);

        IQueryBuilder<T> Skip(int value);
        IQueryBuilder<T> Take(int value);

        IConjunction<T> Conjunction();
        IDisjunction<T> Disjunction();
        IRestriction<T, TValue> Restriction<TValue>(Expression<Func<T, TValue>> expression);

        IList<T> List();
        TResult Result<TResult>() where TResult : struct;

        int Count();
    }

    public interface IDisjunction<T> where T : class, IDataItem
    {
        IDisjunction<T> Add(Expression<Func<T, bool>> expression);
        IDisjunction<T> Add(ICriterion<T> criterion);
    }

    public interface IConjunction<T> where T : class, IDataItem
    {
        IConjunction<T> Add(Expression<Func<T, bool>> expression);
        IConjunction<T> Add(ICriterion<T> criterion);
    }

    public interface IRestriction<T, TValue> where T : class, IDataItem
    {
        /// <summary>
        /// Performs a search using [Column] IN ({values[0]}, {values[1]}, {values[2]}, ...)
        /// </summary>
        ICriterion<T> In(IEnumerable<TValue> values);

        /// <summary>
        /// Performs a search using [Column] LIKE {value} (% wildcards are not assumed)
        /// </summary>
        ICriterion<T> Like(string value);

        /// <summary>
        /// Performs a search using [Column] LIKE {value}%
        /// </summary>
        ICriterion<T> StartsWith(string value);

        /// <summary>
        /// Performs a search using [Column] LIKE %{value}
        /// </summary>
        ICriterion<T> EndsWith(string value);

        /// <summary>
        /// Performs a search using [Column] LIKE %{value}%
        /// </summary>
        ICriterion<T> Contains(string value);

        /// <summary>
        /// Performs a bitwise search using ([Column] & {value}) > 0
        /// </summary>
        ICriterion<T> HasAny(TValue value);

        /// <summary>
        /// Performs a bitwise search using ([Column] & {value}) = {value}
        /// </summary>
        ICriterion<T> HasBit(TValue value);
    }

    public interface ICriterion<T> where T : class, IDataItem { }

    public interface IBulkCopy : IDisposable
    {
        string DestinationTableName { get; }
        void AddColumnMapping(string sourceColumn, string destinationColumn);
        void AddColumnMapping(string columnName);
        void WriteToServer(DataTable dt);
        void WriteToServer(DataTable dt, DataRowState state);
    }
}
