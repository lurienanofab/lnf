using LNF.Billing;
using LNF.Billing.Process;
using System;

namespace LNF.CommonTools
{
    //TODO: add a function to end all reservations that ran past the end of the month
    //to this on the fourth business day
    //also, check that res.isactive=0 has null for act times, etc

    public static class DataTableManager
    {
        private readonly static WriteData _wd;

        static DataTableManager()
        {
            _wd = new WriteData(ServiceProvider.Current);
        }

        public static UpdateTablesResult Update(BillingCategory types)
        {
            return _wd.UpdateTables(types);
        }

        public static FinalizeResult Finalize(DateTime period)
        {
            return _wd.Finalize(period);
        }
    }
}
