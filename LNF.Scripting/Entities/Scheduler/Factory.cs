using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scripting.Entities.Scheduler
{
    public class Factory : IFactory
    {
        public IEnumerable<T1> Search<T1, T2>(Func<T2, bool> fn)
            where T1 : class, IEntity
            where T2 : class, IDataItem
        {
            IEnumerable<T2> query = DA.Current.Query<T2>().Where(fn);
            return query.Select(x => (T1)Activator.CreateInstance(typeof(T1), x));
        }
    }
}
