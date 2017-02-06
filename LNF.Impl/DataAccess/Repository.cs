using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Context;
using NHibernate.Linq;
using LNF.Repository;
using LNF.Impl.DataAccess.Scheduler;

namespace LNF.Impl.DataAccess
{
    public class Repository<TContext, TDataItem> : NHibernateRepository<TContext>, IRepository<TDataItem>
        where TContext : ICurrentSessionContext
        where TDataItem : IDataItem
    {
        public TDataItem Single(object id)
        {
            return Session.Get<TDataItem>(id);
        }

        public IQueryable<TDataItem> Query()
        {
            return Session.Query<TDataItem>();
        }
    }
}
