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
            var dt = DA.Command()
                .Param("Action", "SelectByResource")
                .Param("ResourceID", resourceId)
                .FillDataTable("sselScheduler.dbo.procProcessInfoLineSelect");

            dt.Columns["ProcessInfoLineID"].AutoIncrement = true;
            dt.Columns["ProcessInfoLineID"].AutoIncrementSeed = 1;
            dt.Columns["ProcessInfoLineID"].AutoIncrementStep = 1;
            dt.PrimaryKey = new[] { dt.Columns["ProcessInfoLineID"] };

            return dt;
        }

        public static DataTable SelectByProcessInfo(int processInfoId)
        {
            var dt = DA.Command()
                .Param("Action", "SelectByProcessInfo")
                .Param("ProcessInfoID", processInfoId)
                .FillDataTable("sselScheduler.dbo.procProcessInfoLineSelect");

            dt.Columns["ProcessInfoLineID"].AutoIncrement = true;
            dt.Columns["ProcessInfoLineID"].AutoIncrementSeed = 1;
            dt.Columns["ProcessInfoLineID"].AutoIncrementStep = 1;
            dt.PrimaryKey = new[] { dt.Columns["ProcessInfoLineID"] };

            return dt;
        }

        /// <summary>
        /// Insert/Update/Delete ProcessInfo
        /// </summary>
        public static void Update(DataTable dt)
        {
            DA.Command().Update(dt, x =>
            {
                x.Insert.SetCommandText("sselScheduler.dbo.procProcessInfoLineInsert");
                x.Insert.AddParameter("ProcessInfoLineID", SqlDbType.Int, ParameterDirection.Output);
                x.Insert.AddParameter("ProcessInfoID", SqlDbType.Int);
                x.Insert.AddParameter("Param", SqlDbType.NVarChar, 50);
                x.Insert.AddParameter("MinValue", SqlDbType.Float);
                x.Insert.AddParameter("MaxValue", SqlDbType.Float);

                x.Update.SetCommandText("sselScheduler.dbo.procProcessInfoLineUpdate");
                x.Update.AddParameter("ProcessInfoLineID", SqlDbType.Int);
                x.Update.AddParameter("Param", SqlDbType.NVarChar, 50);
                x.Update.AddParameter("MinValue", SqlDbType.Float);
                x.Update.AddParameter("MaxValue", SqlDbType.Float);

                x.Delete.SetCommandText("sselScheduler.dbo.procProcessInfoLineDelete");
                x.Delete.AddParameter("ProcessInfoLineID", SqlDbType.Int);
            });
        }
    }
}