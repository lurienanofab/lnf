using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    //- Raw means straight from the DB.
    //- Cleaned means that extraneous data have been removed.
    //- Unlike room and tool, for which the cost is stored in the local DB, for the store
    //  the costs are stored remotely. Thus, when store activity is gathered, the cost will
    //  will be gathered as well.
    public class ReadStoreDataManager
    {
        public enum StoreDataCleanOption
        {
            AllItems = 0,
            RechargeItems = 1
        }

        public ReadStoreDataManager() { }

        public DataTable ReadStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            using (var adap = DA.Current.GetAdapter())
            {
                adap.SelectCommand
                    .AddParameter("@Action", "AggByPeriod")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@ItemID", itemId > 0, itemId);

                var dt = adap.FillDataTable("StoreData_Select");
                dt.TableName = "StoreUsage";

                return dt;
            }
        }

        //Making AllItems true for all items is logical.
        //However, in the SP, we need to pass in a 0 for all items.
        public DataTable ReadStoreDataClean(StoreDataCleanOption option, DateTime sd, DateTime ed, int clientId = 0, int itemId = 0)
        {
            //need to use DA.Current.GetAdapter() because of DryBox data
            using (var adap = DA.Current.GetAdapter())
            {
                adap.SelectCommand
                    .AddParameter("@Action", "ByClient")
                    .AddParameter("@sDate", sd)
                    .AddParameter("@eDate", ed)
                    .AddParameter("@AllItems", (int)option)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@ItemID", itemId > 0, itemId);

                return adap.FillDataTable("StoreDataClean_Select");
            }
        }

        public DataTable ReadStoreDataFiltered(DateTime sd, DateTime ed, int clientId = 0)
        {
            //Cannot imagine what sort of cleaning would be needed, but for consistency...
            //Instead of calling StoreDataRaw once per client, pass ClientID=0 for all clients.
            return ReadStoreDataRaw(sd, ed, clientId);
        }

        public DataTable ReadStoreDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            //need to use DA.Current.GetAdapter() because of DryBox data
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "StoreDataRaw")
                    .AddParameter("@sDate", sd)
                    .AddParameter("@eDate", ed)
                    .AddParameterIf("@ClientID", clientId > 0, clientId);

                return dba.FillDataTable("sselMAS_Select");
            }
        }
    }
}
