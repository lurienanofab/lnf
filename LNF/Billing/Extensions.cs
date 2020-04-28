using LNF.Cache;
using LNF.Data;
using System;
using System.Data;

namespace LNF.Billing
{
    public static class CacheExtensions
    {
        [Obsolete("Use HttpContextBase instead.")]
        public static void ItemType(this ICache c, string value)
        {
            throw new NotImplementedException();
            //c.SetSessionValue("ItemType", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static string ItemType(this ICache c)
        {
            throw new NotImplementedException();
            //return c.GetSessionValue("ItemType", () => string.Empty);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void Exp(this ICache c, string value)
        {
            throw new NotImplementedException();
            //c.SetSessionValue("Exp", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static string Exp(this ICache c)
        {
            throw new NotImplementedException();
            //return c.GetSessionValue("Exp", () => string.Empty);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void Updated(this ICache c, bool value)
        {
            throw new NotImplementedException();
            //c.SetSessionValue("Updated", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static bool Updated(this ICache c)
        {
            throw new NotImplementedException();
            //return c.GetSessionValue("Updated", () => false);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void StartPeriod(this ICache c, DateTime value)
        {
            throw new NotImplementedException();
            //c.SetSessionValue("StartPeriod", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static DateTime StartPeriod(this ICache c)
        {
            throw new NotImplementedException();
            //return c.GetSessionValue("StartPeriod", () => DateTime.Now.FirstOfMonth().AddMonths(-1));
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void EndPeriod(this ICache c, DateTime value)
        {
            throw new NotImplementedException();
            //c.SetSessionValue("EndPeriod", value);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static DateTime EndPeriod(this ICache c)
        {
            throw new NotImplementedException();
            //return c.GetSessionValue("EndPeriod", () => c.StartPeriod().AddMonths(1));
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static void InvoiceReport(this ICache c, DataSet ds)
        {
            throw new NotImplementedException();
            //c.SetSessionValue("InvoiceReport", ds);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public static DataSet InvoiceReport(this ICache c)
        {
            throw new NotImplementedException();
            //return c.GetSessionValue<DataSet>("InvoiceReport", () => null);
        }
    }

    public static class MiscBillingChargeExtensions
    {
        public static decimal GetTotalCost(this IMiscBillingCharge mbc)
        {
            throw new NotImplementedException();
        }
    }

    public static class RoomBillingExtensions
    {
        public static IAccount GetAccount(this IRoomBilling item)
        {
            return ServiceProvider.Current.Data.Account.GetAccount(item.AccountID);
        }

        public static IOrg GetOrg(this IRoomBilling item)
        {
            return ServiceProvider.Current.Data.Org.GetOrg(item.OrgID);
        }
    }
}
