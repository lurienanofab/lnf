using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Store;
using LNF.Repository.Inventory;

namespace LNF.Store
{
    public static class PriceUtility
    {
		private static String STORE_MULTIPLIER_COST = "StoreMultiplierCost";

        public static Price GetHistoricalPrice(IList<Price> prices, DateTime date)
        {
            //if (date.Day != 1)
            //    date = new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1);

            IList<Price> included = prices.Where(x => x.DateActive <= date.Date).ToList();
            Price result = null;
            if (included.Count > 0)
                result = prices.Where(x => x.DateActive == included.Max(b => b.DateActive)).FirstOrDefault();
            if (result == null)
                result = prices[0];
            return result;
        }

        public static IList<Price> GetPrices(Item item)
        {
            IList<Package> packages = DA.Current.Query<Package>().Where(x => x.Item == item).ToList();
            IList<VendorPackage> vendorPackages = DA.Current.Query<VendorPackage>().Where(x => packages.Contains(x.Package)).ToList();
            IList<Price> prices = DA.Current.Query<Price>().Where(x => vendorPackages.Contains(x.VendorPackage)).ToList();
            return prices;
        }
		public static IList<Price> GetPricesFor(int itemID, int clientID)
		{
			IList<Package> packages = DA.Current.Query<Package>().Where(x => x.Item.ItemID == itemID).ToList();
			IList<VendorPackage> vendorPackages = DA.Current.Query<VendorPackage>().Where(x => packages.Contains(x.Package)).ToList();
			IList<Price> prices = DA.Current.Query<Price>().Where(x => vendorPackages.Contains(x.VendorPackage)).ToList();
			return prices;
		}
		public static decimal ApplyStoreMultiplier(decimal actPrice, int acctID)
		{
			Account acct = DA.Current.Single<Account>(acctID);
            IList<Cost> costs = DA.Current.Query<Cost>().Where(x => x.ChargeType.ChargeTypeID == acct.Org.OrgType.ChargeType.ChargeTypeID && x.TableNameOrDescription == STORE_MULTIPLIER_COST).ToList();
            Cost effectiveCost = costs.OrderBy(x => x.EffDate).Last();
            return actPrice * effectiveCost.MulVal;
		}
    }
}
