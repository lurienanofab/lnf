using System;

namespace LNF.Billing
{
    public interface IOrgRechargeRepository
    {
        IOrgRecharge GetOrgRecharge(int orgId);
        IOrgRecharge GetOrgRecharge(int orgId, DateTime sd, DateTime ed);
        IOrgRecharge AddOrgRecharge(int orgId, int accountId, DateTime createdDate, DateTime endabledDate);
        void Disable(int orgRechargeId, DateTime disableDate);
    }
}
