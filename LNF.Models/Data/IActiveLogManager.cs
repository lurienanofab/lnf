using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IActiveLogManager
    {
        IEnumerable<IActiveLog> GetActiveLogs(string tableName, int record);
        IEnumerable<IActiveLog> GetRange(string tableName, DateTime sd, DateTime ed);
        IEnumerable<IActiveLog> GetRange(string tableName, DateTime sd, DateTime ed, int[] records);
        bool IsActive(string tableName, int record);
        bool IsActive(string tableName, int record, DateTime sd, DateTime ed);
        bool IsActive(IActiveLog item);
        bool IsActive(IActiveLog item, DateTime sd, DateTime ed);
        void Disable(string tableName, int record);
        void Enable(string tableName, int record);
    }
}