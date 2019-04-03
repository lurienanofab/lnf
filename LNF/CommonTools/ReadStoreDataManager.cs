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
    public class ReadStoreDataManager : ManagerBase, IReadStoreDataManager
    {
        public ReadStoreDataManager(IProvider provider) : base(provider) { }

        public DataTable ReadStoreDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            var dt = new DataTable();
            dt.Columns.Add("Quantity", typeof(double)); // make Quantity column double for dryboxes

            Command()
                .Param("Action", "StoreDataRaw")
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("ClientID", clientId > 0, clientId)
                .FillDataTable(dt, "dbo.sselMAS_Select");

            return dt;
        }

        //Making AllItems true for all items is logical.
        //However, in the SP, we need to pass in a 0 for all items.
        public DataTable ReadStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, StoreDataCleanOption option = StoreDataCleanOption.AllItems)
        {
            return Command()
                .Param("Action", "ByClient")
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("AllItems", (int)option)
                .Param("ClientID", clientId > 0, clientId)
                .Param("ItemID", itemId > 0, itemId)
                .FillDataTable("dbo.StoreDataClean_Select");
        }

        public DataTable ReadStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            return Command()
                .Param("Action", "AggByPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .Param("ItemID", itemId > 0, itemId)
                .FillDataTable("dbo.StoreData_Select");
        }
    }
}
