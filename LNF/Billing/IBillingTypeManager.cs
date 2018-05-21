using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Billing
{
    public interface IBillingTypeManager : IManager
    {
        BillingType Default { get; }
        BillingType ExtAc_Ga { get; }
        BillingType ExtAc_Hour { get; }
        BillingType ExtAc_Si { get; }
        BillingType ExtAc_Tools { get; }
        BillingType Grower_Observer { get; }
        BillingType Int_Ga { get; }
        BillingType Int_Hour { get; }
        BillingType Int_Si { get; }
        BillingType Int_Tools { get; }
        BillingType NonAc { get; }
        BillingType NonAc_Hour { get; }
        BillingType NonAc_Tools { get; }
        BillingType Other { get; }
        BillingType Regular { get; }
        BillingType RegularException { get; }
        BillingType Remote { get; }
        IList<BillingType> GetAllBillingTypes();
        void CalculateRoomLineCost(DataTable dt);
        void CalculateToolLineCost(DataTable dt);
        BillingType Find(int billingTypeId);
        BillingType Find(string name);
        BillingType GetBillingType(Client client, Account account, DateTime period);
        BillingType GetBillingType(string text);
        BillingType GetBillingTypeByClientAndOrg(DateTime period, Client client, Org org);
        string GetBillingTypeName(BillingType billingType);
        decimal GetLineCost(IRoomBilling item);
        decimal GetLineCost(IToolBilling item);
        bool IsGrowerUserBillingType(int billingTypeId);
        bool IsMonthlyUserBillingType(int billingTypeId);
        IList<IToolBilling> SelectToolBillingData<T>(Client client, DateTime period) where T : IToolBilling;
        void Update(Client client, DateTime period);
    }
}