using LNF.Models.Billing;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class MiscBillingByAccount
    {
        public DateTime Period { get; }
        public Client Client { get; }
        public Account Account { get; }
        public BillingType BillingType { get; }
        public BillingCategory BillingCategory { get; }
        public decimal TotalMisc { get; }
        public decimal TotalMiscSubsidyDiscount { get; }

        private MiscBillingByAccount(DateTime period, Client client, Account acct, BillingType bt, BillingCategory bc, decimal totalMisc, decimal totalSubsidy)
        {
            Period = period;
            Client = client;
            Account = acct;
            BillingType = bt;
            BillingCategory = bc;
            TotalMisc = totalMisc;
            TotalMiscSubsidyDiscount = totalSubsidy;
        }

        public static IEnumerable<MiscBillingByAccount> Create(IEnumerable<MiscBillingCharge> source)
        {
            return source.GroupBy(x => new MiscBillingGroupByKeySelector(x))
                .Select(CreateMiscBillingByAccount)
                .OrderBy(x => x.Period)
                .ThenBy(x => x.Client.ClientID)
                .ThenBy(x => x.Account.AccountID)
                .ThenBy(x => x.BillingCategory)
                .ToArray();
        }

        private static MiscBillingByAccount CreateMiscBillingByAccount(IGrouping<MiscBillingGroupByKeySelector, MiscBillingCharge> grp)
        {
            DateTime period = grp.Key.Period;
            Client client = grp.Key.Client;
            Account acct = grp.Key.Account;
            BillingType bt = BillingTypeUtility.GetBillingTypeByClientAndOrg(grp.Key.Period, grp.Key.Client, grp.Key.Account.Org);
            BillingCategory bc = (BillingCategory)Enum.Parse(typeof(BillingCategory), grp.Key.SUBType, true);
            decimal totalMisc = grp.Sum(g => g.Quantity * g.UnitCost);
            decimal totalSubsidy = grp.Sum(g => g.SubsidyDiscount);

            return new MiscBillingByAccount(period, client, acct, bt, bc, totalMisc, totalSubsidy);
        }

        private struct MiscBillingGroupByKeySelector
        {
            public MiscBillingGroupByKeySelector(MiscBillingCharge mbc)
            {
                SUBType = mbc.SUBType;
                Period = mbc.Period;
                Client = mbc.Client;
                Account = mbc.Account;
            }

            public string SUBType { get; }
            public DateTime Period { get; }
            public Client Client { get; }
            public Account Account { get; }
        }
    }
}