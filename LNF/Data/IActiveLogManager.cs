using System;
using System.Collections.Generic;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IActiveLogManager : IManager
    {
        bool IsActive(IActiveDataItem item);
        bool IsActive(IActiveDataItem item, DateTime sd, DateTime ed);
        bool IsActive(IActiveLogItem item);
        bool IsActive(IActiveLogItem item, DateTime sd, DateTime ed);
        T Item<T>(ActiveLog item) where T : IActiveDataItem;
        IList<ActiveLog> Range(string tableName, DateTime startDate, DateTime endDate);
        IList<ActiveLog> Range(string tableName, DateTime startDate, DateTime endDate, IEnumerable<int> records);
        IEnumerable<T> Range<T>(IEnumerable<T> items, DateTime startDate, DateTime endDate) where T : IActiveDataItem;
    }
}