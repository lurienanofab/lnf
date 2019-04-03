using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Inventory;
using LNF.Repository.Store;
using LNF.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class StoreDataCleanUtility
    {
        public static int LoadDryBoxBilling(DateTime sd, DateTime ed)
        {
            int result = 0;

            double daysInPeriod = (ed - sd).TotalDays;

            IList<DryBoxAssignment> activeAssignments = ServiceProvider.Current.DryBoxManager.ActiveAssignments(sd, ed);

            if (activeAssignments.Count > 0)
            {
                int dryboxCategoryId = 33;
                Category dryboxCategory = DA.Current.Single<Category>(dryboxCategoryId);
                IList<Item> dryboxItems = DA.Current.Query<Item>().Where(x => x.Category == dryboxCategory).ToList();

                DeleteStoreDataClean(sd, ed, category: dryboxCategory);

                Dictionary<int, Item> map = new Dictionary<int, Item>
                    {
                        { 5, dryboxItems.Where(x => x.Description.Contains("[Academic]")).FirstOrDefault() },
                        { 15, dryboxItems.Where(x => x.Description.Contains("[Academic]")).FirstOrDefault() },
                        { 25, dryboxItems.Where(x => x.Description.Contains("[Non-Academic]")).FirstOrDefault() }
                    };

                Dictionary<Item, IList<Price>> prices = new Dictionary<Item, IList<Price>>();

                foreach (KeyValuePair<int, Item> kvp in map)
                {
                    if (!prices.ContainsKey(kvp.Value))
                    {
                        prices.Add(kvp.Value, PriceUtility.GetPrices(kvp.Value));
                    }
                }

                IList<DryBoxAssignmentLog> logItems = DA.Current.Query<DryBoxAssignmentLog>().Where(x => activeAssignments.Contains(x.DryBoxAssignment)).ToList();

                var clientAccountIds = logItems.Select(x => x.ClientAccount.ClientAccountID).Distinct().ToArray();

                var clientAccounts = DA.Current.Query<ClientAccountInfo>().Where(x => clientAccountIds.Contains(x.ClientAccountID)).ToList();

                foreach (DryBoxAssignmentLog logItem in logItems)
                {
                    StoreDataClean sdc = new StoreDataClean();
                    DateTime sdate = (logItem.EnableDate < sd) ? sd : logItem.EnableDate;
                    DateTime edate;
                    if (logItem.DisableDate != null) edate = (logItem.DisableDate.Value > ed) ? ed : logItem.DisableDate.Value;
                    else edate = (DateTime.Now.AddDays(1) > ed) ? ed : DateTime.Now.Date.AddDays(1);
                    double daysUsed = (edate - sdate).TotalDays;

                    //may be zero if approved and removed on the same day
                    if (daysUsed > 0)
                    {
                        var clientAcctInfo = clientAccounts.FirstOrDefault(x => x.ClientAccountID == logItem.ClientAccount.ClientAccountID);
                        sdc.Account = DA.Current.Single<Account>(clientAcctInfo.AccountID);
                        sdc.Category = dryboxCategory;
                        sdc.Client = DA.Current.Single<Client>(clientAcctInfo.ClientID);
                        sdc.Item = map[clientAcctInfo.ChargeTypeID];
                        sdc.OrderDate = sdate;
                        sdc.Quantity = Convert.ToDecimal(Math.Round(daysUsed / daysInPeriod, 4));
                        sdc.RechargeItem = true;
                        sdc.StatusChangeDate = sdate;
                        sdc.UnitCost = PriceUtility.GetHistoricalPrice(prices[sdc.Item], edate).PackagePrice;
                        DA.Current.Insert(sdc);
                        result += 1;
                    }
                }
            }

            return result;
        }

        public static int DeleteStoreDataClean(DateTime startDate, DateTime endDate, Client client = null, Item item = null, Category category = null)
        {
            int clientId = (client == null) ? 0 : client.ClientID;
            int itemId = (item == null) ? 0 : item.ItemID;
            int categoryId = (category == null) ? 0 : category.CatID;

            return DA.Command()
                .Param("sDate", startDate)
                .Param("eDate", endDate)
                .Param("ClientID", clientId > 0, clientId)
                .Param("ItemID", itemId > 0, itemId)
                .Param("CategoryID", categoryId > 0, categoryId)
                .ExecuteNonQuery("dbo.StoreDataClean_Delete").Value;
        }
    }
}
