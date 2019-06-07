using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class ActiveLogManager : ManagerBase, IActiveLogManager
    {
        private static readonly string[] allowed = { "Account", "Client", "ClientAccount", "ClientManager", "ClientOrg", "ClientRemote", "Org" };

        public ActiveLogManager(IProvider provider) : base(provider) { }

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
        public void Disable(string tableName, int record)
        {
            if (!allowed.Contains(tableName))
                throw new ArgumentException("TableName is invalid.", "tableName");

            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be saved first.", "record");

            DA.Command(CommandType.Text)
                .Param("Record", record)
                .ExecuteNonQuery($"UPDATE sselData.dbo.{tableName} SET Active = 0 WHERE {tableName}ID = @Record");

            DA.Command(CommandType.Text)
                .Param("TableName", tableName)
                .Param("Record", record)
                .Param("DisableDate", DateTime.Now.Date.AddDays(1))
                .ExecuteNonQuery("UPDATE sselData.dbo.ActiveLog SET DisableDate = @DisableDate WHERE TableName = @TableName AND Record = @Record AND DisableDate IS NULL");
        }

        /// <summary>
        /// Sets Active to true and updates ActiveLog
        /// </summary>
        public void Enable(string tableName, int record)
        {
            if (!allowed.Contains(tableName))
                throw new ArgumentException("TableName is invalid.", "tableName");

            if (record == 0)
                throw new ArgumentException("Record cannot be zero. If this is a new object it should be saved first.", "record");

            Command(CommandType.Text)
                .Param("Record", record)
                .ExecuteNonQuery($"UPDATE sselData.dbo.{tableName} SET Active = 1 WHERE {tableName}ID = @Record");

            ActiveLog alog = Session.Query<ActiveLog>().FirstOrDefault(x => x.Record == record && x.TableName == tableName && !x.DisableDate.HasValue);

            // if an ActiveLog with null DisableDate already exists then there is no reason to create a new one or do anything else
            if (alog == null)
            {
                Session.Insert(new ActiveLog()
                {
                    DisableDate = null,
                    EnableDate = DateTime.Now.Date,
                    Record = record,
                    TableName = tableName
                });
            }
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