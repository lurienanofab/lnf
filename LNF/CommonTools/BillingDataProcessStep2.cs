using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    //2010-07-01 This class is used for the first time
    //This class will handle the transformation from RoomBilling, ToolBilling and StoreBilling into 
    //RoomBillingByRoomOrg, ToolBillingTyToolOrg, StoreBillingByItemOrg
    //2011-01-01
    //Now we will also process data into RoomBillingByAccount, ToolBillingByAccount, StoreBillingByAccount
    public class BillingDataProcessStep2
    {
        /// <summary>
        /// Populate the RoomBillingByRoomOrg table
        /// </summary>
        public static int PopulateRoomBillingByRoomOrg(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateRoomBillingByRoomOrg")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.RoomApportionmentInDaysMonthly_Select").Value;
        }

        /// <summary>
        /// Populate the RoomBillingByAccount table
        /// </summary>
        public static int PopulateRoomBillingByAccount(DateTime period, int clientId = 0)
        {
            return DA.Command()
                    .Param("Action", "PopulateRoomBillingByAccount")
                    .Param("Period", period)
                    .Param("ClientID", clientId > 0, clientId)
                    .ExecuteNonQuery("dbo.RoomApportionmentInDaysMonthly_Select").Value;
        }

        public static int PopulateToolBillingByToolOrg(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateToolBillingByToolOrg")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.ToolBilling_Select").Value;
        }

        public static int PopulateToolBillingByAccount(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateToolBillingByAccount")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.ToolBilling_Select").Value;
        }

        public static int PopulateStoreBillingByItemOrg(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateStoreBillingByItemOrg")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.StoreBilling_Select").Value;
        }

        public static int PopulateStoreBillingByAccount(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateStoreBillingByAccount")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.StoreBilling_Select").Value;
        }
    }
}
