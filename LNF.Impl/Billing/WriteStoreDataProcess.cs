using LNF.Billing;
using LNF.Billing.Process;
using LNF.CommonTools;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    /// <summary>
    /// This process will:
    ///     1) Delete records from StoreData in the date range.
    ///     2) Select records from StoreDataClean in the date range to insert.
    ///     3) Insert records from StoreDataClean records into StoreData.
    /// </summary>
    public class WriteStoreDataProcess : ProcessBase<WriteStoreDataResult>
    {
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int ItemID { get; set; }

        public WriteStoreDataProcess(SqlConnection conn, DateTime period, int clientId = 0, int itemId = 0) : base(conn)
        {
            Period = period;
            ClientID = clientId;
            ItemID = itemId;
        }

        protected override WriteStoreDataResult CreateResult()
        {
            return new WriteStoreDataResult
            {
                Period = Period,
                ClientID = ClientID,
                ItemID = ItemID
            };
        }

        public override int DeleteExisting()
        {
            //get rid of any non-user entered entries
            using (var cmd = new SqlCommand("dbo.StoreData_Delete", Connection) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);
                AddParameterIf(cmd, "ItemID", ItemID > 0, ItemID);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public override DataTable Extract()
        {
            //get all store data for period
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);
            var reader = new StoreDataReader(Connection);
            return reader.ReadStoreDataClean(sd, ed, ClientID, ItemID, StoreDataCleanOption.RechargeItems);
        }

        public override DataTable Transform(DataTable dtExtract)
        {
            dtExtract.Columns.Add("Period", typeof(DateTime));
            foreach (DataRow dr in dtExtract.Rows)
                dr.SetField("Period", Period);
            return dtExtract;
        }

        public override LNF.DataAccess.IBulkCopy CreateBulkCopy()
        {
            var bcp = new DefaultBulkCopy("dbo.StoreData");
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
