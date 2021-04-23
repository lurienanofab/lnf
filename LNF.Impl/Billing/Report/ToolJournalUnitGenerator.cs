using LNF.Billing.Reports.ServiceUnitBilling;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public class ToolJournalUnitGenerator : JournalUnitGenerator<ToolJU>
    {
        private ToolJournalUnitGenerator(NHibernate.ISession session, ToolJU report) : base(session, report) { }

        public static ToolJournalUnitGenerator Create(NHibernate.ISession session, ToolJU report)
        {
            return new ToolJournalUnitGenerator(session, report);
        }

        protected override void GenerateDataTables()
        {
            object queryParameters;

            if (Report.ClientID == 0)
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod };
            else
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

            var ds = DataAccess.ToolBillingSelect(Session, queryParameters);

            DataTable dtBillingData = ds.Tables[0];
            ClientAccountData = ds.Tables[1];
            ApplyFormula(dtBillingData);
            ApplyFilter();
            ApplyMiscCharge(dtBillingData, Report.ClientID);
            ProcessTable(dtBillingData);
        }
    }
}