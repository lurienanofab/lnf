using LNF.Cache;
using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Data;
using System.Linq;

namespace LNF.Billing
{
    public static class CacheManagerExtensions
    {
        [Obsolete("Use HttpContextBase instead.")]
        public static void ItemType(this CacheManager cm, string value)
        {
            cm.SetSessionValue("ItemType", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static string ItemType(this CacheManager cm)
        {
            return cm.GetSessionValue("ItemType", () => string.Empty);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void Exp(this CacheManager cm, string value)
        {
            cm.SetSessionValue("Exp", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static string Exp(this CacheManager cm)
        {
            return cm.GetSessionValue("Exp", () => string.Empty);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void Updated(this CacheManager cm, bool value)
        {
            cm.SetSessionValue("Updated", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static bool Updated(this CacheManager cm)
        {
            return cm.GetSessionValue("Updated", () => false);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void StartPeriod(this CacheManager cm, DateTime value)
        {
            cm.SetSessionValue("StartPeriod", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static DateTime StartPeriod(this CacheManager cm)
        {
            return cm.GetSessionValue("StartPeriod", () => DateTime.Now.FirstOfMonth().AddMonths(-1));
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void EndPeriod(this CacheManager cm, DateTime value)
        {
            cm.SetSessionValue("EndPeriod", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static DateTime EndPeriod(this CacheManager cm)
        {
            return cm.GetSessionValue("EndPeriod", () => cm.StartPeriod().AddMonths(1));
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void InvoiceReport(this CacheManager cm, DataSet ds)
        {
            cm.SetSessionValue("InvoiceReport", ds);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static DataSet InvoiceReport(this CacheManager cm)
        {
            return cm.GetSessionValue<DataSet>("InvoiceReport", () => null);
        }
    }

    public static class AccountSubsidyExtensions
    {
        public static void ApplyToBilling(this AccountSubsidy item, DateTime period)
        {
            // get all the billing records with this account and override the subsidy discount

            // xxxxx Tool xxxxx
            var toolBilling = DA.Current.Query<ToolBilling>().Where(x => x.AccountID == item.AccountID && x.Period == period).ToArray();

            foreach (var tb in toolBilling)
                tb.SubsidyDiscount = tb.GetTotalCharge() * item.UserPaymentPercentage;

            // xxxxx Room xxxxx
            var roomBilling = DA.Current.Query<RoomBilling>().Where(x => x.AccountID == item.AccountID && x.Period == period).ToArray();

            foreach (var rb in roomBilling)
                rb.SubsidyDiscount = rb.GetTotalCharge() * item.UserPaymentPercentage;

            // xxxxx Misc xxxxx
            var miscBilling = DA.Current.Query<MiscBillingCharge>().Where(x => new[] { "Room", "Tool" }.Contains(x.SubType) && x.Account.AccountID == item.AccountID && x.Period == period).ToArray();

            foreach (var mb in miscBilling)
                mb.SubsidyDiscount = mb.GetTotalCost() * item.UserPaymentPercentage;
        }
    }

    public static class RoomBillingExtensions
    {
        public static Account GetAccount(this RoomBillingBase item)
        {
            return DA.Current.Single<Account>(item.AccountID);
        }

        public static Org GetOrg(this RoomBillingBase item)
        {
            return DA.Current.Single<Org>(item.OrgID);
        }
    }
}
