using LNF.Billing.Reports.ServiceUnitBilling;
using LNF.CommonTools;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public class StoreServiceUnitBillingGenerator : ServiceUnitBillingGenerator<StoreSUB>
    {
        private StoreServiceUnitBillingGenerator(StoreSUB report) : base(report) { }

        public static StoreServiceUnitBillingGenerator Create(StoreSUB report)
        {
            return new StoreServiceUnitBillingGenerator(report);
        }

        protected override void GenerateDataTables()
        {
            DataSet ds = null;

            object queryParameters;

            if (Report.TwoCreditAccounts)
            {
                if (Report.ClientID == 0)
                    queryParameters = new { Action = "ForSUBReportWithTwoCreditAccounts", Report.StartPeriod, Report.EndPeriod };
                else
                    queryParameters = new { Action = "ForSUBReportWithTwoCreditAccounts", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

                ds = DataAccess.StoreBillingSelect(Session, queryParameters);

                if (ds == null) return;

                DataTable dtStoreBillingGeneralLab = ds.Tables[0];
                DataTable dtStoreBillingSSELCurrent = ds.Tables[1];

                ClientAccountData = ds.Tables[2];

                ApplyFormula(dtStoreBillingGeneralLab);
                ApplyFilter();
                ApplyMiscCharge(dtStoreBillingGeneralLab, Report.ClientID);

                ProcessTable(dtStoreBillingGeneralLab);

                if (dtStoreBillingSSELCurrent.Rows.Count > 0)
                {
                    //******** for CreditAccount of "SSEL Current", AccountID = 197, ShortCode = 182115, Number = 61835010000216112ADMIN21100U023432
                    string CreditAccountShortCode = Utility.ConvertTo(dtStoreBillingSSELCurrent.Rows[0]["CreditAccountShortCode"], string.Empty);   //this is the SSEL Current's shortcode
                    string CreditAccount = Utility.ConvertTo(dtStoreBillingSSELCurrent.Rows[0]["CreditAccount"], string.Empty);                     //this is the SSEL Current's Number
                    ProcessTable(dtStoreBillingSSELCurrent);
                }
            }
            else
            {
                if (Report.ClientID == 0)
                    queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod };
                else
                    queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

                ds = DataAccess.StoreBillingSelect(Session, queryParameters);

                if (ds == null) return;

                DataTable dtBillingData = ds.Tables[0];

                ClientAccountData = ds.Tables[1];
                ApplyFormula(dtBillingData);
                ApplyFilter();
                ApplyMiscCharge(dtBillingData, Report.ClientID);
                ProcessTable(dtBillingData);
            }
        }
    }
}