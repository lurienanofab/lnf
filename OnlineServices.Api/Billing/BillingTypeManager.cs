using LNF.Models.Billing;
using LNF.Models.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineServices.Api.Billing
{
    public class BillingTypeManager : ApiClient, IBillingTypeManager
    {
        public IBillingType Default => throw new NotImplementedException();

        public IBillingType ExtAc_Ga => throw new NotImplementedException();

        public IBillingType ExtAc_Hour => throw new NotImplementedException();

        public IBillingType ExtAc_Si => throw new NotImplementedException();

        public IBillingType ExtAc_Tools => throw new NotImplementedException();

        public IBillingType Grower_Observer => throw new NotImplementedException();

        public IBillingType Int_Ga => throw new NotImplementedException();

        public IBillingType Int_Hour => throw new NotImplementedException();

        public IBillingType Int_Si => throw new NotImplementedException();

        public IBillingType Int_Tools => throw new NotImplementedException();

        public IBillingType NonAc => throw new NotImplementedException();

        public IBillingType NonAc_Hour => throw new NotImplementedException();

        public IBillingType NonAc_Tools => throw new NotImplementedException();

        public IBillingType Other => throw new NotImplementedException();

        public IBillingType Regular => throw new NotImplementedException();

        public IBillingType RegularException => throw new NotImplementedException();

        public IBillingType Remote => throw new NotImplementedException();

        public void CalculateRoomLineCost(DataTable dt)
        {
            throw new NotImplementedException();
        }

        public void CalculateToolLineCost(DataTable dt)
        {
            throw new NotImplementedException();
        }

        public IBillingType Find(int billingTypeId)
        {
            throw new NotImplementedException();
        }

        public IBillingType Find(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IBillingType> GetAllBillingTypes()
        {
            throw new NotImplementedException();
        }

        public IBillingType GetBillingType(int clientId, int accountId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public IBillingType GetBillingType(string text)
        {
            throw new NotImplementedException();
        }

        public IBillingType GetBillingTypeByClientAndOrg(DateTime period, int clientId, int orgId, IEnumerable<IHoliday> holidays)
        {
            throw new NotImplementedException();
        }

        public string GetBillingTypeName(IBillingType billingType)
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

        public bool IsGrowerUserBillingType(int billingTypeId)
        {
            throw new NotImplementedException();
        }

        public bool IsMonthlyUserBillingType(int billingTypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> SelectToolBillingData(int clientId, DateTime period, bool temp)
        {
            throw new NotImplementedException();
        }

        public void Update(int clientId, DateTime period)
        {
            throw new NotImplementedException();
        }
    }
}
