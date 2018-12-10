using LNF.Models.Billing.Process;
using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    /// <summary>
    /// This process will:
    ///     1) Delete records from StoreData in the date range.
    ///     2) Select records from StoreDataClean in the date range to insert.
    ///     3) Insert records from StoreDataClean records into StoreData.
    /// </summary>
    public class WriteStoreDataProcess : ProcessBase<WriteStoreDataProcessResult>
    {
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int ItemID { get; set; }

        public WriteStoreDataProcess(DateTime period, int clientId = 0, int itemId = 0)
        {
            Period = period;
            ClientID = clientId;
            ItemID = itemId;
        }

        public override int DeleteExisting()
        {
            //get rid of any non-user entered entries
            return DA.Command()
                .Param("Period", Period)
                .Param("ClientID", ClientID > 0, ClientID)
                .Param("ItemID", ItemID > 0, ItemID)
                .ExecuteNonQuery("dbo.StoreData_Delete").Value;
        }

        public override DataTable Extract()
        {
            //get all store data for period
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);
            return ReadData.Store.ReadStoreDataClean(sd, ed, ClientID, ItemID, StoreDataCleanOption.RechargeItems);
        }

        public override DataTable Transform(DataTable dtExtract)
        {
            dtExtract.Columns.Add("Period", typeof(DateTime));
            foreach (DataRow dr in dtExtract.Rows)
                dr.SetField("Period", Period);
            return dtExtract;
        }

        public override IBulkCopy CreateBulkCopy()
        {
            var bcp = DA.Current.GetBulkCopy("dbo.StoreData");
            bcp.AddColumnMapping("Period");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("ItemID");
            bcp.AddColumnMapping("OrderDate");
            bcp.AddColumnMapping("AccountID");
            bcp.AddColumnMapping("Quantity");
            bcp.AddColumnMapping("UnitCost");
            bcp.AddColumnMapping("CategoryID");
            bcp.AddColumnMapping("StatusChangeDate");
            return bcp;
        }
    }
}
