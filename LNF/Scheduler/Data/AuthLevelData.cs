using LNF.CommonTools;
using LNF.Repository;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling AuthLevel data using the System.Data namespace.
    /// </summary>
    public static class AuthLevelData
    {
        /// <summary>
        /// Returns all auth levels 
        /// </summary>
        public static DataTable SelectAll()
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .MapSchema()
                    .ApplyParameters(new { Action = "SelectAll" })
                    .FillDataTable("sselScheduler.dbo.procAuthLevelSelect");
        }

        public static DataTable SelectAuthorizable()
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .MapSchema()
                    .ApplyParameters(new { Action = "SelectAuthorizable" })
                    .FillDataTable("sselScheduler.dbo.procAuthLevelSelect");
        }
    }
}
