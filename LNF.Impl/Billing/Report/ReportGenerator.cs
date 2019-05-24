using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Billing.Reports.ServiceUnitBilling;
using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing.Report
{
    public abstract class ReportGenerator<T> where T : ReportBase
    {
        private DataTable dtManagers;
        protected IBillingTypeManager BillingTypeManager => ServiceProvider.Current.Billing.BillingType;
        protected List<DataTable> _ReportTables = new List<DataTable>();
        protected List<DataView> _ReportViews = new List<DataView>();
        protected string _CreditAccount;
        protected string _CreditAccountShortCode;
        protected DataTable ClientAccountData { get; set; }

        protected T Report { get; private set; }

        protected DataTable ManagersData
        {
            get
            {
                if (dtManagers == null)
                    dtManagers = DataAccess.ClientAccountSelect(new { Action = "AllWithManagerName", sDate = Report.StartPeriod, eDate = Report.EndPeriod });
                return dtManagers;
            }
        }

        public string CreditAccount { get { return _CreditAccount; } }
        public string CreditAccountShortCode { get { return _CreditAccountShortCode; } }

        public ReportGenerator(T report)
        {
            Report = report;

            var gc = DA.Current.Query<GlobalCost>().First();

            _CreditAccountShortCode = gc.LabCreditAccount.ShortCode;

            switch (Report.ReportType)
            {
                case ReportTypes.JU:
                    _CreditAccount = gc.SubsidyCreditAccount.Number;
                    break;
                case ReportTypes.SUB:
                    _CreditAccount = gc.LabCreditAccount.Number;
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

        protected string DataRowFilter(DataRow dr)
        {
            return string.Format("Period = '{0}' AND ClientID = {1} AND AccountID = {2}", dr["Period"], dr["ClientID"], dr["AccountID"]);
        }

        protected void ApplyFilter()
        {
            ReportUtility.ApplyFilter(ClientAccountData, Report.BillingCategory);
        }

        protected void ApplyFormula(DataTable dt)
        {
            switch (Report.BillingCategory)
            {
                case BillingCategory.Tool:
                    BillingTypeManager.CalculateToolLineCost(dt);
                    break;
                case BillingCategory.Room:
                    BillingTypeManager.CalculateRoomLineCost(dt);
                    break;
                case BillingCategory.Store:
                    //do nothing
                    break;
            }
        }

        protected void ApplyMiscCharge(DataTable dt, int id)
        {
            ReportUtility.ApplyMiscCharge(dt, ClientAccountData, Report.StartPeriod, Report.EndPeriod, Report.BillingCategory, id);
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