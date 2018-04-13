using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Threading.Tasks;

namespace LNF.Scheduler
{
    public class FinOpsManager : ManagerBase
    {
        public ToolBillingManager ToolBillingManager { get; }

        public FinOpsManager(ISession session, ToolBillingManager toolBillingMgr) : base(session)
        {
            ToolBillingManager = toolBillingMgr;
        }

        [Obsolete("Use webapi from now on.")]
        public bool UpdateForgivenChargeOnFinOps(Reservation rsv)
        {
            ToolBillingManager.UpdateChargeMultiplierByReservationToolDataClean(rsv);
            ToolBillingManager.UpdateChargeMultiplierByReservationToolData(rsv);
            ToolBillingManager.UpdateChargeMultiplierByReservationToolBilling(rsv);
            return true;
        }

        [Obsolete("Use webapi from now on.")]
        public Task<bool> UpdateAccountOnFinOps(Reservation rsv)
        {
            throw new NotImplementedException();

            //using (var service = Providers.Service.NewSchedulerService())
            //{
            //    Exception exception = null;

            //    bool result = true;
            //    try
            //    {
            //        List<DateTime> periods = new List<DateTime>();
            //        var chargeBeginDateTime = rsv.ChargeBeginDateTime();
            //        var chargeEndDateTime = rsv.ChargeEndDateTime();

            //        DateTime p = new DateTime(chargeBeginDateTime.Year, chargeBeginDateTime.Month, 1);
            //        DateTime endPeriod = new DateTime(chargeEndDateTime.Year, chargeEndDateTime.Month, 1);

            //        while (p <= endPeriod)
            //        {
            //            periods.Add(p);
            //            p = p.AddMonths(1);
            //        }

            //        var wd = new WriteData();

            //        foreach (var period in periods)
            //        {
            //            bool isTemp = Utility.IsCurrentPeriod(period);

            //            await service.PopulateDataClean(new PopulateDataOptions() { Type = BillingProcessType.Tool, StartDate = period, EndDate = period.AddMonths(1), ClientID = rsv.Client.ClientID, Record = 0 });
            //            await service.PopulateData(new PopulateDataOptions() { Type = BillingProcessType.Tool, StartDate = period, EndDate = period.AddMonths(1), ClientID = rsv.Client.ClientID, Record = 0 });

            //            await service.PopulateDataClean(new PopulateDataOptions() { Type = BillingProcessType.Room, StartDate = period, EndDate = period.AddMonths(1), ClientID = rsv.Client.ClientID, Record = 0 });
            //            await service.PopulateData(new PopulateDataOptions() { Type = BillingProcessType.Room, StartDate = period, EndDate = period.AddMonths(1), ClientID = rsv.Client.ClientID, Record = 0 });

            //            await service.BillingProcessStep1(new BillingProcessStep1Options() { Type = BillingProcessType.Tool, Period = period, IsTemp = isTemp });
            //            await service.BillingProcessStep1(new BillingProcessStep1Options() { Type = BillingProcessType.Room, Period = period, IsTemp = isTemp });

            //            await service.BillingProcessStep4(new BillingProcessStep4Options() { Period = period, ClientID = rsv.Client.ClientID });

            //            if (!isTemp)
            //            {
            //                await service.BillingProcessStep2(new BillingProcessStep2Options() { Type = BillingProcessType.Tool, Period = period, ClientID = rsv.Client.ClientID });
            //                await service.BillingProcessStep3(new BillingProcessStep3Options() { Type = BillingProcessType.Tool, Period = period, ClientID = rsv.Client.ClientID });

            //                await service.BillingProcessStep2(new BillingProcessStep2Options() { Type = BillingProcessType.Room, Period = period, ClientID = rsv.Client.ClientID });
            //                await service.BillingProcessStep3(new BillingProcessStep3Options() { Type = BillingProcessType.Room, Period = period, ClientID = rsv.Client.ClientID });

            //                await service.BillingProcessStep4(new BillingProcessStep4Options() { Period = period, ClientID = rsv.Client.ClientID });
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        exception = ex;
            //        result = false;
            //    }

            //    return result;
            //}
        }
    }
}
