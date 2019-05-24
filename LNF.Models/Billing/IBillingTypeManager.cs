using LNF.Models.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Models.Billing
{
    public interface IBillingTypeManager
    {
        IBillingType Default { get; }
        IBillingType ExtAc_Ga { get; }
        IBillingType ExtAc_Hour { get; }
        IBillingType ExtAc_Si { get; }
        IBillingType ExtAc_Tools { get; }
        IBillingType Grower_Observer { get; }
        IBillingType Int_Ga { get; }
        IBillingType Int_Hour { get; }
        IBillingType Int_Si { get; }
        IBillingType Int_Tools { get; }
        IBillingType NonAc { get; }
        IBillingType NonAc_Hour { get; }
        IBillingType NonAc_Tools { get; }
        IBillingType Other { get; }
        IBillingType Regular { get; }
        IBillingType RegularException { get; }
        IBillingType Remote { get; }
        IEnumerable<IBillingType> GetAllBillingTypes();
        void CalculateRoomLineCost(DataTable dt);
        void CalculateToolLineCost(DataTable dt);
        IBillingType Find(int billingTypeId);
        IBillingType Find(string name);
        IBillingType GetBillingType(int clientId, int accountId, DateTime period);
        IBillingType GetBillingType(string text);
        IBillingType GetBillingTypeByClientAndOrg(DateTime period, int clientId, int orgId, IEnumerable<IHoliday> holidays);
        string GetBillingTypeName(IBillingType billingType);
        decimal GetLineCost(IRoomBilling item);
        decimal GetLineCost(IToolBilling item);
        bool IsGrowerUserBillingType(int billingTypeId);
        bool IsMonthlyUserBillingType(int billingTypeId);
        IEnumerable<IToolBilling> SelectToolBillingData(int clientId, DateTime period, bool temp);
        void Update(int clientId, DateTime period);
    }
}