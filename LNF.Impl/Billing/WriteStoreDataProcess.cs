using LNF.Billing;
using LNF.Billing.Process;
using LNF.CommonTools;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public class WriteStoreDataConfig : PeriodProcessConfig
    {
        public int ItemID { get; set; }
    }

    /// <summary>
    /// This process will:
    ///     1) Delete records from StoreData in the date range.
    ///     2) Select records from StoreDataClean in the date range to insert.
    ///     3) Insert records from StoreDataClean records into StoreData.
    /// </summary>
    public class WriteStoreDataProcess : PeriodProcessBase<WriteStoreDataResult>
    {
        private readonly WriteStoreDataConfig _config;

        public int ItemID => _config.ItemID;

        public WriteStoreDataProcess(WriteStoreDataConfig cfg) : base(cfg)
        {
            _config = cfg;
        }

        public override string ProcessName => "StoreData";

        protected override WriteStoreDataResult CreateResult(DateTime startedAt)
        {
            return new WriteStoreDataResult(startedAt)
            {
                Period = Period,
                ClientID = ClientID,
                ItemID = ItemID
            };
        }

        public override int DeleteExisting()
        {
            //get rid of any non-user entered entries
            using (var cmd = Connection.CreateCommand("dbo.StoreData_Delete"))
            {
                AddParameter(cmd, "Period", Period, SqlDbType.DateTime);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID, SqlDbType.Int);
                AddParameterIf(cmd, "ItemID", ItemID > 0, ItemID, SqlDbType.Int);
                AddParameter(cmd, "Context", _config.Context, SqlDbType.NVarChar, 50);
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
