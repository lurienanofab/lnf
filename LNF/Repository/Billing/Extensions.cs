using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Billing.Reports;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Billing
{
    public static class Extensions
    {
        /*===== Room ============================================================================*/
        public static RoomBillingItem CreateRoomBillingItem(this IRoomBilling item) => CreateRoomBillingItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<RoomBillingItem> CreateRoomBillingItems(this IQueryable<IRoomBilling> query)
        {
            if (query == null) return null;

            return query.Select(x => new RoomBillingItem
            {
                RoomBillingID = x.RoomBillingID,
                Period = x.Period,
                ClientID = x.ClientID,
                RoomID = x.RoomID,
                AccountID = x.AccountID,
                ChargeTypeID = x.ChargeTypeID,
                BillingTypeID = x.BillingTypeID,
                OrgID = x.OrgID,
                ChargeDays = x.ChargeDays,
                PhysicalDays = x.PhysicalDays,
                AccountDays = x.AccountDays,
                Entries = x.Entries,
                Hours = x.Hours,
                IsDefault = x.IsDefault,
                RoomRate = x.RoomRate,
                EntryRate = x.EntryRate,
                MonthlyRoomCharge = x.MonthlyRoomCharge,
                RoomCharge = x.RoomCharge,
                EntryCharge = x.EntryCharge,
                SubsidyDiscount = x.SubsidyDiscount,
                IsTemp = x.IsTemp
            }).ToList();
        }

        public static RoomDataItem CreateRoomDataItem(this RoomData item) => CreateRoomDataItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<RoomDataItem> CreateRoomDataItems(this IQueryable<RoomData> query)
        {
            if (query == null) return null;

            return query.Select(x => new RoomDataItem
            {
                RoomDataID = x.RoomDataID,
                Period = x.Period,
                ClientID = x.ClientID,
                RoomID = x.RoomID,
                ParentID = x.ParentID,
                PassbackRoom = x.PassbackRoom,
                EvtDate = x.EvtDate,
                AccountID = x.AccountID,
                Entries = x.Entries,
                Hours = x.Hours,
                Days = x.Days,
                Months = x.Months,
                DataSource = x.DataSource,
                HasToolUsage = x.HasToolUsage
            }).ToList();
        }

        public static RoomDataCleanItem CreateRoomDataCleanItem(this RoomDataClean item) => CreateRoomDataCleanItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<RoomDataCleanItem> CreateRoomDataCleanItems(this IQueryable<RoomDataClean> query)
        {
            if (query == null) return null;

            return query.Select(x => new RoomDataCleanItem
            {
                RoomDataID = x.RoomDataID,
                ClientID = x.ClientID,
                RoomID = x.RoomID,
                EntryDT = x.EntryDT,
                ExitDT = x.ExitDT,
                Duration = x.Duration
            }).ToList();
        }


        /*===== Store ===========================================================================*/
        public static StoreBillingItem CreateStoreBillingItem(this IStoreBilling item) => CreateStoreBillingItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<StoreBillingItem> CreateStoreBillingItems(this IQueryable<IStoreBilling> query)
        {
            if (query == null) return null;

            return query.Select(x => new StoreBillingItem
            {
                StoreBillingID = x.StoreBillingID,
                Period = x.Period,
                ClientID = x.ClientID,
                AccountID = x.AccountID,
                ChargeTypeID = x.ChargeTypeID,
                ItemID = x.ItemID,
                CategoryID = x.CategoryID,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                CostMultiplier = x.CostMultiplier,
                LineCost = x.LineCost,
                StatusChangeDate = x.StatusChangeDate,
                IsTemp = x.IsTemp,
            }).ToList();
        }

        public static StoreDataItem CreateStoreDataItem(this StoreData item) => CreateStoreDataItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<StoreDataItem> CreateStoreDataItems(this IQueryable<StoreData> query)
        {
            if (query == null) return null;

            return query.Select(x => new StoreDataItem
            {
                StoreDataID = x.StoreDataID,
                Period = x.Period,
                ClientID = x.ClientID,
                ItemID = x.ItemID,
                OrderDate = x.OrderDate,
                AccountID = x.AccountID,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                CategoryID = x.CategoryID,
                StatusChangeDate = x.StatusChangeDate,
            }).ToList();
        }

        public static StoreDataCleanItem CreateStoreDataCleanItem(this StoreDataClean item) => CreateStoreDataCleanItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<StoreDataCleanItem> CreateStoreDataCleanItems(this IQueryable<StoreDataClean> query)
        {
            if (query == null) return null;

            return query.Select(x => new StoreDataCleanItem
            {
                StoreDataID = x.StoreDataID,
                ClientID = x.Client.ClientID,
                ItemID = x.Item.ItemID,
                OrderDate = x.OrderDate,
                AccountID = x.Account.AccountID,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                CategoryID = x.Category.CatID,
                RechargeItem = x.RechargeItem,
                StatusChangeDate = x.StatusChangeDate,
            }).ToList();
        }
    }
}
