using LNF.Repository;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public static class DataAccess
    {
        public static DataSet ToolBillingSelect(object parameters) => FillDataSet("dbo.ToolBilling_Select", parameters);

        public static DataSet RoomBillingSelect(object parameters) => FillDataSet("dbo.RoomApportionmentInDaysMonthly_Select", parameters);

        public static DataSet StoreBillingSelect(object parameters) => FillDataSet("dbo.StoreBilling_Select", parameters);

        public static DataTable ClientAccountSelect(object parameters) => FillDataTable("dbo.ClientAccount_Select", parameters);

        public static DataTable AccountSelect(object parameters) => FillDataTable("dbo.Account_Select", parameters);

        public static DataTable MiscBillingChargeSelect(object parameters) => FillDataTable("dbo.MiscBillingCharge_Select", parameters);

        private static DataSet FillDataSet(string proc, object parameters)
        {
            return DA.Command()
                .Param(parameters)
                .FillDataSet(proc);
        }

        private static DataTable FillDataTable(string proc, object parameters)
        {
            return DA.Command()
                .Param(parameters)
                .FillDataTable(proc);
        }
    }
}