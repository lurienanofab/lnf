using LNF.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Store
{
    public static class Prices
    {
		private static readonly string STORE_MULTIPLIER_COST = "StoreMultiplierCost";

        public static IEnumerable<IPrice> GetPrices(IItem item)
        {
            return ServiceProvider.Current.Store.GetPrices(item.ItemID);
        }

		public static decimal ApplyStoreMultiplier(decimal price, int accountId)
		{
            var acct = ServiceProvider.Current.Data.Account.GetAccount(accountId);
            var cost = ServiceProvider.Current.Data.Cost.FindCosts(new[] { STORE_MULTIPLIER_COST }, chargeTypeId: acct.ChargeTypeID).OrderByDescending(x => x.EffDate).First();
            var result = price * cost.MulVal;
            return result;
		}
    }
}
