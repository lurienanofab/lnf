using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IActiveDataItemManager : IManager
    {
        IQueryable<ActiveLog> ActiveLogs(IActiveDataItem item);
        void Disable(IActiveDataItem item);
        void Enable(IActiveDataItem item);
        IEnumerable<T> FindActive<T>(IQueryable<T> query, Expression<Func<T, int>> record, DateTime sd, DateTime ed) where T : IActiveDataItem;
        IEnumerable<ActiveLogItem<IActiveDataItem>> Range(IEnumerable<IActiveDataItem> list, DateTime startDate, DateTime endDate);
    }
}