using LNF.Impl.Repository;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public static class DataAccess
    {
        public static DataSet ToolBillingSelect(NHibernate.ISession session, object parameters) => FillDataSet(session, "dbo.ToolBilling_Select", parameters);

        public static DataSet RoomBillingSelect(NHibernate.ISession session, object parameters) => FillDataSet(session, "dbo.RoomApportionmentInDaysMonthly_Select", parameters);

        public static DataSet StoreBillingSelect(NHibernate.ISession session, object parameters) => FillDataSet(session, "dbo.StoreBilling_Select", parameters);

        public static DataTable ClientAccountSelect(NHibernate.ISession session, object parameters) => FillDataTable(session, "dbo.ClientAccount_Select", parameters);

        public static DataTable AccountSelect(NHibernate.ISession session, object parameters) => FillDataTable(session, "dbo.Account_Select", parameters);

        public static DataTable MiscBillingChargeSelect(NHibernate.ISession session, object parameters) => FillDataTable(session, "dbo.MiscBillingCharge_Select", parameters);

        private static DataSet FillDataSet(NHibernate.ISession session, string proc, object parameters)
        {
            return session.Command()
                .Param(parameters)
                .FillDataSet(proc);
        }

        private static DataTable FillDataTable(NHibernate.ISession session, string proc, object parameters)
        {
            return session.Command()
                .Param(parameters)
                .FillDataTable(proc);
        }
    }
}