using LNF.Models.Billing.Reports.ServiceUnitBilling;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public class ToolServiceUnitBillingGenerator : ServiceUnitBillingGenerator<ToolSUB>
    {
        private ToolServiceUnitBillingGenerator(ToolSUB report) : base(report) { }

        public static ToolServiceUnitBillingGenerator Create(ToolSUB report)
        {
            return new ToolServiceUnitBillingGenerator(report);
        }

        protected override void GenerateDataTables()
        {
            object queryParameters;

            if (Report.ClientID == 0)
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod };
            else
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

            DataSet ds = DataAccess.ToolBillingSelect(queryParameters);

            DataTable dtBillingData = ds.Tables[0];
            ClientAccountData = ds.Tables[1];
            ApplyFormula(dtBillingData);
            ApplyFilter();
            ApplyMiscCharge(dtBillingData, Report.ClientID);
            ProcessTable(dtBillingData);
        }
    }
}