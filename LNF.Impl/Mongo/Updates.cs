using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LNF.Impl.Mongo
{
    public class Updates<T>
    {
        private List<UpdateDefinition<T>> _defs = new List<UpdateDefinition<T>>();

        internal Updates() { }

        internal UpdateDefinition<T> Combine() => Builders<T>.Update.Combine(_defs);

        public Updates<T> Set<TField>(Expression<Func<T, TField>> field, TField value)
        {
            _defs.Add(Builders<T>.Update.Set(field, value));
            return this;
        }

        public Updates<T> AddToSet<TItem>(Expression<Func<T, IEnumerable<TItem>>> field, TItem value)
        {
            _defs.Add(Builders<T>.Update.AddToSet(field, value));
            return this;
        }

        public Updates<T> Pull<TItem>(Expression<Func<T, IEnumerable<TItem>>> field, TItem value)
        {
            _defs.Add(Builders<T>.Update.Pull(field, value));
            return this;
        }

        public Updates<T> Pull<TItem>(Expression<Func<T, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> filter)
        {
            var filterDef = Builders<TItem>.Filter.Where(filter);
            _defs.Add(Builders<T>.Update.PullFilter(field, filterDef));
            return this;
        }
    }

}
