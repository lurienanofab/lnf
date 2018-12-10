using LNF.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Linq;

namespace LNF.Billing
{
    public static class OrgRechargeUtility
    {
        public static IChargeTypeManager ChargeTypeManager => ServiceProvider.Current.Use<IChargeTypeManager>();

        public static Account GetRechargeAccount(Org org, DateTime startDate, DateTime endDate)
        {
            Account result = null;

            OrgRecharge or = DA.Current.Query<OrgRecharge>().FirstOrDefault(x => x.Org == org && x.EnableDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate));

            if (or == null)
                result = ChargeTypeManager.GetAccount(org.OrgType.ChargeType); //no recharge account specified for this org so use the default from ChargeType
            else
                result = or.Account;

            return result;
        }

        public static OrgRecharge Enable(Org org, Account acct)
        {
            //first get the existing entity if there is one
            OrgRecharge existing = DA.Current.Query<OrgRecharge>().FirstOrDefault(x => x.Org == org && x.DisableDate == null);

            if (existing != null)
            {
                //we don't need to do anything if Org and Account are the same
                if (existing.Account == acct)
                    return existing;

                //disable existing
                existing.DisableDate = DateTime.Now;
            }

            //add a new one
            OrgRecharge result = new OrgRecharge()
            {
                Org = org,
                Account = acct,
                CreatedDate = DateTime.Now,
                EnableDate = DateTime.Now,
                DisableDate = null
            };

            DA.Current.Insert(result);

            return result;
        }

        public static void Disable(OrgRecharge item)
        {
            item.DisableDate = DateTime.Now;
        }
    }
}

