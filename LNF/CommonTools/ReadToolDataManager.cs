using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public class ReadToolDataManager : ManagerBase, IReadToolDataManager
    {
        public ReadToolDataManager(IProvider provider) : base(provider) { }

        public DataTable ReadToolDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            return Command()
                .Param("Action", "ToolDataRaw")
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("ClientID", clientId > 0, clientId)
                .FillDataTable("dbo.sselScheduler_Select");
        }

        public DataSet ReadToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0)
        {
            var ds = Command()
                .Param("Action", "ByDateRange")
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("ClientID", clientId > 0, clientId)
                .Param("ResourceID", resourceId > 0, resourceId)
                .FillDataSet("dbo.ToolDataClean_Select");

            // Three tables are returned:
            //  0) ToolDataClean
            //  1) Client
            //  2) Resource

            ds.Tables[0].TableName = "ToolDataClean";
            ds.Tables[1].TableName = "Client";
            ds.Tables[2].TableName = "Resource";

            return ds;
        }

        public DataTable ReadToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            return Command()
                .Param("Action", "ByMonthTool")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .Param("ResourceID", resourceId > 0, resourceId)
                .FillDataTable("dbo.ToolData_Select");
        }

        public DataTable ReadToolUtilization(string sumCol, bool includeForgiven, DateTime sd, DateTime ed)
        {
            return Command().Param(new
            {
                Action = "Utilization",
                SumCol = sumCol,
                sDate = sd,
                eDate = ed,
                IncludeForgiven = includeForgiven
            }).FillDataTable("dbo.ToolDataClean_Select");
        }
    }
}
