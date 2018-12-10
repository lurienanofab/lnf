using LNF.Repository;
using System;

namespace LNF.CommonTools
{
    //2010-07-01 This class is used for the first time
    //This will populate the RoomBillingByOrg, ToolBillingByOrg, and StoreBillingByOrg
    public class BillingDataProcessStep3
    {
        public static int PopulateRoomBillingByOrg(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateRoomBillingByOrg")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.RoomBillingByRoomOrg_Select").Value;
        }

        public static int PopulateToolBillingByOrg(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateToolBillingByOrg")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.ToolBillingByToolOrg_Select").Value;
        }

        public static int PopulateStoreBillingByOrg(DateTime period)
        {
            return DA.Command()
                .Param(new { Action = "PopulateStoreBillingByOrg", Period = period })
                .ExecuteNonQuery("dbo.StoreBillingByItemOrg_Select").Value;
        }
    }
}
