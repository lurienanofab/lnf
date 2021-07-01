using LNF.Billing;
using LNF.Billing.Reports.ServiceUnitBilling;
using LNF.CommonTools;
using LNF.Data;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public abstract class ReportGenerator<T> where T : ReportBase
    {
        private DataTable dtManagers;
        protected List<DataTable> _ReportTables = new List<DataTable>();
        protected List<DataView> _ReportViews = new List<DataView>();
        protected string _CreditAccount;
        protected string _CreditAccountShortCode;

        protected ISession Session { get; }
        protected IGlobalCost GlobalCost { get; }
        protected DataTable ClientAccountData { get; set; }

        protected T Report { get; }

        public ReportGenerator(ISession session)
        {
            Session = session;
        }

        protected DataTable ManagersData
        {
            get
            {
                if (dtManagers == null)
                {
                    dtManagers = DataAccess.ClientAccountSelect(Session, new { Action = "AllWithManagerName", sDate = Report.StartPeriod, eDate = Report.EndPeriod });
                }

                return dtManagers;
            }
        }

        public string CreditAccount { get { return _CreditAccount; } }
        public string CreditAccountShortCode { get { return _CreditAccountShortCode; } }

        public ReportGenerator(ISession session, T report)
        {
            Session = session;
            Report = report;

            GlobalCost = ServiceProvider.Current.Data.Cost.GetActiveGlobalCost();

            _CreditAccountShortCode = GlobalCost.LabCreditAccountShortCode;

            switch (Report.ReportType)
            {
                case ReportTypes.JU:
                    _CreditAccount = GlobalCost.SubsidyCreditAccountNumber;
                    break;
                case ReportTypes.SUB:
                    _CreditAccount = GlobalCost.LabCreditAccountNumber;
                    break;
            }
        }

        protected abstract void LoadReportItems(DataView dv);
        protected abstract void ProcessTable(DataTable dtBilling);
        protected abstract DataTable InitTable();
        protected abstract void GenerateDataTables();

        public void Generate()
        {
            GenerateDataTables();
            GenerateViews();
        }

        protected virtual void GenerateViews()
        {
            foreach (DataTable dt in _ReportTables)
            {
                DataView dv = dt.DefaultView;
                dv.Sort = "ItemDescription";
                _ReportViews.Add(dv);
                LoadReportItems(dv);
            }
        }

        protected void ValidPeriodCheck(DataRow dr)
        {
            var period = dr.Field<DateTime>("Period");
            var clientId = dr.Field<int>("ClientID");

            // sanity check
            if (period.Day != 1 || period.Hour != 0 || period.Minute != 0 || period.Second != 0)
            {
                SendEmail.SendDeveloperEmail("LNF.Impl.Billing.Report.ReportGenerator<T>.ValidPeriodCheck", $"Invalid period detected in {Report.ReportName} [run at {DateTime.Now:yyyy-MM-dd HH:mm:ss}]", $"Invalid period used - not midnight or 1st of month. Period = '{period:yyyy-MM-dd HH:mm:ss}', ClientID = {clientId}");
                //throw new Exception($"Period is not midnight on the 1st of the month. Report: {Report.ReportName}, Period: {period:yyyy-MM-dd HH:mm:ss}, ClientID: {clientId}");
            }
        }

        protected string DataRowFilter(DataRow dr)
        {
            return string.Format("Period = #{0}# AND ClientID = {1} AND AccountID = {2}", dr["Period"], dr["ClientID"], dr["AccountID"]);
        }

        protected void ApplyFilter()
        {
            ReportUtility.ApplyFilter(Session, ClientAccountData, Report.BillingCategory);
        }

        protected void ApplyFormula(DataTable dt)
        {
            switch (Report.BillingCategory)
            {
                case BillingCategory.Tool:
                    ToolBillingUtility.CalculateToolLineCost(dt);
                    break;
                case BillingCategory.Room:
                    RoomBillingUtility.CalculateRoomLineCost(dt);
                    break;
                case BillingCategory.Store:
                    //do nothing
                    break;
            }
        }

        protected void ApplyMiscCharge(DataTable dt, int id)
        {
            ReportUtility.ApplyMiscCharge(Session, dt, ClientAccountData, Report.StartPeriod, Report.EndPeriod, Report.BillingCategory, id);
        }

        protected string ManagerName(DataRow dr)
        {
            string key = string.Empty;
            string notFoundText = string.Empty;
            switch (Report.ReportType)
            {
                case ReportTypes.JU:
                    key = "ManagerUniqueName";
                    notFoundText = "Not Found";
                    break;
                case ReportTypes.SUB:
                    key = "ManagerName";
                    notFoundText = "Manager Not Found";
                    break;
            }

            string result = string.Empty;

            DataRow[] drManagers = ManagersData.Select(string.Format("AccountID = {0}", dr["AccountID"]));

            if (!string.IsNullOrEmpty(key) && ManagersData.Columns.Contains(key))
            {
                if (drManagers.Length > 0)
                    result = Utility.ConvertTo(drManagers[0][key], "[unknown]");
                else
                    result = notFoundText;
            }
            else
            {
                result = "column does not exist: " + key;
            }

            return result;
        }

        protected virtual string GetItemDescription(DataRow dr, string prefix = null)
        {
            // The goal is a string with max length 30.
            // prefix   :  6
            // space    :  1
            // name     : 18
            // hyphens  :  2
            // btype    :  3
            // 6 + 1 + 18 + 2 + 3 = 30

            // [2018-04-18 jg] As of now return the same string for Room, Tool, and Store charges.
            //  Also the name + hyphens + btype length (23) should be the same for both SUB and JU reports

            string displayName = Utility.ConvertTo(dr["DisplayName"], string.Empty);
            string billingTypeName = string.Empty;

            if (dr.Table.Columns.Contains("BillingTypeName"))
                billingTypeName = "--" + Utility.ConvertTo(dr["BillingTypeName"], string.Empty);

            string result = ReportUtility.ClipText(prefix, 6) + " "
                + ReportUtility.ClipText(displayName, 18)
                + ReportUtility.ClipText(billingTypeName, 5);

            return result.Trim();
        }
    }
}