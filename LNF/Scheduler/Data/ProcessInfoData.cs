using LNF.CommonTools;
using LNF.Repository;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ProcessInfo data using the System.Data namespace.
    /// </summary>
    public static class ProcessInfoData
    {
        /// <summary>
        /// Returns all ProcessInfo belonging to the specified Resource
        /// </summary>
        public static DataTable SelectProcessInfo(int resourceId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                var dt = dba
                    .ApplyParameters(new { ResourceID = resourceId })
                    .FillDataTable("sselScheduler.dbo.procProcessInfoSelect");

                dt.Columns["ProcessInfoID"].AutoIncrement = true;
                dt.Columns["ProcessInfoID"].AutoIncrementSeed = 1;
                dt.Columns["ProcessInfoID"].AutoIncrementStep = 1;
                dt.PrimaryKey = new[] { dt.Columns["ProcessInfoID"] };

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
                    .AddParameter("@ProcessInfoID", SqlDbType.Int, ParameterDirection.Output)
                    .AddParameter("@ResourceID", SqlDbType.Int)
                    .AddParameter("@ProcessInfoName", SqlDbType.NVarChar, 50)
                    .AddParameter("@ParamName", SqlDbType.NVarChar, 50)
                    .AddParameter("@ValueName", SqlDbType.NVarChar, 50)
                    .AddParameter("@Special", SqlDbType.NVarChar, 50)
                    .AddParameter("@AllowNone", SqlDbType.Bit)
                    .AddParameter("@RequireValue", SqlDbType.Bit)
                    .AddParameter("@RequireSelection", SqlDbType.Bit)
                    .AddParameter("@Order", SqlDbType.Int);

                dba.UpdateCommand
                    .AddParameter("@ProcessInfoID", SqlDbType.Int)
                    .AddParameter("@ProcessInfoName", SqlDbType.NVarChar, 50)
                    .AddParameter("@ParamName", SqlDbType.NVarChar, 50)
                    .AddParameter("@ValueName", SqlDbType.NVarChar, 50)
                    .AddParameter("@Special", SqlDbType.NVarChar, 50)
                    .AddParameter("@AllowNone", SqlDbType.Bit)
                    .AddParameter("@RequireValue", SqlDbType.Bit)
                    .AddParameter("@RequireSelection", SqlDbType.Bit)
                    .AddParameter("@Order", SqlDbType.Int);

                dba.DeleteCommand.AddParameter("@ProcessInfoID", SqlDbType.Int);

                dba.UpdateDataTable(dt,
                    "sselScheduler.dbo.procProcessInfoInsert",
                    "sselScheduler.dbo.procProcessInfoUpdate",
                    "sselScheduler.dbo.procProcessInfoDelete");
            }
        }
    }
}