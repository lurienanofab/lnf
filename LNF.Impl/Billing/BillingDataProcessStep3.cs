using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    //2010-07-01 This class is used for the first time
    //This will populate the RoomBillingByOrg, ToolBillingByOrg, and StoreBillingByOrg
    public class BillingDataProcessStep3 : ReaderBase
    {
        public BillingDataProcessStep3(SqlConnection conn) : base(conn) { }

        public int PopulateRoomBillingByOrg(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.RoomBillingByRoomOrg_Select", "PopulateRoomBillingByOrg", period, clientId);
        }

        public int PopulateToolBillingByOrg(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.ToolBillingByToolOrg_Select", "PopulateToolBillingByOrg", period, clientId);
        }

        public int PopulateStoreBillingByOrg(DateTime period)
        {
            return ExecuteAction("dbo.StoreBillingByItemOrg_Select", "PopulateStoreBillingByOrg", period, 0);
        }

        private int ExecuteAction(string proc, string action, DateTime period, int clientId)
        {
            using (var cmd = Connection.CreateCommand(proc))
            {
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("Period", period);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
        }
    }
}
