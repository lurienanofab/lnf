using LNF.Billing;
using LNF.Billing.Process;
using System;

namespace LNF.CommonTools
{
    //TODO: add a function to end all reservations that ran past the end of the month
    //to this on the fourth business day
    //also, check that res.isactive=0 has null for act times, etc

    public class DataTableManager
    {
        private DataTableManager(IProvider provider)
        {
            _wd = new WriteData(provider);
        }

        public static DataTableManager Create(IProvider provider)
        {
            return new DataTableManager(provider);
        }

        private readonly WriteData _wd;

        /// <summary>
        /// Loads DataClean and Data tables with latest Tool, Room, and Store data. This does not load the Billing tables.
        /// </summary>
        public UpdateTablesResult Update(BillingCategory types)
        {
            return _wd.UpdateTables(types);
        }

        public FinalizeResult Finalize(DateTime period)
        {
            return _wd.Finalize(period);
        }
    }
}
