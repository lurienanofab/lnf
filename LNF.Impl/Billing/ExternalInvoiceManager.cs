using LNF.Billing;
using LNF.CommonTools;
using LNF.Impl.Repository;
using LNF.Repository;
using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    internal class ExternalInvoiceManager : IExternalInvoiceManager
    {
        public static readonly string Tool = "Tool";
        public static readonly string Room = "Room";
        public static readonly string Store = "Store";
        public static readonly string Misc = "Misc";

        public int AccountID { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool ShowRemote { get; }
        public bool IncludeAccountsWithNoUsage { get; set; }
        protected IBillingTypeRepository BillingTypeRepository { get; }
        protected ISession Session { get; }
        protected IDataCommand Command(CommandType type = CommandType.StoredProcedure) => Session.Command(type);

        private IDictionary<string, ExternalInvoiceUsage> _data;

        /// <summary>
        /// Generates invoices for all accounts.
        /// </summary>
        public ExternalInvoiceManager(DateTime sd, DateTime ed, bool showRemote, IBillingTypeRepository billingTypeRepository, ISession session)
            : this(0, sd, ed, showRemote, billingTypeRepository, session) { }

        /// <summary>
        /// Generates invoices for a single account.
        /// </summary>
        public ExternalInvoiceManager(int accountId, DateTime sd, DateTime ed, bool showRemote, IBillingTypeRepository billingTypeRepository, ISession session)
        {
            AccountID = accountId;
            StartDate = sd;
            EndDate = ed;
            ShowRemote = showRemote;
            IncludeAccountsWithNoUsage = false;
            BillingTypeRepository = billingTypeRepository;
            Session = session;

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

        public IEnumerable<ExternalInvoice> GetInvoices(int accountId = 0)
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
            return Command()
                .Param("Action", "External")
                .FillDataTable("dbo.ChargeType_Select");
        }

        public DataTable GetActiveExternalOrgs(DateTime sd, DateTime ed)
        {
            var dt = Command()
                .Param(new { Action = "ActiveExternal", sDate = sd, eDate = ed })
                .FillDataTable("dbo.Org_Select");

            dt.PrimaryKey = new[] { dt.Columns["AccountID"] };

            return dt;
        }

        protected void ValidPeriodCheck(DataRow dr, string billingCategory)
        {
            var period = dr.Field<DateTime>("Period");
            var clientId = dr.Field<int>("ClientID");
            var accountId = dr.Field<int>("AccountID");

            // sanity check
            if (period.Day != 1 || period.Hour != 0 || period.Minute != 0 || period.Second != 0)
            {
                SendEmail.SendDeveloperEmail("LNF.Impl.Billing.ExternalInvoiceManager.ValidPeriodCheck", $"Invalid period detected in External Invoice - {billingCategory} Data [run at {DateTime.Now:yyyy-MM-dd HH:mm:ss}]", $"Invalid period used - not midnight or 1st of month. Period = '{period:yyyy-MM-dd HH:mm:ss}', ClientID = {clientId}, AccountID = {accountId}");
                throw new Exception($"Period is not midnight on the 1st of the month. Report: External Invoice - {billingCategory} Data, Period: {period:yyyy-MM-dd HH:mm:ss}, ClientID: {clientId}");
            }
        }

        protected string GetFilter(DataRow dr)
        {
            return $"Period = #{dr["Period"]}# AND ClientID = {dr["ClientID"]} AND AccountID = {dr["AccountID"]}";
        }

        public ExternalInvoiceUsage GetToolUsage()
        {
            var ds = Command()
                .Param("Action", "ForInvoice")
                .Param("StartPeriod", StartDate)
                .Param("EndPeriod", EndDate)
                .Param("IsInternal", false)
                .Param("AccountID", AccountID > 0, AccountID)
                .Param("BillingTypeID", !ShowRemote, BillingTypes.Remote)
                .FillDataSet("dbo.ToolBilling_Select");

            // It will return a dataset with two tables inside
            // table #1: the data table contains individual tool usage
            // table #2: the client table contains all users who has used the tools on this account

            ds.Tables[0].Columns.Add("LineCost", typeof(double));

            //Calculate the true cost based on billing types
            BillingTypeRepository.CalculateToolLineCost(ds.Tables[0]);

            var dt = ds.Tables[0];
            var dtClient = ds.Tables[1];

            var result = new ExternalInvoiceUsage();

            //Aggregate the report based on ClientID
            foreach (DataRow dr in dtClient.Rows)
            {
                ValidPeriodCheck(dr, Tool);
                string filter = GetFilter(dr);
                double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", filter));
                DataRow[] rows = dt.Select(filter);
                string desc = ExternalInvoiceUtility.GetToolDescription(rows[0]);
                result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
            }

            return result;
        }

        public ExternalInvoiceUsage GetExternalRoomUsage()
        {
            var ds = Command()
                .Param("Action", "ForInvoice")
                .Param("StartPeriod", StartDate)
                .Param("EndPeriod", EndDate)
                .Param("IsInternal", false)
                .Param("AccountID", AccountID > 0, AccountID)
                .Param("BillingTypeID", !ShowRemote, BillingTypes.Remote)
                .FillDataSet("dbo.RoomApportionmentInDaysMonthly_Select");

            ds.Tables[0].Columns.Add("LineCost", typeof(double));

            //Calculate the true cost based on billing types
            BillingTypeRepository.CalculateRoomLineCost(ds.Tables[0]);

            DataTable dt = ds.Tables[0];
            DataTable dtClient = ds.Tables[1];

            var result = new ExternalInvoiceUsage();

            //Aggregate the report based on ClientID
            foreach (DataRow dr in dtClient.Rows)
            {
                ValidPeriodCheck(dr, Room);
                string filter = GetFilter(dr);
                double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", filter));
                DataRow[] rows = dt.Select(filter);
                string desc = ExternalInvoiceUtility.GetRoomDescription(rows[0]);
                result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
            }

            return result;
        }

        public ExternalInvoiceUsage GetStoreUsage()
        {
            var ds = Command()
                .Param("Action", "ForInvoice")
                .Param("StartPeriod", StartDate)
                .Param("EndPeriod", EndDate)
                .Param("IsInternal", false)
                .Param("AccountID", AccountID > 0, AccountID)
                .FillDataSet("dbo.StoreBilling_Select");

            var dt = ds.Tables[0];
            var dtClient = ds.Tables[1];

            var result = new ExternalInvoiceUsage();

            //Aggregate the report based on ClientID
            foreach (DataRow dr in dtClient.Rows)
            {
                ValidPeriodCheck(dr, Store);
                string filter = GetFilter(dr);
                double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", filter));
                DataRow[] rows = dt.Select(filter);
                string desc = ExternalInvoiceUtility.GetStoreDescription(rows[0]);
                result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
            }

            return result;
        }

        public ExternalInvoiceUsage GetMiscUsage()
        {
            var dt = Command()
                .Param("Action", "ForInvoice")
                .Param("StartPeriod", StartDate)
                .Param("EndPeriod", EndDate)
                .Param("IsInternal", false)
                .Param("AccountID", AccountID > 0, AccountID)
                .FillDataTable("dbo.MiscBillingCharge_Select");

            dt.Columns.Add("LineCost", typeof(double), "Quantity * Cost");

            var result = new ExternalInvoiceUsage();

            //Aggregate the report based on ClientID
            foreach (DataRow dr in dt.Rows)
            {
                ValidPeriodCheck(dr, Misc);
                double totalFee = Convert.ToDouble(dr.Field<decimal>("Cost"));
                string desc = ExternalInvoiceUtility.GetMiscDescription(dr);
                result.Add(ExternalInvoiceUtility.CreateInvoiceLineItem(dr, dr.Field<double>("Quantity"), totalFee, desc));
            }

            return result;
        }

        public IDictionary<string, ExternalInvoiceUsage> GetAllUsage()
        {
            return new Dictionary<string, ExternalInvoiceUsage>
            {
                { Tool, GetToolUsage() },
                { Room, GetExternalRoomUsage() },
                { Store, GetStoreUsage() },
                { Misc, GetMiscUsage() }
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
