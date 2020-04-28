using LNF.Data;
using System;

namespace LNF.Billing
{
    public class OrgRechargeUtility
    {
        public OrgRechargeUtility(DateTime now, IProvider provider)
        {
            Now = now;
            Provider = provider;
        }

        public DateTime Now { get; }
        public IProvider Provider { get; }

        public IAccount GetRechargeAccount(IOrg org, DateTime startDate, DateTime endDate)
        {
            IAccount result = null;

            var or = Provider.Billing.OrgRecharge.GetOrgRecharge(org.OrgID, startDate, endDate);

            if (or == null)
            {
                IChargeType ct = Provider.Data.Org.GetChargeType(org.ChargeTypeID);
                result = Provider.Data.Account.GetAccount(ct.AccountID); //no recharge account specified for this org so use the default from ChargeType
            }
            else
            {
                result = Provider.Data.Account.GetAccount(or.AccountID);
            }

            return result;
        }

        public IOrgRecharge Enable(IOrg org, IAccount acct)
        {
            //first get the existing entity if there is one
            var existing = Provider.Billing.OrgRecharge.GetOrgRecharge(org.OrgID);

            if (existing != null)
            {
                //we don't need to do anything if Org and Account are the same
                if (existing.AccountID == acct.AccountID)
                    return existing;

                //disable existing
                existing.DisableDate = Now;
            }

            //add a new one
            var result = Provider.Billing.OrgRecharge.AddOrgRecharge(org.OrgID, acct.AccountID, Now, Now);

            return result;
        }

        public void Disable(int orgRechargeId)
        {
            Provider.Billing.OrgRecharge.Disable(orgRechargeId, Now);
        }
    }
}

