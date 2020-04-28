using LNF.DataAccess;
using System;
using System.Collections.Generic;

namespace LNF.Scripting.Entities
{
    public interface IFactory
    {
        IEnumerable<T1> Search<T1, T2>(NHibernate.ISession session, Func<T2, bool> fn)
            where T1 : class, IEntity
            where T2 : class, IDataItem;
    }
}
