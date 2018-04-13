using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using System.Text;
using System.Threading.Tasks;

namespace LNF.Repository
{
    public class Repository<T> : ISession<T> where T : IDataItem
    {
        public IQueryable<T> Query()
        {
            return DA.Current.Query<T>();
        }

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            return Query().FirstOrDefault(predicate);
        }

        public T First(Func<T, bool> predicate)
        {
            return Query().First(predicate);
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return Query().Where(predicate);
        }

        public int Count(Func<T, bool> predicate)
        {
            return Query().Count(predicate);
        }

        public bool Any(Func<T, bool> predicate)
        {
            return Query().Any(predicate);
        }

        public T Single(object id)
        {
            return DA.Current.Single<T>(id);
        }
    }
}
