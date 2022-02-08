using LNF.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Billing
{
    public interface IBillingTypeRepository
    {
        IEnumerable<IBillingType> GetBillingTypes();
        IBillingType GetBillingType(int billingTypeId);
        IBillingType GetBillingType(int clientId, int accountId, DateTime period);
        IBillingType GetBillingType(DateTime period, int clientId, int orgId, IEnumerable<IHoliday> holidays);
        void CalculateRoomLineCost(DataTable dt);
        void CalculateToolLineCost(DataTable dt);
        decimal GetLineCost(IRoomBilling item);
        IEnumerable<IToolBilling> SelectToolBillingData(int clientId, DateTime period, bool temp);
        void UpdateBilling(int clientId, DateTime period, DateTime now);
        IEnumerable<IClientOrgBillingTypeLog> GetClientOrgBillingTypeLogs(int clientOrgId, DateTime? disableDate);
        IEnumerable<IClientOrgBillingTypeLog> GetActiveClientOrgBillingTypeLogs(DateTime sd, DateTime ed);
        IClientOrgBillingTypeLog GetActiveClientOrgBillingTypeLog(int clientOrgId, DateTime sd, DateTime ed);
        IClientOrgBillingTypeLog UpdateClientOrgBillingTypeLog(int clientOrgId, int billingTypeId);
    }
}