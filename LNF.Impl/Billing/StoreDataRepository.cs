using LNF.Billing;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Inventory;
using LNF.Impl.Repository.Store;
using LNF.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class StoreDataRepository : BillingRepository, IStoreDataRepository
    {
        public StoreDataRepository(ISessionManager mgr) : base(mgr) { }

        public DataTable ReadStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadStoreData(period, clientId, itemId);
                conn.Close();
                return result;
            }
        }

        public DataTable ReadStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, StoreDataCleanOption option = StoreDataCleanOption.AllItems)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadStoreDataClean(sd, ed, clientId, itemId, option);
                conn.Close();
                return result;
            }
        }

        public DataTable ReadStoreDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadStoreDataRaw(sd, ed, clientId);
                conn.Close();
                return result;
            }
        }

        public int LoadDryBoxBilling(DateTime sd, DateTime ed)
        {
            int result = 0;

            double daysInPeriod = (ed - sd).TotalDays;

            var activeAssignments = ServiceProvider.Current.Data.DryBox.GetActiveAssignments(sd, ed).ToList();

            if (activeAssignments.Count > 0)
            {
                int dryboxCategoryId = 33;
                Category dryboxCategory = Require<Category>(dryboxCategoryId);
                IList<Item> dryboxItems = Session.Query<Item>().Where(x => x.CatID == dryboxCategory.CatID).ToList();

                DeleteStoreDataClean(sd, ed, catId: dryboxCategory.CatID);

                Dictionary<int, Item> map = new Dictionary<int, Item>
                    {
                        { 5, dryboxItems.Where(x => x.Description.Contains("[Academic]")).FirstOrDefault() },
                        { 15, dryboxItems.Where(x => x.Description.Contains("[Academic]")).FirstOrDefault() },
                        { 25, dryboxItems.Where(x => x.Description.Contains("[Non-Academic]")).FirstOrDefault() }
                    };

                Dictionary<Item, IList<PriceInfo>> prices = new Dictionary<Item, IList<PriceInfo>>();

                foreach (KeyValuePair<int, Item> kvp in map)
                {
                    if (!prices.ContainsKey(kvp.Value))
                    {
                        prices.Add(kvp.Value, Session.GetPrices(kvp.Value.ItemID));
                    }
                }

                IList<DryBoxAssignmentLog> logItems = Session.Query<DryBoxAssignmentLog>().Where(x => activeAssignments.Any(y => y.DryBoxAssignmentID == x.DryBoxAssignment.DryBoxAssignmentID)).ToList();

                var clientAccountIds = logItems.Select(x => x.ClientAccount.ClientAccountID).Distinct().ToArray();

                var clientAccounts = Session.Query<ClientAccountInfo>().Where(x => clientAccountIds.Contains(x.ClientAccountID)).ToList();

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
                        sdc.Account = Require<Account>(clientAcctInfo.AccountID);
                        sdc.Category = dryboxCategory;
                        sdc.Client = Require<Client>(clientAcctInfo.ClientID);
                        sdc.Item = map[clientAcctInfo.ChargeTypeID];
                        sdc.OrderDate = sdate;
                        sdc.Quantity = Convert.ToDecimal(Math.Round(daysUsed / daysInPeriod, 4));
                        sdc.RechargeItem = true;
                        sdc.StatusChangeDate = sdate;
                        sdc.UnitCost = GetHistoricalPrice(prices[sdc.Item], edate).PackagePrice;
                        Session.Save(sdc);
                        result += 1;
                    }
                }
            }

            return result;
        }

        public int DeleteStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, int catId = 0)
        {
            return Session.Command()
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("ClientID", clientId > 0, clientId)
                .Param("ItemID", itemId > 0, itemId)
                .Param("CategoryID", catId > 0, catId)
                .ExecuteNonQuery("dbo.StoreDataClean_Delete").Value;
        }

        private IPrice GetHistoricalPrice(IEnumerable<IPrice> prices, DateTime date)
        {
            var included = prices.Where(x => x.DateActive <= date.Date).ToList();

            IPrice result = null;

            if (included.Count > 0)
                result = prices.Where(x => x.DateActive == included.Max(b => b.DateActive)).FirstOrDefault();

            if (result == null)
                result = prices.First();

            return result;
        }

        private StoreDataReader GetReader(SqlConnection conn)
        {
            return new StoreDataReader(conn);
        }
    }
}
