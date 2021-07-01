using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public class ToolDataReader
    {
        private readonly SqlConnection _conn;

        public ToolDataReader(SqlConnection conn)
        {
            _conn = conn;
        }

        public DataTable ReadToolDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            // all this does is call sselScheduler.dbo.SSEL_DataRead passing the same arguments so if you're trying to figure
            // out what this stored proc does go directly to sselScheduler.dbo.SSEL_DataRead
            using (var cmd = new SqlCommand("dbo.sselScheduler_Select", _conn) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ToolDataRaw");
                cmd.Parameters.AddWithValue("sDate", sd);
                cmd.Parameters.AddWithValue("eDate", ed);

                if (clientId > 0)
                    cmd.Parameters.AddWithValue("ClientID", clientId);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }

        public DataSet ReadToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0)
        {
            using (var cmd = new SqlCommand("Billing.dbo.ToolDataClean_Select", _conn) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ByDateRange");
                cmd.Parameters.AddWithValue("StartDate", sd);
                cmd.Parameters.AddWithValue("EndDate", ed);

                if (clientId > 0)
                    cmd.Parameters.AddWithValue("ClientID", clientId);

                if (resourceId > 0)
                    cmd.Parameters.AddWithValue("ResourceID", resourceId);

                var ds = new DataSet();
                adap.Fill(ds);

                // Three tables are returned:
                //  0) ToolDataClean
                //  1) Client
                //  2) Resource

                ds.Tables[0].TableName = "ToolDataClean";
                ds.Tables[1].TableName = "Client";
                ds.Tables[2].TableName = "Resource";

                return ds;
            }
        }

        public DataTable ReadToolData(DateTime period, int clientId = 0, int reservationId = 0)
        {
            using (var cmd = new SqlCommand("Billing.dbo.ToolData_Select", _conn) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("Action", "ForToolBilling");
                cmd.Parameters.AddWithValue("Period", period);

                if (clientId > 0)
                    cmd.Parameters.AddWithValue("ClientID", clientId);

                if (reservationId > 0)
                    cmd.Parameters.AddWithValue("ReservationID", reservationId);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }

        public DataTable ReadToolUtilization(string sumCol, bool includeForgiven, DateTime sd, DateTime ed)
        {
            using (var cmd = new SqlCommand("dbo.ToolDataClean_Select", _conn) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "Utilization");
                cmd.Parameters.AddWithValue("SumCol", sumCol);
                cmd.Parameters.AddWithValue("sDate", sd);
                cmd.Parameters.AddWithValue("eDate", ed);
                cmd.Parameters.AddWithValue("IncludeForgiven", includeForgiven);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }
    }
}
