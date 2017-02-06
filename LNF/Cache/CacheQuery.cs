using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace LNF.Cache
{
    public enum CacheQueryType
    {
        AllPossible,
        OnDemand
    }

    public class CacheQuery<T> 
    {
        private CacheQueryType _type;
        private IMongoCollection<T> _col;
        private Func<IEnumerable<T>> _defval;

        // type indicates whether defval represents all possible items or just the items being selected

        public CacheQuery(IMongoCollection<T> col, Func<IEnumerable<T>> defval, CacheQueryType type)
        {
            _type = type;
            _col = col;
            _defval = defval;
        }

        public T First(Expression<Func<T, bool>> predicate)
        {
            return _col.AsQueryable().First(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _col.AsQueryable().FirstOrDefault(predicate);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _col.AsQueryable().Where(predicate);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _col.AsQueryable().Any(predicate);
        }
    }
}
