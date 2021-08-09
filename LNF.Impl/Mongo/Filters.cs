using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LNF.Impl.Mongo
{
    public class Filters<T>
    {
        private FilterDefinition<T> _defs = null;

        internal Filters() { }

        private void AddFilter(FilterDefinition<T> def)
        {
            if (_defs == null)
                _defs = def;
            else
                _defs &= def;
        }

        internal FilterDefinition<T> Combine() => _defs;

        public Filters<T> Where(Expression<Func<T, bool>> filter)
        {
            AddFilter(Builders<T>.Filter.Where(filter));
            return this;
        }

        public Filters<T> AnyEq<TItem>(Expression<Func<T, IEnumerable<TItem>>> field, TItem value)
        {
            AddFilter(Builders<T>.Filter.AnyEq(field, value));
            return this;
        }
    }
}
