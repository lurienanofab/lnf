using LNF.Billing.Reports.ServiceUnitBilling;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public class ToolServiceUnitBillingGenerator : ServiceUnitBillingGenerator<ToolSUB>
    {
        private readonly NHibernate.ISession _session;

        private ToolServiceUnitBillingGenerator(NHibernate.ISession session, ToolSUB report) : base(report)
        {
            _session = session;
        }

        public static ToolServiceUnitBillingGenerator Create(NHibernate.ISession session, ToolSUB report)
        {
            return new ToolServiceUnitBillingGenerator(session, report);
        }

        protected override void GenerateDataTables()
        {
            object queryParameters;

            if (Report.ClientID == 0)
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod };
            else
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

            DataSet ds = DataAccess.ToolBillingSelect(_session, queryParameters);

            DataTable dtBillingData = ds.Tables[0];
            ClientAccountData = ds.Tables[1];
            ApplyFormula(dtBillingData);
            ApplyFilter();
            ApplyMiscCharge(dtBillingData, Report.ClientID);
            ProcessTable(dtBillingData);
        }
    }
}