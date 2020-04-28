using LNF.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class MiscBillingByAccount
    {
        public DateTime Period { get; }
        public IClient Client { get; }
        public IAccount Account { get; }
        public IBillingType BillingType { get; }
        public BillingCategory BillingCategory { get; }
        public decimal TotalMisc { get; }
        public decimal TotalMiscSubsidyDiscount { get; }

        private MiscBillingByAccount(DateTime period, IClient client, IAccount acct, IBillingType bt, BillingCategory bc, decimal totalMisc, decimal totalSubsidy)
        {
            Period = period;
            Client = client;
            Account = acct;
            BillingType = bt;
            BillingCategory = bc;
            TotalMisc = totalMisc;
            TotalMiscSubsidyDiscount = totalSubsidy;
        }

        public IEnumerable<MiscBillingByAccount> Create(IEnumerable<IMiscBillingCharge> source, IEnumerable<IHoliday> holidays, IBillingTypeRepository mgr)
        {
            return source.GroupBy(x => new MiscBillingGroupByKeySelector(x))
                .Select(x => CreateMiscBillingByAccount(x, holidays, mgr))
                .OrderBy(x => x.Period)
                .ThenBy(x => x.Client.ClientID)
                .ThenBy(x => x.Account.AccountID)
                .ThenBy(x => x.BillingCategory)
                .ToArray();
        }

        private MiscBillingByAccount CreateMiscBillingByAccount(IGrouping<MiscBillingGroupByKeySelector, IMiscBillingCharge> grp, IEnumerable<IHoliday> holidays, IBillingTypeRepository mgr)
        {
            var period = grp.Key.Period;
            var clientId = grp.Key.ClientID;
            var accountId = grp.Key.AccountID;

            IClient client = ServiceProvider.Current.Data.Client.GetClient(clientId);
            IAccount acct = ServiceProvider.Current.Data.Account.GetAccount(accountId);
            
            IBillingType bt = mgr.GetBillingType(period, clientId, acct.OrgID, holidays);
            BillingCategory bc = (BillingCategory)Enum.Parse(typeof(BillingCategory), grp.Key.SubType, true);
            decimal totalMisc = grp.Sum(g => Convert.ToDecimal(g.Quantity) * g.UnitCost);
            decimal totalSubsidy = grp.Sum(g => g.SubsidyDiscount);

            return new MiscBillingByAccount(period, client, acct, bt, bc, totalMisc, totalSubsidy);
        }

        private struct MiscBillingGroupByKeySelector
        {
            public MiscBillingGroupByKeySelector(IMiscBillingCharge mbc)
            {
                SubType = mbc.SUBType;
                Period = mbc.Period;
                ClientID = mbc.ClientID;
                AccountID = mbc.AccountID;
            }

            public string SubType { get; }
            public DateTime Period { get; }
            public int ClientID { get; }
            public int AccountID { get; }
        }
    }
}