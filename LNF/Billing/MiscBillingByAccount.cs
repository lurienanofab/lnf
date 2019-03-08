using LNF.Models.Billing;
using LNF.Repository;
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

        public IEnumerable<MiscBillingByAccount> Create(IEnumerable<MiscBillingCharge> source, IEnumerable<Holiday> holidays, BillingTypeManager mgr)
        {
            return source.GroupBy(x => new MiscBillingGroupByKeySelector(x))
                .Select(x => CreateMiscBillingByAccount(x, holidays, mgr))
                .OrderBy(x => x.Period)
                .ThenBy(x => x.Client.ClientID)
                .ThenBy(x => x.Account.AccountID)
                .ThenBy(x => x.BillingCategory)
                .ToArray();
        }

        private MiscBillingByAccount CreateMiscBillingByAccount(IGrouping<MiscBillingGroupByKeySelector, MiscBillingCharge> grp, IEnumerable<Holiday> holidays, BillingTypeManager mgr)
        {
            var period = grp.Key.Period;
            var client = grp.Key.Client;
            var account = grp.Key.Account;

            BillingType bt = mgr.GetBillingTypeByClientAndOrg(period, client, account.Org, holidays);
            BillingCategory bc = (BillingCategory)Enum.Parse(typeof(BillingCategory), grp.Key.SubType, true);
            decimal totalMisc = grp.Sum(g => Convert.ToDecimal(g.Quantity) * g.UnitCost);
            decimal totalSubsidy = grp.Sum(g => g.SubsidyDiscount);

            return new MiscBillingByAccount(period, client, account, bt, bc, totalMisc, totalSubsidy);
        }

        private struct MiscBillingGroupByKeySelector
        {
            public MiscBillingGroupByKeySelector(MiscBillingCharge mbc)
            {
                SubType = mbc.SubType;
                Period = mbc.Period;
                Client = mbc.Client;
                Account = mbc.Account;
            }

            public string SubType { get; }
            public DateTime Period { get; }
            public Client Client { get; }
            public Account Account { get; }
        }
    }
}