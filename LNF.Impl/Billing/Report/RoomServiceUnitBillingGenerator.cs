using LNF.Billing.Reports.ServiceUnitBilling;
using NHibernate;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public class RoomServiceUnitBillingGenerator : ServiceUnitBillingGenerator<RoomSUB>
    {
        private RoomServiceUnitBillingGenerator(ISession session, RoomSUB report) : base(session, report) { }

        public static RoomServiceUnitBillingGenerator Create(ISession session, RoomSUB report)
        {
            return new RoomServiceUnitBillingGenerator(session, report);
        }

        protected override void GenerateDataTables()
        {
            object queryParameters;

            if (Report.ClientID == 0)
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod };
            else
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

            var ds = DataAccess.RoomBillingSelect(Session, queryParameters);

            DataTable dtBillingData = ds.Tables[0];
            dtBillingData.Columns.Add("LineCost", typeof(double));
            ClientAccountData = ds.Tables[1];
            ApplyFormula(dtBillingData);
            ApplyFilter();
            ApplyMiscCharge(dtBillingData, Report.ClientID);
            ProcessTable(dtBillingData);
        }
    }
}