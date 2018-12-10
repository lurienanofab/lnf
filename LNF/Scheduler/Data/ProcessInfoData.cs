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
            var dt = DA.Command()
                .Param(new { ResourceID = resourceId })
                .FillDataTable("sselScheduler.dbo.procProcessInfoSelect");

            dt.Columns["ProcessInfoID"].AutoIncrement = true;
            dt.Columns["ProcessInfoID"].AutoIncrementSeed = 1;
            dt.Columns["ProcessInfoID"].AutoIncrementStep = 1;
            dt.PrimaryKey = new[] { dt.Columns["ProcessInfoID"] };

            return dt;
        }

        /// <summary>
        /// Insert/Update/Delete ProcessInfo
        /// </summary>
        public static void Update(DataTable dt)
        {
            DA.Command().Update(dt, x =>
            {
                x.Insert.SetCommandText("sselScheduler.dbo.procProcessInfoInsert");
                x.Insert.AddParameter("ProcessInfoID", SqlDbType.Int, ParameterDirection.Output);
                x.Insert.AddParameter("ResourceID", SqlDbType.Int);
                x.Insert.AddParameter("ProcessInfoName", SqlDbType.NVarChar, 50);
                x.Insert.AddParameter("ParamName", SqlDbType.NVarChar, 50);
                x.Insert.AddParameter("ValueName", SqlDbType.NVarChar, 50);
                x.Insert.AddParameter("Special", SqlDbType.NVarChar, 50);
                x.Insert.AddParameter("AllowNone", SqlDbType.Bit);
                x.Insert.AddParameter("RequireValue", SqlDbType.Bit);
                x.Insert.AddParameter("RequireSelection", SqlDbType.Bit);
                x.Insert.AddParameter("Order", SqlDbType.Int);

                x.Update.SetCommandText("sselScheduler.dbo.procProcessInfoUpdate");
                x.Update.AddParameter("ProcessInfoID", SqlDbType.Int);
                x.Update.AddParameter("ProcessInfoName", SqlDbType.NVarChar, 50);
                x.Update.AddParameter("ParamName", SqlDbType.NVarChar, 50);
                x.Update.AddParameter("ValueName", SqlDbType.NVarChar, 50);
                x.Update.AddParameter("Special", SqlDbType.NVarChar, 50);
                x.Update.AddParameter("AllowNone", SqlDbType.Bit);
                x.Update.AddParameter("RequireValue", SqlDbType.Bit);
                x.Update.AddParameter("RequireSelection", SqlDbType.Bit);
                x.Update.AddParameter("Order", SqlDbType.Int);

                x.Delete.SetCommandText("sselScheduler.dbo.procProcessInfoDelete");
                x.Delete.AddParameter("ProcessInfoID", SqlDbType.Int);
            });
        }
    }
}