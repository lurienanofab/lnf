using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class ActiveLogRepository : RepositoryBase, IActiveLogRepository
    {
        public ActiveLogRepository(ISessionManager mgr) : base(mgr) { }

        /// <summary>
        /// Gets all ActiveLog entities for the IActiveDataItem item.
        /// </summary>
        public IEnumerable<IActiveLog> GetActiveLogs(string tableName, int record)
        {
            return Session.Query<ActiveLog>().Where(x => x.TableName == tableName && x.Record == record).CreateModels<IActiveLog>();
        }

        /// <summary>
        /// Sets Active to false and updates ActiveLog
        /// </summary>
        public void Disable(LNF.DataAccess.IActiveDataItem item)
        {
            Session.Disable(item);
        }

        /// <summary>
        /// Sets Active to true and updates ActiveLog
        /// </summary>
        public void Enable(LNF.DataAccess.IActiveDataItem item)
        {
            Session.Enable(item);
        }

        public IEnumerable<IActiveLog> GetRange(string tableName, DateTime sd, DateTime ed)
        {
            return Session.Query<ActiveLog>().Where(x => x.TableName == tableName && x.EnableDate < ed && (x.DisableDate == null || x.DisableDate.Value > sd)).CreateModels<IActiveLog>();
        }

        public IEnumerable<IActiveLog> GetRange(string tableName, DateTime sd, DateTime ed, int[] records)
        {
            return Session.Query<ActiveLog>().Where(x => x.TableName == tableName && records.Contains(x.Record) && x.EnableDate < ed && (x.DisableDate == null || x.DisableDate.Value > sd)).CreateModels<IActiveLog>();
        }

        /// <summary>
        /// Checks to see if an item is currently active
        /// </summary>
        /// <param name="item">The IActiveDataItem item</param>
        /// <returns>True if the item is currently active, otherwise false</returns>
        public bool IsActive(string tableName, int record)
        {
            return Session.Query<ActiveLog>().Any(x => x.TableName == tableName && x.Record == record && x.DisableDate == null);
        }

        /// <summary>
        /// Checks to see if an item was active during the date range
        /// </summary>
        /// <param name="item">The IActiveDataItem item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>True if the item was active during the date range, otherwise false</returns>
        public bool IsActive(string tableName, int record, DateTime sd, DateTime ed)
        {
            return Session.Query<ActiveLog>().Any(x => x.TableName == tableName && x.Record == record && x.EnableDate < ed && (x.DisableDate == null || x.DisableDate.Value > sd));
        }

        /// <summary>
        /// Checks to see if an item is currently active
        /// </summary>
        /// <param name="item">The IActiveLogItem item</param>
        /// <returns>True if the item is currently active, otherwise false</returns>
        public bool IsActive(IActiveLog item)
        {
            return item.DisableDate == null;
        }

        /// <summary>
        /// Checks to see if an item was active during the date range
        /// </summary>
        /// <param name="item">The IActiveLogItem item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>True if the item was active during the date range, otherwise false</returns>
        public bool IsActive(IActiveLog item, DateTime sd, DateTime ed)
        {
            return item.EnableDate < ed && (item.DisableDate == null || item.DisableDate > sd);
        }
    }
}