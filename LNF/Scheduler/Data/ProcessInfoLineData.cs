using LNF.CommonTools;
using LNF.Repository;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ProcessInfoLine data using the System.Data namespace.
    /// </summary>
    public static class ProcessInfoLineData
    {
        /// <summary>
        /// Returns all ProcessInfo belonging to the specified Resource
        /// </summary>
        public static DataTable SelectByResource(int resourceId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                var dt = dba
                    .ApplyParameters(new { Action = "SelectByResource", ResourceID = resourceId })
                    .FillDataTable("sselScheduler.dbo.procProcessInfoLineSelect");

                dt.Columns["ProcessInfoLineID"].AutoIncrement = true;
                dt.Columns["ProcessInfoLineID"].AutoIncrementSeed = 1;
                dt.Columns["ProcessInfoLineID"].AutoIncrementStep = 1;
                dt.PrimaryKey = new[] { dt.Columns["ProcessInfoLineID"] };

                return dt;
            }
        }

        public static DataTable SelectByProcessInfo(int processInfoId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                var dt = dba
                    .ApplyParameters(new { Action = "SelectByProcessInfo", ProcessInfoID = processInfoId })
                    .FillDataTable("sselScheduler.dbo.procProcessInfoLineSelect");

                dt.Columns["ProcessInfoLineID"].AutoIncrement = true;
                dt.Columns["ProcessInfoLineID"].AutoIncrementSeed = 1;
                dt.Columns["ProcessInfoLineID"].AutoIncrementStep = 1;
                dt.PrimaryKey = new[] { dt.Columns["ProcessInfoLineID"] };

                return dt;
            }
        }

        /// <summary>
        /// Insert/Update/Delete ProcessInfo
        /// </summary>
        public static void Update(DataTable dt)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.InsertCommand
                    .AddParameter("@ProcessInfoLineID", SqlDbType.Int, ParameterDirection.Output)
                    .AddParameter("@ProcessInfoID", SqlDbType.Int)
                    .AddParameter("@Param", SqlDbType.NVarChar, 50)
                    .AddParameter("@MinValue", SqlDbType.Float)
                    .AddParameter("@MaxValue", SqlDbType.Float);

                dba.UpdateCommand
                    .AddParameter("@ProcessInfoLineID", SqlDbType.Int)
                    .AddParameter("@Param", SqlDbType.NVarChar, 50)
                    .AddParameter("@MinValue", SqlDbType.Float)
                    .AddParameter("@MaxValue", SqlDbType.Float);

                dba.DeleteCommand.AddParameter("@ProcessInfoLineID", SqlDbType.Int);

                dba.UpdateDataTable(dt,
                    "sselScheduler.dbo.procProcessInfoLineInsert",
                    "sselScheduler.dbo.procProcessInfoLineUpdate",
                    "sselScheduler.dbo.procProcessInfoLineDelete");
            }
        }
    }
}