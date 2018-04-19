using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Data
{
    public interface IActiveDataItemManager : IManager
    {
        IQueryable<ActiveLog> ActiveLogs(IActiveDataItem item);
        void Disable(IActiveDataItem item);
        void Enable(IActiveDataItem item);
        IEnumerable<T> FindActive<T>(IQueryable<T> query, Expression<Func<T, int>> record, DateTime sd, DateTime ed) where T : IActiveDataItem;
        IEnumerable<ActiveLogItem<T>> Range<T>(IQueryable<T> list, Expression<Func<T, ActiveLogKey>> key, DateTime startDate, DateTime endDate) where T : class, IActiveDataItem;
    }
}