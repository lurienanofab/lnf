using LNF.Billing;
using LNF.Billing.Process;
using System;

namespace LNF.CommonTools
{
    public class WriteData
    {
        public IProvider Provider { get; }

        public WriteData(IProvider provider)
        {
            Provider = provider;
        }

        public UpdateTablesResult UpdateTables(BillingCategory types)
        {
            DateTime now = DateTime.Now;
            DateTime period = now.FirstOfMonth();

            var holidays = Utility.GetHolidays(period, period.AddMonths(1));

            var result = new UpdateTablesResult
            {
                Now = now,
                Period = period,
                IsFirstBusinessDay = Utility.IsFirstBusinessDay(now, holidays),
                //First, update tables
                UpdateResult = Provider.Billing.Process.Update(new UpdateCommand
                {
                    BillingTypes = types,
                    UpdateTypes = UpdateDataType.DataClean | UpdateDataType.Data,
                    Period = period,
                    ClientID = 0
                })
            };

            //the daily update DOES produce correct data
            //however, if a change was made since the update, it needs to be caught 
            if (result.IsFirstBusinessDay)
                result.FinalizeResult = Finalize(period.AddMonths(-1));

            return result;
        }

        public FinalizeResult Finalize(DateTime period)
        {
            return Provider.Billing.Process.Finalize(new FinalizeCommand { Period = period });
        }
    }
}
