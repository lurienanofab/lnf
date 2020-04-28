using LNF.Billing.Reports.ServiceUnitBilling;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public class RoomJournalUnitGenerator : JournalUnitGenerator<RoomJU>
    {
        private readonly NHibernate.ISession _session;

        private RoomJournalUnitGenerator(NHibernate.ISession session, RoomJU report) : base(report)
        {
            _session = session;
        }

        public static RoomJournalUnitGenerator Create(NHibernate.ISession session, RoomJU report)
        {
            return new RoomJournalUnitGenerator(session, report);
        }

        protected override void GenerateDataTables()
        {
            object queryParameters;

            if (Report.ClientID == 0)
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod };
            else
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

            var ds = DataAccess.RoomBillingSelect(_session, queryParameters);

            DataTable dtBillingData = ds.Tables[0];
            ClientAccountData = ds.Tables[1];
            ApplyFormula(dtBillingData);
            ApplyFilter();
            ApplyMiscCharge(dtBillingData, Report.ClientID);
            ProcessTable(dtBillingData);
        }
    }
}