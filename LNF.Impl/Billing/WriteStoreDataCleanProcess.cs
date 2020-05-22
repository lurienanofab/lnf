using LNF.Billing.Process;
using LNF.CommonTools;
using LNF.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    /// <summary>
    /// This process will:
    ///     1) Select records from Store in the date range to insert.
    ///     2) Delete records from StoreDataClean in the date range.
    ///     3) Insert records from Store into StoreDataClean.
    ///     4) Insert records for DryBox into StoreDataClean.
    /// </summary>
    public class WriteStoreDataCleanProcess : ProcessBase<WriteStoreDataCleanResult>
    {
        public static readonly int DryBoxCategoryID = 33;

        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int ClientID { get; }

        private DataSet _ds;
        private Dictionary<int, DataRow> _map;
        private EnumerableRowCollection<DataRow> _prices;

        protected override WriteStoreDataCleanResult CreateResult()
        {
            return new WriteStoreDataCleanResult
            {
                StartDate = StartDate,
                EndDate = EndDate,
                ClientID = ClientID
            };
        }

        public WriteStoreDataCleanProcess(SqlConnection conn, DateTime sd, DateTime ed, int clientId = 0) : base(conn)
        {
            StartDate = sd;
            EndDate = ed;
            ClientID = clientId;
        }

        public override int DeleteExisting()
        {
            using (var cmd = new SqlCommand("dbo.StoreDataClean_Delete", Connection) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("sDate", StartDate);
                cmd.Parameters.AddWithValue("eDate", EndDate);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public override DataTable Extract()
        {
            var reader = new StoreDataReader(Connection);
            var dtStoreDataRaw = reader.ReadStoreDataRaw(StartDate, EndDate, ClientID);
            dtStoreDataRaw.TableName = "StoreDataRaw";

            _ds = ServiceProvider.Current.Data.DryBox.ReadDryBoxData(StartDate, EndDate, ClientID);
            _ds.Tables.Add(dtStoreDataRaw);

            return dtStoreDataRaw;
        }

        public override DataTable Transform(DataTable dtExtract)
        {
            // The table dtExtract already has normal StoreDataClean rows and does
            // not need to be tranformed, but we need to add drybox data to it.
            AddDryBoxData(dtExtract);
            return dtExtract;
        }

        public override IBulkCopy CreateBulkCopy()
        {
            IBulkCopy bcp = new DefaultBulkCopy("dbo.StoreDataClean");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("ItemID");
            bcp.AddColumnMapping("OrderDate");
            bcp.AddColumnMapping("AccountID");
            bcp.AddColumnMapping("Quantity");
            bcp.AddColumnMapping("UnitCost");
            bcp.AddColumnMapping("CategoryID");
            bcp.AddColumnMapping("RechargeItem");
            bcp.AddColumnMapping("StatusChangeDate");
            return bcp;
        }

        private void AddDryBoxData(DataTable dtStoreDataClean)
        {
            DataTable dtDryBoxAssignment = _ds.Tables["DryBoxAssignment"];

            // setup the itemId-chargeTypeId map
            var items = _ds.Tables["DryBoxItem"].AsEnumerable();
            _map = new Dictionary<int, DataRow>
            {
                { 5, items.FirstOrDefault(x => x.Field<string>("Description").Contains("[Academic]")) },
                { 15, items.FirstOrDefault(x => x.Field<string>("Description").Contains("[Academic]")) },
                { 25, items.FirstOrDefault(x => x.Field<string>("Description").Contains("[Non-Academic]")) }
            };

            // setup prices
            _prices = _ds.Tables["Price"].AsEnumerable();

            var clientAccounts = _ds.Tables["ClientAccount"].AsEnumerable();

            int dryboxRows = 0;

            // For drybox we need the next day as end date when running the current period.
            var ed = EndDate > DateTime.Now.Date.AddDays(1) ? DateTime.Now.Date.AddDays(1) : EndDate;

            foreach (DataRow dr in dtDryBoxAssignment.Rows)
            {
                DataRow sdc = dtStoreDataClean.NewRow();

                DateTime enableDate = dr.Field<DateTime>("EnableDate");
                DateTime? disableDate = dr.Field<DateTime?>("DisableDate");

                DateTime sdate = (enableDate < StartDate) ? StartDate : enableDate;
                DateTime edate;

                if (disableDate.HasValue)
                    edate = (disableDate.Value > ed) ? ed : disableDate.Value;
                else
                    edate = ed;

                double daysUsed = (edate - sdate).TotalDays;
                double daysInPeriod = (EndDate - StartDate).TotalDays;

                //may be zero if approved and removed on the same day
                if (daysUsed > 0)
                {
                    var ca = clientAccounts.FirstOrDefault(x => x.Field<int>("ClientAccountID") == dr.Field<int>("ClientAccountID"));
                    var iid = GetItemID(ca.Field<int>("ChargeTypeID"));
                    var clientId = ca.Field<int>("ClientID");

                    var qty = Math.Round(daysUsed / daysInPeriod, 4);
                    var unitCost = GetHistoricalPrice(iid, edate);

                    sdc.SetField("ClientID", clientId);
                    sdc.SetField("ItemID", iid);
                    sdc.SetField("OrderDate", sdate);
                    sdc.SetField("AccountID", ca.Field<int>("AccountID"));
                    sdc.SetField("Quantity", qty);
                    sdc.SetField("UnitCost", unitCost);
                    sdc.SetField("CategoryID", DryBoxCategoryID);
                    sdc.SetField("RechargeItem", true);
                    sdc.SetField("StatusChangeDate", sdate);
                    dtStoreDataClean.Rows.Add(sdc);

                    dryboxRows += 1;
                }
            }

            _result.DryBoxRows = dryboxRows;
        }

        private int GetItemID(int chargeTypeId)
        {
            var dr = _map[chargeTypeId];
            return dr.Field<int>("ItemID");
        }

        private double GetHistoricalPrice(int itemId, DateTime edate)
        {
            var rows = _prices.Where(x => x.Field<int>("ItemID") == itemId).ToArray();

            var included = rows.Where(x => x.Field<DateTime>("DateActive") <= edate.Date).ToArray();

            DataRow dr = null;

            if (included.Length > 0)
                dr = rows.Where(x => x.Field<DateTime>("DateActive") == included.Max(m => m.Field<DateTime>("DateActive"))).FirstOrDefault();
            if (dr == null)
                dr = rows[0];

            var result = dr.Field<double>("PackagePrice");

            return result;
        }
    }
}
