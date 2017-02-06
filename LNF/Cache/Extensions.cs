using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Cache
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the value from a MongoCacheObject. If the MongoCacheObject is null a default value is returned.
        /// </summary>
        public static T GetValue<T>(this CacheObject<T> mco)
        {
            if (mco == null)
                return default(T);
            else
                return mco.Value;
        }

        /// <summary>
        /// Gets a list of values from a collection of MongoCacheObjects. If the collection is null an empty list is returned.
        /// </summary>
        public static IList<T> GetValues<T>(this IEnumerable<CacheObject<T>> mcos)
        {
            if (mcos == null)
                return new List<T>();
            else
                return mcos.Select(x => x.Value).ToList();
        }

        public static IList<T> GetValues<T>(this IFindFluent<CacheObject<T>, CacheObject<T>> fluent)
        {
            return fluent.ToList().GetValues();
        }

        public static IndexKeysDefinition<T> GetKeysAscending<T>(this IndexKeysDefinitionBuilder<T> builder, IEnumerable<Expression<Func<T, object>>> fields)
        {
            IndexKeysDefinition<T> keys = null;

            foreach (var f in fields)
            {
                if (keys == null)
                    keys = builder.Ascending(f);
                else
                    keys = keys.Ascending(f);
            }

            return keys;
        }

        public static IMongoCollection<T> Expire<T>(this IMongoCollection<T> col, TimeSpan? expiry, Expression<Func<T, object>> field)
        {
            if (!expiry.HasValue) return col;
            string indexName = "ttl_index";
            var options = new CreateIndexOptions() { Name = indexName, ExpireAfter = expiry.Value };
            var builder = new IndexKeysDefinitionBuilder<T>();
            var keys = builder.Ascending(field);
            col.Indexes.CreateOne(keys, options);
            return col;
        }

        public static IMongoCollection<T> Unique<T>(this IMongoCollection<T> col, params Expression<Func<T, object>>[] fields)
        {
            if (fields == null || fields.Length == 0) return col;
            string indexName = "unique_index";
            var options = new CreateIndexOptions() { Name = indexName, Unique = true };
            var builder = new IndexKeysDefinitionBuilder<T>();
            var keys = builder.GetKeysAscending(fields);
            col.Indexes.CreateOne(keys, options);
            return col;
        }

        public static IQueryable<T> Query<T>(this IMongoCollection<T> col, Expression<Func<T, bool>> filter, Func<IEnumerable<T>> defval, bool delete)
        {
            // defval should return all possible items if delete is true

            var query = col.AsQueryable();

            var result = query.Where(filter);

            if (result == null || result.Count() == 0)
            {
                var items = defval();

                if (delete)
                    col.DeleteMany(FilterDefinition<T>.Empty);

                if (items != null && items.Count() > 0)
                    col.InsertMany(items);

                result = query.Where(filter);
            }

            return result;
        }
    }
}
