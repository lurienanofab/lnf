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

        public UpdateTablesResult UpdateTables(BillingCategory categories, UpdateDataType types = UpdateDataType.DataClean | UpdateDataType.Data)
        {
            DateTime startedAt = DateTime.Now;
            DateTime period = startedAt.FirstOfMonth();

            //First, update tables
            UpdateResult updateResult = Provider.Billing.Process.Update(new UpdateCommand
            {
                BillingTypes = categories,
                UpdateTypes = types,
                Period = period,
                ClientID = 0
            });

            var holidays = Utility.GetHolidays(period, period.AddMonths(1));
            var isFirstBusinessDay = Utility.IsFirstBusinessDay(startedAt, holidays);
            FinalizeResult finalizeResult;

            //the daily update DOES produce correct data
            //however, if a change was made since the update, it needs to be caught 
            if (isFirstBusinessDay)
                finalizeResult = Finalize(period.AddMonths(-1));
            else
                finalizeResult = null;

            var result = new UpdateTablesResult(startedAt)
            {
                Now = startedAt,
                Period = period,
                IsFirstBusinessDay = isFirstBusinessDay,
                FinalizeResult = finalizeResult,
                UpdateResult = updateResult
            };

            return result;
        }

        public FinalizeResult Finalize(DateTime period)
        {
            return Provider.Billing.Process.Finalize(new FinalizeCommand { Period = period });
        }
    }
}
