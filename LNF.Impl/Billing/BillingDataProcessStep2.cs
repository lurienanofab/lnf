using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    //2010-07-01 This class is used for the first time
    //This class will handle the transformation from RoomBilling, ToolBilling and StoreBilling into 
    //RoomBillingByRoomOrg, ToolBillingTyToolOrg, StoreBillingByItemOrg
    //2011-01-01
    //Now we will also process data into RoomBillingByAccount, ToolBillingByAccount, StoreBillingByAccount
    public class BillingDataProcessStep2 : ReaderBase
    {
        public BillingDataProcessStep2(SqlConnection conn) : base(conn) { }

        /// <summary>
        /// Populate the RoomBillingByRoomOrg table
        /// </summary>
        public int PopulateRoomBillingByRoomOrg(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.RoomApportionmentInDaysMonthly_Select", "PopulateRoomBillingByRoomOrg", period, clientId);
        }

        /// <summary>
        /// Populate the RoomBillingByAccount table
        /// </summary>
        public int PopulateRoomBillingByAccount(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.RoomApportionmentInDaysMonthly_Select", "PopulateRoomBillingByAccount", period, clientId);
        }

        public int PopulateToolBillingByToolOrg(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.ToolBilling_Select", "PopulateToolBillingByToolOrg", period, clientId);
        }

        public int PopulateToolBillingByAccount(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.ToolBilling_Select", "PopulateToolBillingByAccount", period, clientId);
        }

        public int PopulateStoreBillingByItemOrg(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.StoreBilling_Select", "PopulateStoreBillingByItemOrg", period, clientId);
        }

        public int PopulateStoreBillingByAccount(DateTime period, int clientId = 0)
        {
            return ExecuteAction("dbo.StoreBilling_Select", "PopulateStoreBillingByAccount", period, clientId);
        }

        private int ExecuteAction(string proc, string action, DateTime period, int clientId)
        {
            using (var cmd = new SqlCommand(proc, Connection) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("Period", period);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }
    }
}
