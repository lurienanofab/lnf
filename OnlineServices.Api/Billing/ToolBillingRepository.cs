using LNF.Billing;
using LNF.Data;
using LNF.Scheduler;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class ToolBillingRepository : ApiClient, IToolBillingRepository
    {
        internal ToolBillingRepository(IRestClient rc) : base(rc) { }

        public void CalculateBookingFee(IToolBilling item)
        {
            throw new NotImplementedException();
        }

        public void CalculateReservationFee(IToolBilling item)
        {
            throw new NotImplementedException();
        }

        public void CalculateUsageFeeCharged(IToolBilling item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> CreateToolBilling(DateTime period, int clientId = 0)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool/create", QueryStrings(new { period, clientId }));
        }

        public IEnumerable<IToolBilling> CreateToolBilling(int reservationId)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool/create/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<IToolData> CreateToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data/create", QueryStrings(new { period, clientId, resourceId }));
        }

        public IEnumerable<IToolData> CreateToolData(int reservationId)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data/create/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<IToolDataRaw> DataFiltered(DateTime sd, DateTime ed, int clientId, int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolDataRaw> DataRaw(DateTime period, IEnumerable<IReservation> reservations)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolDataRaw> DataRaw(DateTime period, IEnumerable<IToolBillingReservation> reservations)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> ForSUBReport(DateTime StartPeriod, DateTime EndPeriod, IList<SubLineItem> lineItems)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> GetToolBilling(DateTime period, int clientId = 0, int resourceId = 0, int accountId = 0)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool", QueryStrings(new { period, clientId, resourceId, accountId }));
        }

        public IEnumerable<IToolBilling> GetToolBilling(int reservationId)
        {
            return Get<List<ToolBillingItem>>("webapi/billing/tool/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<IToolData> GetToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data", QueryStrings(new { period, clientId, resourceId }));
        }

        public IEnumerable<IToolData> GetToolData(int reservationId)
        {
            return Get<List<ToolDataItem>>("webapi/billing/tool/data/{reservationId}", UrlSegments(new { reservationId }));
        }

        public IEnumerable<IToolDataClean> GetToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0)
        {
            return Get<List<ToolDataCleanItem>>("webapi/billing/tool/data/clean", QueryStrings(new { sd, ed, clientId, resourceId }));
        }

        public IToolDataClean GetToolDataClean(int reservationId)
        {
            return Get<ToolDataCleanItem>("webapi/billing/tool/data/clean/{reservationId}", UrlSegments(new { reservationId }));
        }

        public int MinimumDaysForApportionment(IClient co, IRoom r, DateTime period)
        {
            throw new NotImplementedException();
        }

        public decimal RatePeriodCharge(IToolBilling item, decimal duration)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBillingReservation> SelectReservations(DateTime startDate, DateTime endDate, int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> SelectToolBilling(DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> SelectToolBilling(DateTime period, int clientId)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolBilling(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolBilling(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolData(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolData(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolDataClean(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolDataClean(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolBilling(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolBilling(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolData(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolData(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolDataClean(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolDataClean(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }
    }
}
