using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Context;
using LNF.Repository;

namespace LNF.Impl.DataAccess
{
    public abstract class RepositoryCollection<TContext> where TContext : ICurrentSessionContext
    {
        protected Repository<TContext, TDataItem> CreateRepository<TDataItem>() where TDataItem : IDataItem
        {
            return new Repository<TContext, TDataItem>();
        }
    }
}
