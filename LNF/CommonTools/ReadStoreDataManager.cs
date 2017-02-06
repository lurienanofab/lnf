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

        private ReadStoreDataManager() { }

        private DateTime _StartDate { get; set; }
        private DateTime _EndDate { get; set; }
        private int _ClientID { get; set; }
        private int _ItemID { get; set; }

        public DateTime StartDate { get { return _StartDate; } }
        public DateTime EndDate { get { return _EndDate; } }
        public int ClientID { get { return _ClientID; } }
        public int ItemID { get { return _ItemID; } }

        public static ReadStoreDataManager Create(DateTime startDate, DateTime endDate, int clientId = 0, int itemId = 0)
        {
            ReadStoreDataManager result = new ReadStoreDataManager();
            result._StartDate = startDate;
            result._EndDate = endDate;
            result._ClientID = clientId;
            result._ItemID = itemId;
            return result;
        }

        public DataTable ReadStoreData()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "AggByPeriod")
                    .AddParameter("@Period", StartDate)
                    .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                    .AddParameterIf("@ItemID", ItemID > 0, ItemID);

                var dt = dba.FillDataTable("StoreData_Select");
                dt.TableName = "StoreUsage";

                return dt;
            }
        }

        //Making AllItems true for all items is logical.
        //However, in the SP, we need to pass in a 0 for all items.
        public DataTable ReadStoreDataClean(StoreDataCleanOption option)
        {
            //need to use DA.Current.GetAdapter() because of DryBox data
            using (var adapter = DA.Current.GetAdapter())
            {
                adapter.SelectCommand
                    .AddParameter("@Action", "ByClient")
                    .AddParameter("@sDate", StartDate)
                    .AddParameter("@eDate", EndDate)
                    .AddParameter("@AllItems", (int)option)
                    .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                    .AddParameterIf("@ItemID", ItemID > 0, ItemID);

                return adapter.FillDataTable("StoreDataClean_Select");
            }
        }

        public DataTable ReadStoreDataFiltered()
        {
            //Cannot imagine what sort of cleaning would be needed, but for consistency...
            //Instead of calling StoreDataRaw once per client, pass ClientID=0 for all clients.
            return ReadStoreDataRaw();
        }

        public DataTable ReadStoreDataRaw()
        {
            //need to use DA.Current.GetAdapter() because of DryBox data
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "StoreDataRaw")
                    .AddParameter("@sDate", StartDate)
                    .AddParameter("@eDate", EndDate)
                    .AddParameterIf("@ClientID", ClientID > 0, ClientID);

                return dba.FillDataTable("sselMAS_Select");
            }
        }
    }
}
