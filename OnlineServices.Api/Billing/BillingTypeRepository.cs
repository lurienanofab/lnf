using LNF.Billing;
using LNF.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineServices.Api.Billing
{
    public class BillingTypeRepository : ApiClient, IBillingTypeRepository
    {
        public IEnumerable<IBillingType> GetBillingTypes()
        {
            throw new NotImplementedException();
        }

        public IBillingType GetBillingType(int billingTypeId)
        {
            throw new NotImplementedException();
        }

        public IBillingType GetBillingType(int clientId, int accountId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public IBillingType GetBillingType(DateTime period, int clientId, int orgId, IEnumerable<IHoliday> holidays)
        {
            throw new NotImplementedException();
        }

        public void CalculateRoomLineCost(DataTable dt)
        {
            throw new NotImplementedException();
        }

        public void CalculateToolLineCost(DataTable dt)
        {
            throw new NotImplementedException();
        }

        public IClientOrgBillingTypeLog GetActiveClientOrgBillingTypeLog(int clientOrgId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientOrgBillingTypeLog> GetActiveClientOrgBillingTypeLogs(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientOrgBillingTypeLog> GetClientOrgBillingTypeLogs(int clientOrgId, DateTime? disableDate)
        {
            throw new NotImplementedException();
        }

        public decimal GetLineCost(IRoomBilling item)
        {
            throw new NotImplementedException();
        }

        public decimal GetLineCost(IToolBilling item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> SelectToolBillingData(int clientId, DateTime period, bool temp)
        {
            throw new NotImplementedException();
        }

        public void UpdateBilling(int clientId, DateTime period, DateTime now)
        {
            throw new NotImplementedException();
        }

        public void UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public IClientOrgBillingTypeLog UpdateClientOrgBillingTypeLog(int clientOrgId, int billingTypeId)
        {
            throw new NotImplementedException();
        }
    }
}
