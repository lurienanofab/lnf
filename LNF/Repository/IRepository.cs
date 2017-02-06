using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Repository
{
    public static class RepositoryExtensions
    {
        public static void Insert(this IRepository repo, IDataItem item)
        {
            repo.Insert(new[] { item });
        }

        public static void Delete(this IRepository repo, IDataItem item)
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

    public interface IRepository<T> where T : IDataItem
    {
        T Single(object id);
        IQueryable<T> Query();
    }

    public interface IRepository
    {
        /// <summary>
        /// Used to notify listeners when the UnitOfWork is disposed
        /// </summary>
        //public event EventHandler Disposed;

        int GetTransactionHashCode();

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

        QueryBuilder QueryBuilder();

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
        /// Remove the item from the session.
        /// </summary>
        void Evict(IDataItem item);

        T SqlResult<T>(string sql, object parameters);

        IList<T> SqlQuery<T>(string sql, object parameters) where T : IDataItem;

        /// <summary>
        /// Returns the base instance of a proxy class if the implementation uses proxy classes (as NHibernate does). If the item is not a proxy the object is simply returned.
        /// </summary>
        object Unproxy(IDataItem proxy);
    }

    public interface IBulkCopy : IDisposable
    {
        string DestinationTableName { get; }
        void AddColumnMapping(string sourceColumn, string destinationColumn);
        void AddColumnMapping(string columnName);
        void WriteToServer(DataTable dt);
    }
}
