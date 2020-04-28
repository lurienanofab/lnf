using LNF.Billing;
using System;

namespace OnlineServices.Api.Billing
{
    public class OrgRechargeRepository : ApiClient, IOrgRechargeRepository
    {
        public IOrgRecharge AddOrgRecharge(int orgId, int accountId, DateTime createdDate, DateTime endabledDate)
        {
            throw new NotImplementedException();
        }

        public void Disable(int orgRechargeId, DateTime disableDate)
        {
            throw new NotImplementedException();
        }

        public IOrgRecharge GetOrgRecharge(int orgId)
        {
            throw new NotImplementedException();
        }

        public IOrgRecharge GetOrgRecharge(int orgId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }
    }
}
