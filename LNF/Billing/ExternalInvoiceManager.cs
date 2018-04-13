using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LNF.Repository;
using LNF.Repository.Billing;

namespace LNF.Billing
{
    public class ExternalInvoiceManager : IEnumerable<KeyValuePair<string, ExternalInvoiceUsage>>
    {
        public int AccountID { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool ShowRemote { get; }
        public bool IncludeAccountsWithNoUsage { get; set; }
        public BillingTypeManager BillingTypeManager { get; }
        public ISession Session { get { return BillingTypeManager.Session; } }

        private IDictionary<string, ExternalInvoiceUsage> _data;

        /// <summary>
        /// Generates invoices for all accounts.
        /// </summary>
        public ExternalInvoiceManager(DateTime sd, DateTime ed, bool showRemote, ISession session, BillingTypeManager mgr)
            : this(0, sd, ed, showRemote, session, mgr) { }

        /// <summary>
        /// Generates invoices for a single account.
        /// </summary>
        public ExternalInvoiceManager(int accountId, DateTime sd, DateTime ed, bool showRemote, ISession session, BillingTypeManager mgr)
        {
            AccountID = accountId;
            StartDate = sd;
            EndDate = ed;
            ShowRemote = showRemote;
            IncludeAccountsWithNoUsage = false;
            BillingTypeManager = mgr;

            _data = GetAllUsage();
        }

        public ExternalInvoiceUsage this[string key]
        {
            get { return _data[key]; }
        }

        public IEnumerable<ExternalInvoiceSummaryItem> GetSummary()
        {
            var chargeTypes = GetExternalChargeTypes();

            foreach (DataRow dr in chargeTypes.Rows)
            {
                var chargeTypeId = dr.Field<int>("ChargeTypeID");
                var chargeTypeName = dr.Field<string>("ChargeType");

                yield return new ExternalInvoiceSummaryItem()
                {
                    ChargeTypeID = chargeTypeId,
                    ChargeTypeName = chargeTypeName,
                    TotalCharges = _data.Sum(x => x.Value.GetChargeTypeTotal(chargeTypeId))
                };
            }
        }

        public IList<ExternalInvoice> GetInvoices(int accountId = 0)
        {
            var headers = GetHeaders(accountId);
            IList<ExternalInvoice> result = new List<ExternalInvoice>();

            foreach (var h in headers)
            {
                var item = new ExternalInvoice()
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Header = h,
                    Usage = GetAccountUsage(h.AccountID)
                };

                result.Add(item);
            }

            return result;
        }

        public ExternalInvoiceUsage GetAccountUsage(int accountId)
        {
            List<ExternalInvoiceLineItem> result = _data.SelectMany(x => x.Value.GetAccountItems(accountId)).ToList();
            return new ExternalInvoiceUsage(result);
        }

        public IEnumerable<ExternalInvoiceHeader> GetHeaders(int accountId = 0)
        {
            var headers = new List<ExternalInvoiceHeader>();

            var lineItems = _data.SelectMany(x => x.Value).Where(x => accountId == 0 || x.AccountID == accountId);

            // First get orgs based on usage. If one has usage it should be included even if it's not active.
            foreach (var item in lineItems)
            {
                if (!headers.Any(x => x.AccountID == item.AccountID))
                {
                    headers.Add(new ExternalInvoiceHeader()
                    {
                        AccountID = item.AccountID,
                        OrgID = item.OrgID,
                        OrgName = item.OrgName,
                        AccountName = item.AccountName,
                        PoEndDate = item.PoEndDate,
                        PoRemainingFunds = item.PoRemainingFunds,
                        InvoiceNumber = item.InvoiceNumber,
                        DeptRef = item.DeptRef,
                        HasActivity = true
                    });
                }
            }

            if (IncludeAccountsWithNoUsage)
            {
                // Second get orgs based on active during period. Here we will get the orgs that were active but didn't have any usage.

                // get all active external orgs along with their accounts
                var dtActiveOrg = GetActiveExternalOrgs(StartDate, EndDate);

                foreach (DataRow dr in dtActiveOrg.Rows)
                {
                    if (!headers.Any(x => x.AccountID == dr.Field<int>("AccountID")))
                    {
                        // found an active org with no activity
                        headers.Add(new ExternalInvoiceHeader()
                        {
                            AccountID = dr.Field<int>("AccountID"),
                            OrgID = dr.Field<int>("OrgID"),
                            OrgName = dr.Field<string>("OrgName"),
                            AccountName = dr.Field<string>("Name"),
                            PoEndDate = dr.Field<DateTime?>("PoEndDate"),
                            PoRemainingFunds = dr.Field<decimal?>("PoRemainingFunds").GetValueOrDefault(),
                            InvoiceNumber = dr.Field<string>("InvoiceNumber"),
                            DeptRef = dr.Field<string>("DisplayDeptRefID"),
                            HasActivity = false
                        });
                    }
                }
            }

            var result = headers.Where(x => IncludeAccountsWithNoUsage || x.HasActivity).OrderBy(x => x.OrgName).ThenBy(x => x.AccountName);

            return result;
        }

        public DataTable GetExternalChargeTypes()
        {
            using (var dba = Session.GetAdapter())
                return dba.ApplyParameters(new { Action = "External" }).FillDataTable("ChargeType_Select");
        }

        public DataTable GetActiveExternalOrgs(DateTime sd, DateTime ed)
        {
            using (var dba = Session.GetAdapter())
            {
                DataTable dt = dba.ApplyParameters(new { Action = "ActiveExternal", sDate = sd, eDate = ed }).FillDataTable("Org_Select");
                dt.PrimaryKey = new[] { dt.Columns["AccountID"] };
                return dt;
            }
        }

        public ExternalInvoiceUsage GetToolUsage()
        {
            using (var dba = Session.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);
                dba.AddParameter("@IsInternal", false);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                if (!ShowRemote)
                    dba.AddParameter("@BillingTypeID", BillingType.Remote);

                // It will return a dataset with two tables inside
                // table #1: the data table contains individual tool usage
                // table #2: the client table contains all users who has used the tools on this account
                var ds = dba.FillDataSet("ToolBilling_Select");

                ds.Tables[0].Columns.Add("LineCost", typeof(double));

                //Calculate the true cost based on billing types
                BillingTypeManager.CalculateToolLineCost(ds.Tables[0]);

                var dt = ds.Tables[0];
                var dtClient = ds.Tables[1];

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]));
                    string desc = ExternalInvoiceUtility.GetToolDescription(rows[0]);
                    result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
                }

                return result;
            }
        }

        public ExternalInvoiceUsage GetExternalRoomUsage()
        {
            using (var dba = Session.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);
                dba.AddParameter("@IsInternal", false);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                if (!ShowRemote)
                    dba.AddParameter("@BillingTypeID", BillingType.Remote);

                DataSet ds = dba.FillDataSet("RoomApportionmentInDaysMonthly_Select");

                ds.Tables[0].Columns.Add("LineCost", typeof(double));

                //Calculate the true cost based on billing types
                BillingTypeManager.CalculateRoomLineCost(ds.Tables[0]);

                DataTable dt = ds.Tables[0];
                DataTable dtClient = ds.Tables[1];

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]), string.Empty);
                    string desc = ExternalInvoiceUtility.GetRoomDescription(rows[0]);
                    result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
                }

                return result;
            }
        }

        public ExternalInvoiceUsage GetStoreUsage()
        {
            using (var dba = Session.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);
                dba.AddParameter("@IsInternal", false);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                var ds = dba.FillDataSet("StoreBilling_Select");

                DataTable dt = ds.Tables[0];
                DataTable dtClient = ds.Tables[1];

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]));
                    string desc = ExternalInvoiceUtility.GetStoreDescription(rows[0]);
                    result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
                }

                return result;
            }
        }

        public ExternalInvoiceUsage GetMiscUsage()
        {
            using (var dba = Session.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", StartDate);
                dba.AddParameter("@EndPeriod", EndDate);
                dba.AddParameter("@IsInternal", false);

                if (AccountID > 0)
                    dba.AddParameter("@AccountID", AccountID);

                var dt = dba.FillDataTable("MiscBillingCharge_Select");

                dt.Columns.Add("LineCost", typeof(double), "Quantity * Cost");

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dt.Rows)
                {
                    double totalFee = Convert.ToDouble(dr.Field<decimal>("Cost"));
                    string desc = ExternalInvoiceUtility.GetMiscDescription(dr);
                    result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(dr, dr.Field<double>("Quantity"), totalFee, desc));
                }

                return result;
            }
        }

        public IDictionary<string, ExternalInvoiceUsage> GetAllUsage()
        {
            return new Dictionary<string, ExternalInvoiceUsage>
            {
                { "Tool", GetToolUsage() },
                { "Room", GetExternalRoomUsage() },
                { "Store", GetStoreUsage() },
                { "Misc", GetMiscUsage() }
            };
        }


        public IEnumerator<KeyValuePair<string, ExternalInvoiceUsage>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
