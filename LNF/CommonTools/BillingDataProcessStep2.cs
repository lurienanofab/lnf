using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Logging;

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
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep2.PopulateRoomBillingByRoomOrg", "period = '{0:yyyy-MM-dd}', clientId = {1}, count = {2}", () => new object[] { period, clientId, count}))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand
                    .AddParameter("@Action", "PopulateRoomBillingByRoomOrg")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .ExecuteNonQuery("RoomApportionmentInDaysMonthly_Select");
                return count;
            }
        }

        /// <summary>
        /// Populate the RoomBillingByAccount table
        /// </summary>
        public static int PopulateRoomBillingByAccount(DateTime period, int clientId = 0)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep2.PopulateRoomBillingByAccount", "period = '{0:yyyy-MM-dd}', clientId = {1}, count = {2}", () => new object[] { period, clientId, count }))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand
                    .AddParameter("@Action", "PopulateRoomBillingByAccount")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .ExecuteNonQuery("RoomApportionmentInDaysMonthly_Select");
                return count;
            }
        }

        public static int PopulateToolBillingByToolOrg(DateTime period, int clientId = 0)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep2.PopulateToolBillingByToolOrg", "period = '{0:yyyy-MM-dd}', clientId = {1}, count = {2}", () => new object[] { period, clientId, count }))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand
                    .AddParameter("@Action", "PopulateToolBillingByToolOrg")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId) 
                    .ExecuteNonQuery("ToolBilling_Select");
                return count;
            }
        }

        public static int PopulateToolBillingByAccount(DateTime period, int clientId = 0)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep2.PopulateToolBillingByAccount", "period = '{0:yyyy-MM-dd}', clientId = {1}, count = {2}", () => new object[] { period, clientId, count }))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand
                    .AddParameter("@Action", "PopulateToolBillingByAccount")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId) 
                    .ExecuteNonQuery("ToolBilling_Select");
                return count;
            }
        }

        public static int PopulateStoreBillingByItemOrg(DateTime period, int clientId = 0)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep2.PopulateStoreBillingByItemOrg", "period = '{0:yyyy-MM-dd}', clientId = {1}, count = {2}", () => new object[] { period, clientId, count }))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand
                    .AddParameter("@Action", "PopulateStoreBillingByItemOrg")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId) 
                    .ExecuteNonQuery("StoreBilling_Select");
                return count;
            }
        }

        public static int PopulateStoreBillingByAccount(DateTime period, int clientId = 0)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep2.PopulateStoreBillingByAccount", "period = '{0:yyyy-MM-dd}', clientId = {1}, count = {2}", () => new object[] { period, clientId, count }))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand
                    .AddParameter("@Action", "PopulateStoreBillingByAccount")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId) 
                    .ExecuteNonQuery("StoreBilling_Select");
                return count;
            }
        }
    }
}
