using LNF.Data;
using LNF.Models.Billing.Process;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    /// <summary>
    /// This process will:
    ///     1) Select records from Store in the date range to insert.
    ///     2) Delete records from StoreDataClean in the date range.
    ///     3) Insert records from Store into StoreDataClean.
    ///     4) Insert records for DryBox into StoreDataClean.
    /// </summary>
    public class WriteStoreDataCleanProcess : ProcessBase<WriteStoreDataCleanProcessResult>
    {
        public static readonly int DryBoxCategoryID = 33;

        public IDryBoxManager DryBoxManager => ServiceProvider.Current.Use<IDryBoxManager>();

        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int ClientID { get; }

        private DataSet _ds;
        private Dictionary<int, DataRow> _map;
        private EnumerableRowCollection<DataRow> _prices;

        public WriteStoreDataCleanProcess(DateTime sd, DateTime ed, int clientId = 0)
        {
            StartDate = sd;
            EndDate = ed;
            ClientID = clientId;
        }

        public override int DeleteExisting()
        {
            return DA.Command()
                .Param("sDate", StartDate)
                .Param("eDate", EndDate)
                .Param("ClientID", ClientID > 0, ClientID)
                .ExecuteNonQuery("dbo.StoreDataClean_Delete").Value;
        }

        public override DataTable Extract()
        {
            var dtStoreDataRaw = ReadData.Store.ReadStoreDataRaw(StartDate, EndDate, ClientID);
            dtStoreDataRaw.TableName = "StoreDataRaw";

            _ds = DryBoxManager.ReadDryBoxData(StartDate, EndDate, ClientID);
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
            IBulkCopy bcp = DA.Current.GetBulkCopy("dbo.StoreDataClean");
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
