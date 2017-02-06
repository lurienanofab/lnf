using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Billing
{
    public class ExternalInvoiceManager
    {
        public int AccountID { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool ShowRemote { get; }

        private DataSet _ds;

        public ExternalInvoiceManager(DateTime sd, DateTime ed, bool showRemote)
            : this(0, sd, ed, showRemote) { }

        public ExternalInvoiceManager(int accountId, DateTime sd, DateTime ed, bool showRemote)
        {
            AccountID = 0;
            StartDate = sd;
            EndDate = ed;
            ShowRemote = showRemote;

            _ds = new DataSet();

            LoadToolData();
            LoadRoomData();
            LoadStoreData();
            LoadMiscData();
        }

        public IList<InvoiceLineItem> GetToolData(int accountId)
        {
            var result = GetLineItems(_ds.Tables["Tool"], accountId);
            return result;
        }

        public IList<InvoiceLineItem> GetRoomData(int accountId)
        {
            var result = GetLineItems(_ds.Tables["Room"], accountId);
            return result;
        }

        public IList<InvoiceLineItem> GetStoreData(int accountId)
        {
            var result = GetLineItems(_ds.Tables["Store"], accountId);
            return result;
        }

        public IList<InvoiceLineItem> GetMiscData(int accountId)
        {
            var result = GetLineItems(_ds.Tables["Misc"], accountId);
            return result;
        }

        public IList<InvoiceSummaryItem> GetSummary()
        {
            var chargeTypes = DA.Current.Query<ChargeType>().Where(x => x.ChargeTypeID > 5).ToList();

            var dtTool = _ds.Tables["ToolRaw"];
            var dtRoom = _ds.Tables["RoomRaw"];
            var dtStore = _ds.Tables["StoreRaw"];
            var dtMisc = _ds.Tables["MiscRaw"];

            var result = new List<InvoiceSummaryItem>();

            foreach (var ct in chargeTypes)
            {
                InvoiceSummaryItem summaryItem = new InvoiceSummaryItem();

                summaryItem.ChargeTypeID = ct.ChargeTypeID;
                summaryItem.ChargeTypeName = ct.ChargeTypeName;

                double total = 0;

                total += GetDouble(dtTool.Compute("SUM(LineCost)", string.Format("ChargeTypeID = {0}", summaryItem.ChargeTypeID)));
                total += GetDouble(dtRoom.Compute("SUM(LineCost)", string.Format("ChargeTypeID = {0}", summaryItem.ChargeTypeID)));
                total += GetDouble(dtStore.Compute("SUM(LineCost)", string.Format("ChargeTypeID = {0}", summaryItem.ChargeTypeID)));
                total += GetDouble(dtMisc.Compute("SUM(LineCost)", string.Format("ChargeTypeID = {0}", summaryItem.ChargeTypeID)));

                summaryItem.TotalCharges = total;

                result.Add(summaryItem);
            }

            return result;
        }

        private double GetDouble(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return 0;
            else
                return Convert.ToDouble(obj);
        }

        private IList<InvoiceLineItem> GetLineItems(DataTable dt, int accountId)
        {
            DataRow[] rows = dt.Select(string.Format("AccountID = {0}", accountId), "ClientID ASC");

            var result = rows.Select(x => new InvoiceLineItem()
            {
                ClientID = x.Field<int>("ClientID"),
                AccountID = x.Field<int>("AccountID"),
                Description = x.Field<string>("Descrip"),
                Quantity = x.Field<double>("Quantity"),
                Cost = x.Field<double>("Cost")
            }).ToList();

            return result;
        }

        private void LoadToolData()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                if (!ShowRemote)
                    dba.AddParameter("@BillingTypeID", BillingType.Remote);

                // It will return a dataset with two tables inside
                // table #1: the data table contains individual tool usage
                // table #2: the client table contains all users who has used the tools on this account
                var ds = dba.FillDataSet("ToolBilling_Select");

                DataTable dt = ds.Tables[0];
                DataTable dtClient = ds.Tables[1];

                dt.Columns.Add("LineCost", typeof(double));

                //Calculate the true cost based on billing types
                LineCostUtility.CalculateToolLineCost(dt);

                DataTable result = new DataTable();
                result.Columns.Add("ClientID", typeof(int));
                result.Columns.Add("AccountID", typeof(int));
                result.Columns.Add("Descrip", typeof(string));
                result.Columns.Add("Quantity", typeof(double));
                result.Columns.Add("Cost", typeof(double));

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]));

                    DataRow ndr = result.NewRow();
                    ndr["ClientID"] = dr["ClientID"];
                    ndr["AccountID"] = dr["AccountID"];
                    ndr["Descrip"] = "Tool - " + Utility.Left(rows.First()["DisplayName"].ToString(), 35);
                    ndr["Quantity"] = 1;
                    ndr["Cost"] = totalFee;
                    result.Rows.Add(ndr);
                }

                dt.TableName = "ToolRaw";
                dtClient.TableName = "ToolClients";
                result.TableName = "Tool";

                _ds.Tables.Add(dt.Copy());
                _ds.Tables.Add(dtClient.Copy());
                _ds.Tables.Add(result);
            }
        }

        private void LoadRoomData()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                if (!ShowRemote)
                    dba.AddParameter("@BillingTypeID", BillingType.Remote);

                DataSet ds = dba.FillDataSet("RoomApportionmentInDaysMonthly_Select");

                DataTable dt = ds.Tables[0];
                DataTable dtClient = ds.Tables[1];

                dt.Columns.Add("LineCost", typeof(double));

                //Calculate the true cost based on billing types
                LineCostUtility.CalculateRoomLineCost(dt);

                DataTable result = new DataTable();
                result.Columns.Add("ClientID", typeof(int));
                result.Columns.Add("AccountID", typeof(int));
                result.Columns.Add("Descrip", typeof(string));
                result.Columns.Add("Quantity", typeof(double));
                result.Columns.Add("Cost", typeof(double));

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] drows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]), string.Empty);

                    DataRow ndr = result.NewRow();
                    ndr["ClientID"] = dr["ClientID"];
                    ndr["AccountID"] = dr["AccountID"];
                    ndr["Descrip"] = string.Format("Room - {0}", Utility.Left(drows[0]["DisplayName"].ToString(), 35));
                    ndr["Quantity"] = 1;
                    ndr["Cost"] = totalFee;
                    result.Rows.Add(ndr);
                }

                dt.TableName = "RoomRaw";
                dtClient.TableName = "RoomClients";
                result.TableName = "Room";

                _ds.Tables.Add(dt.Copy());
                _ds.Tables.Add(dtClient.Copy());
                _ds.Tables.Add(result);
            }
        }

        private void LoadStoreData()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                var ds = dba.FillDataSet("StoreBilling_Select");

                DataTable dt = ds.Tables[0];
                DataTable dtClient = ds.Tables[1];

                DataTable result = new DataTable();
                result.Columns.Add("ClientID", typeof(int));
                result.Columns.Add("AccountID", typeof(int));
                result.Columns.Add("Descrip", typeof(string));
                result.Columns.Add("Quantity", typeof(double));
                result.Columns.Add("Cost", typeof(double));

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]));

                    DataRow ndr = result.NewRow();
                    ndr["ClientID"] = dr["ClientID"];
                    ndr["AccountID"] = dr["AccountID"];
                    ndr["Descrip"] = "Store - " + Utility.Left(rows.First()["DisplayName"].ToString(), 35);
                    ndr["Quantity"] = 1;
                    ndr["Cost"] = totalFee;
                    result.Rows.Add(ndr);
                }

                dt.TableName = "StoreRaw";
                dtClient.TableName = "StoreClients";
                result.TableName = "Store";

                _ds.Tables.Add(dt.Copy());
                _ds.Tables.Add(dtClient.Copy());
                _ds.Tables.Add(result);
            }
        }

        private void LoadMiscData()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                var dt = dba.FillDataTable("MiscBillingCharge_Select");

                dt.Columns.Add("LineCost", typeof(double), "Quantity * Cost");

                DataTable result = new DataTable();
                result.Columns.Add("ClientID", typeof(int));
                result.Columns.Add("AccountID", typeof(int));
                result.Columns.Add("Descrip", typeof(string));
                result.Columns.Add("Quantity", typeof(double));
                result.Columns.Add("Cost", typeof(double));

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow ndr = result.NewRow();
                    ndr["ClientID"] = dr["ClientID"];
                    ndr["AccountID"] = dr["AccountID"];
                    ndr["Descrip"] = Utility.Left(dr["Description"].ToString() + " " + dr["DisplayName"].ToString(), 49);
                    ndr["Quantity"] = dr["Quantity"];
                    ndr["Cost"] = dr["Cost"];
                    result.Rows.Add(ndr);
                }

                dt.TableName = "MiscRaw";
                result.TableName = "Misc";

                _ds.Tables.Add(dt.Copy());
                _ds.Tables.Add(result);
            }
        }
    }
}
