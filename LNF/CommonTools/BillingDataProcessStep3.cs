using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Logging;

namespace LNF.CommonTools
{
    //2010-07-01 This class is used for the first time
    //This will populate the RoomBillingByOrg, ToolBillingByOrg, and StoreBillingByOrg
    public class BillingDataProcessStep3
    {
        public static int PopulateRoomBillingByOrg(DateTime period, int clientId = 0)
        {
            int result = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep3.PopulateRoomBillingByOrg", "period = '{0:yyyy-MM-dd}', ClientID = {1}, result = {2}", () => new object[] { period, clientId, result }))
            {
                using (var dba = DA.Current.GetAdapter())
                {
                    result = dba
                        .AddParameter("@Action", "PopulateRoomBillingByOrg")
                        .AddParameter("@Period", period)
                        .AddParameterIf("@ClientID", clientId > 0, clientId)
                        .ExecuteNonQuery("RoomBillingByRoomOrg_Select");
                    return result;
                }
            }
        }

        public static int PopulateToolBillingByOrg(DateTime period, int clientId = 0)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep3.PopulateToolBillingByOrg", "period = '{0:yyyy-MM-dd}', clientId = {1}, count = {2}", () => new object[] { period, clientId, count }))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand
                    .AddParameter("@Action", "PopulateToolBillingByOrg")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .ExecuteNonQuery("ToolBillingByToolOrg_Select");

                return count;
            }
        }

        public static int PopulateStoreBillingByOrg(DateTime period)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep3.PopulateStoreBillingByOrg", "period = '{0:yyyy-MM-dd}', count = {1}", () => new object[] { period, count }))
            using (var dba = DA.Current.GetAdapter())
            {
                count = dba.SelectCommand.ApplyParameters(new { Action = "PopulateStoreBillingByOrg", Period = period }).ExecuteNonQuery("StoreBillingByItemOrg_Select");
                return count;
            }
        }
    }
}
