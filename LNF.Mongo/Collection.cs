using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LNF.Mongo
{
    public class Collection<T>
    {
        private readonly IMongoCollection<T> _col;

        internal Collection(IMongoCollection<T> col)
        {
            _col = col;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return _col.Find(filter).ToList();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            var cur = await _col.FindAsync(filter);
            return cur.ToList();
        }

        public IEnumerable<T> All()
        {
            return _col.Find(FilterDefinition<T>.Empty).ToList();
        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            var cur = await _col.FindAsync(FilterDefinition<T>.Empty);
            return cur.ToList();
        }

        public async Task InsertOneAsync(T item)
        {
            await _col.InsertOneAsync(item);
        }

        public async Task<long> DeleteOneAsync(Expression<Func<T, bool>> filter)
        {
            var deleteResult = await _col.DeleteOneAsync<T>(filter);

            if (deleteResult.IsAcknowledged)
                return deleteResult.DeletedCount;
            else
                throw new Exception("Delete was note acknowledged.");
        }

        public async Task<T> FindOneAndReplaceAsync(Expression<Func<T, bool>> filter, T item, bool returnDocumentAfter, bool isUpsert)
        {
            //ReturnDocument.Before = 0
            //ReturnDocument.After = 1

            var opts = new FindOneAndReplaceOptions<T>()
            {
                ReturnDocument = returnDocumentAfter ? ReturnDocument.After : ReturnDocument.Before,
                IsUpsert = isUpsert
            };

            var result = await _col.FindOneAndReplaceAsync<T>(filter, item, opts);

            return result;
        }
    }
}
