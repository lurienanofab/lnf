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
        public static int LoadDryBoxBilling(DateTime startDate, DateTime endDate)
        {
            int result = 0;

            List<DateTime> periods = new List<DateTime>();
            DateTime p = DateTime.Parse(startDate.ToString("yyyy-MM-01"));
            while (p < endDate)
            {
                periods.Add(p);
                p = p.AddMonths(1);
            }

            //This is run daily but DryBox charges are by period
            foreach (DateTime period in periods)
            {
                DateTime sd = period;
                DateTime ed = sd.AddMonths(1);

                double daysInPeriod = (ed - sd).TotalDays;

                var session = ServiceProvider.Current.DataAccess.Session;
                var mgr = session.DryBoxManager();

                IList<DryBoxAssignment> activeAssignments = mgr.ActiveAssignments(sd, ed);
                if (activeAssignments.Count > 0)
                {
                    int dryboxCategoryId = 33;
                    Category dryboxCategory = session.Single<Category>(dryboxCategoryId);
                    IList<Item> dryboxItems = session.Query<Item>().Where(x => x.Category == dryboxCategory).ToList();

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

                    IList<DryBoxAssignmentLog> logItems = session.Query<DryBoxAssignmentLog>().Where(x => activeAssignments.Contains(x.DryBoxAssignment)).ToList();

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
                            var clientAcctInfo = mgr.GetClientAccountInfo(logItem);
                            sdc.Account = session.Single<Account>(clientAcctInfo.AccountID);
                            sdc.Category = dryboxCategory;
                            sdc.Client = session.Single<Client>(clientAcctInfo.ClientID);
                            sdc.Item = map[clientAcctInfo.ChargeTypeID];
                            sdc.OrderDate = sdate;
                            sdc.Quantity = Convert.ToDecimal(Math.Round(daysUsed / daysInPeriod, 4));
                            sdc.RechargeItem = true;
                            sdc.StatusChangeDate = sdate;
                            sdc.UnitCost = PriceUtility.GetHistoricalPrice(prices[sdc.Item], edate).PackagePrice;
                            session.Insert(sdc);
                            result += 1;
                        }
                    }
                }
            }

            return result;
        }

        public static int DeleteStoreDataClean(DateTime startDate, DateTime endDate, Client client = null, Item item = null, Category category = null)
        {
            int? nil = null;
            int? clientId = (client == null) ? nil : client.ClientID;
            int? itemId = (item == null) ? nil : item.ItemID;
            int? categoryId = (category == null) ? nil : category.CatID;

            using (var dba = DA.Current.GetAdapter())
            {
                return dba.ApplyParameters(new { sDate = startDate, eDate = endDate, ClientID = clientId, ItemID = itemId, CategoryID = categoryId })
                    .ExecuteNonQuery("sselData.dbo.StoreDataClean_Delete");
            }
        }
    }
}
