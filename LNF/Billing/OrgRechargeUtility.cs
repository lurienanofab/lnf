using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Linq;

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

        public IAccount GetRechargeAccount(Org org, DateTime startDate, DateTime endDate)
        {
            IAccount result = null;

            OrgRecharge or = DA.Current.Query<OrgRecharge>().FirstOrDefault(x => x.Org == org && x.EnableDate < endDate && (x.DisableDate == null || x.DisableDate.Value > startDate));

            if (or == null)
                result = Provider.Data.ChargeTypeManager.GetAccount(org.OrgType.ChargeType.CreateModel<IChargeType>()); //no recharge account specified for this org so use the default from ChargeType
            else
                result = or.Account.CreateModel<IAccount>();

            return result;
        }

        public OrgRecharge Enable(Org org, Account acct)
        {
            //first get the existing entity if there is one
            OrgRecharge existing = DA.Current.Query<OrgRecharge>().FirstOrDefault(x => x.Org == org && x.DisableDate == null);

            if (existing != null)
            {
                //we don't need to do anything if Org and Account are the same
                if (existing.Account == acct)
                    return existing;

                //disable existing
                existing.DisableDate = Now;
            }

            //add a new one
            OrgRecharge result = new OrgRecharge()
            {
                Org = org,
                Account = acct,
                CreatedDate = Now,
                EnableDate = Now,
                DisableDate = null
            };

            DA.Current.Insert(result);

            return result;
        }

        public void Disable(OrgRecharge item)
        {
            item.DisableDate = Now;
        }
    }
}

