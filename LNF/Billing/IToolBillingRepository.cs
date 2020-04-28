using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IToolBillingRepository
    {
        IEnumerable<IToolBilling> CreateToolBilling(DateTime period, int clientId = 0);
        IEnumerable<IToolBilling> CreateToolBilling(int reservationId);
        IEnumerable<IToolData> CreateToolData(DateTime period, int clientId = 0, int resourceId = 0);
        IEnumerable<IToolData> CreateToolData(int reservationId);
        IEnumerable<IToolBilling> GetToolBilling(DateTime period, int clientId = 0, int resourceId = 0, int accountId = 0);
        IEnumerable<IToolBilling> GetToolBilling(int reservationId);
        IEnumerable<IToolData> GetToolData(DateTime period, int clientId = 0, int resourceId = 0);
        IEnumerable<IToolData> GetToolData(int reservationId);
        IEnumerable<IToolDataClean> GetToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0);
        IToolDataClean GetToolDataClean(int reservationId);
        void CalculateBookingFee(IToolBilling item);
        void CalculateReservationFee(IToolBilling item);
        void CalculateUsageFeeCharged(IToolBilling item);
        IEnumerable<IToolDataRaw> DataFiltered(DateTime sd, DateTime ed, int clientId, int resourceId);
        IEnumerable<IToolDataRaw> DataRaw(DateTime period, IEnumerable<IToolBillingReservation> reservations);
        IEnumerable<IToolBilling> ForSUBReport(DateTime StartPeriod, DateTime EndPeriod, IList<SubLineItem> lineItems);
        int MinimumDaysForApportionment(IClient co, IRoom r, DateTime period);
        decimal RatePeriodCharge(IToolBilling item, decimal duration);
        IEnumerable<IToolBilling> SelectToolBilling(DateTime period);
        IEnumerable<IToolBilling> SelectToolBilling(DateTime period, int clientId);
        int UpdateAccountByReservationToolBilling(IToolBillingReservation rsv);
        int UpdateAccountByReservationToolData(IToolBillingReservation rsv);
        int UpdateAccountByReservationToolDataClean(IToolBillingReservation rsv);
        int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period);
        int UpdateChargeMultiplierByReservationToolBilling(IToolBillingReservation rsv);
        int UpdateChargeMultiplierByReservationToolData(IToolBillingReservation rsv);
        int UpdateChargeMultiplierByReservationToolDataClean(IToolBillingReservation rsv);
        IEnumerable<IToolBillingReservation> SelectReservations(DateTime sd, DateTime ed, int resourceId);
    }
}