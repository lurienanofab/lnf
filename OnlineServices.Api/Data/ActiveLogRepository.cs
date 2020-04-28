using LNF.Data;
using LNF.DataAccess;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class ActiveLogRepository : ApiClient, IActiveLogRepository
    {
        public IEnumerable<IActiveLog> ActiveLogs(string tableName, int record)
        {
            throw new NotImplementedException();
        }

        public void Disable(IActiveDataItem item)
        {
            throw new NotImplementedException();
        }

        public void Enable(IActiveDataItem item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IActiveLog> GetActiveLogs(string tableName, int record)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IActiveLog> GetRange(string tableName, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IActiveLog> GetRange(string tableName, DateTime sd, DateTime ed, int[] records)
        {
            throw new NotImplementedException();
        }

        public bool IsActive(string tableName, int record)
        {
            throw new NotImplementedException();
        }

        public bool IsActive(string tableName, int record, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public bool IsActive(IActiveLog item)
        {
            throw new NotImplementedException();
        }

        public bool IsActive(IActiveLog item, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }
    }
}
