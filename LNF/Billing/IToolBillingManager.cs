using System;
using System.Collections.Generic;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;

namespace LNF.Billing
{
    public interface IToolBillingManager : IManager
    {
        void CalculateBookingFee(IToolBilling item);
        void CalculateReservationFee(IToolBilling item);
        void CalculateUsageFeeCharged(IToolBilling item);
        IList<ToolDataRaw> DataFiltered(DateTime sd, DateTime ed, int clientId, int resourceId);
        IList<ToolDataRaw> DataRaw(DateTime period, IEnumerable<Reservation> reservations);
        IList<ToolBilling> ForSUBReport(DateTime StartPeriod, DateTime EndPeriod, ref IList<SubLineItem> lineItems);
        int MinimumDaysForApportionment(ClientOrg co, Room r, DateTime period);
        IList<ToolDataClean> PopulateToolDataClean(DateTime period, IList<Reservation> reservations);
        decimal RatePeriodCharge(IToolBilling item, decimal duration);
        IEnumerable<IToolBilling> SelectToolBilling(DateTime period);
        IEnumerable<IToolBilling> SelectToolBilling(DateTime period, int clientId);
        int UpdateAccountByReservationToolBilling(Reservation rsv);
        int UpdateAccountByReservationToolData(Reservation rsv);
        int UpdateAccountByReservationToolDataClean(Reservation rsv);
        int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period);
        int UpdateChargeMultiplierByReservationToolBilling(Reservation rsv);
        int UpdateChargeMultiplierByReservationToolData(Reservation rsv);
        int UpdateChargeMultiplierByReservationToolDataClean(Reservation rsv);
    }
}