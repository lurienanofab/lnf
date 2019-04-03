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
        /*===== Tool ============================================================================*/
        public static ToolBillingItem CreateToolBillingItem(this IToolBilling item) => CreateToolBillingItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<ToolBillingItem> CreateToolBillingItems(this IQueryable<IToolBilling> query)
        {
            if (query == null) return null;

            return query.Select(x => new ToolBillingItem
            {
                ToolBillingID = x.ToolBillingID,
                Period = x.Period,
                ReservationID = x.ReservationID,
                ClientID = x.ClientID,
                AccountID = x.AccountID,
                ChargeTypeID = x.ChargeTypeID,
                BillingTypeID = x.BillingTypeID,
                RoomID = x.RoomID,
                ResourceID = x.ResourceID,
                ActDate = x.ActDate,
                IsStarted = x.IsStarted,
                IsActive = x.IsActive,
                IsFiftyPenalty = x.IsFiftyPenalty,
                ChargeMultiplier = x.ChargeMultiplier,
                Uses = x.Uses,
                SchedDuration = x.SchedDuration,
                ActDuration = x.ActDuration,
                ChargeDuration = x.ChargeDuration,
                TransferredDuration = x.TransferredDuration,
                ForgivenDuration = x.ForgivenDuration,
                MaxReservedDuration = x.MaxReservedDuration,
                OverTime = x.OverTime,
                RatePeriod = x.RatePeriod,
                PerUseRate = x.PerUseRate,
                ResourceRate = x.ResourceRate,
                ReservationRate = x.ReservationRate,
                OverTimePenaltyPercentage = x.OverTimePenaltyPercentage,
                UncancelledPenaltyPercentage = x.UncancelledPenaltyPercentage,
                UsageFeeCharged = x.UsageFeeCharged,
                UsageFee20110401 = x.UsageFee20110401,
                UsageFee = x.UsageFee,
                UsageFeeOld = x.UsageFeeOld,
                OverTimePenaltyFee = x.OverTimePenaltyFee,
                UncancelledPenaltyFee = x.UncancelledPenaltyFee,
                BookingFee = x.BookingFee,
                TransferredFee = x.TransferredFee,
                ForgivenFee = x.ForgivenFee,
                SubsidyDiscount = x.SubsidyDiscount,
                IsCancelledBeforeAllowedTime = x.IsCancelledBeforeAllowedTime,
                ReservationFeeOld = x.ReservationFeeOld,
                ReservationFee2 = x.ReservationFee2,
                UsageFeeFiftyPercent = x.UsageFeeFiftyPercent,
                IsTemp = x.IsTemp
            }).ToList();
        }

        public static ToolDataItem CreateToolDataItem(this ToolData item) => CreateToolDataItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<ToolDataItem> CreateToolDataItems(this IQueryable<ToolData> query)
        {
            if (query == null) return null;

            return query.Select(x => new ToolDataItem
            {
                ToolDataID = x.ToolDataID,
                Period = x.Period,
                ClientID = x.ClientID,
                ResourceID = x.ResourceID,
                RoomID = x.RoomID,
                ActDate = x.ActDate,
                AccountID = x.AccountID,
                Uses = x.Uses,
                SchedDuration = x.SchedDuration,
                ActDuration = x.ActDuration,
                OverTime = x.OverTime,
                Days = x.Days,
                Months = x.Months,
                IsStarted = x.IsStarted,
                ChargeMultiplier = x.ChargeMultiplier,
                ReservationID = x.ReservationID,
                ChargeDuration = x.ChargeDuration,
                TransferredDuration = x.TransferredDuration,
                MaxReservedDuration = x.MaxReservedDuration,
                ChargeBeginDateTime = x.ChargeBeginDateTime,
                ChargeEndDateTime = x.ChargeEndDateTime,
                IsActive = x.IsActive,
                IsCancelledBeforeAllowedTime = x.IsCancelledBeforeAllowedTime
            }).ToList();
        }

        public static ToolDataCleanItem CreateToolDataCleanItem(this ToolDataClean item) => CreateToolDataCleanItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<ToolDataCleanItem> CreateToolDataCleanItems(this IQueryable<ToolDataClean> query)
        {
            if (query == null) return null;

            return query.Select(x => new ToolDataCleanItem
            {
                ToolDataID = x.ToolDataID,
                ClientID = x.ClientID,
                ResourceID = x.ResourceID,
                RoomID = x.RoomID,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ActualBeginDateTime = x.ActualBeginDateTime,
                ActualEndDateTime = x.ActualEndDateTime,
                AccountID = x.AccountID,
                ActivityID = x.ActivityID,
                SchedDuration = x.SchedDuration,
                ActDuration = x.ActDuration,
                OverTime = x.OverTime,
                IsStarted = x.IsStarted,
                ChargeMultiplier = x.ChargeMultiplier,
                ReservationID = x.ReservationID,
                MaxReservedDuration = x.MaxReservedDuration,
                IsActive = x.IsActive,
                CancelledDateTime = x.CancelledDateTime,
                OriginalBeginDateTime = x.OriginalBeginDateTime,
                OriginalEndDateTime = x.OriginalEndDateTime,
                OriginalModifiedOn = x.OriginalModifiedOn,
                CreatedOn = x.CreatedOn
            }).ToList();
        }


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

        /*===== Misc ============================================================================*/
        public static MiscBillingChargeItem CreateMiscBillingChargeItem(this MiscBillingCharge item) => CreateMiscBillingChargeItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<MiscBillingChargeItem> CreateMiscBillingChargeItems(this IQueryable<MiscBillingCharge> query)
        {
            if (query == null) return null;

            return query.Select(x => new MiscBillingChargeItem
            {
                ExpID = x.ExpID,
                ClientID = x.Client.ClientID,
                LName = x.Client.LName,
                FName = x.Client.FName,
                AccountID = x.Account.AccountID,
                AccountName = x.Account.Name,
                ShortCode = x.Account.ShortCode,
                SubType = x.SubType,
                Period = x.Period,
                ActDate = x.ActDate,
                Description = x.Description,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                SubsidyDiscount = x.SubsidyDiscount,
                Active = x.Active
            }).ToList();
        }


        /*===== Regular Exception ===============================================================*/
        public static RegularExceptionItem CreateRegularExceptionItem(this RegularException item) => CreateRegularExceptionItems(Utility.ToQueryable(item)).FirstOrDefault();

        public static IEnumerable<RegularExceptionItem> CreateRegularExceptionItems(this IQueryable<RegularException> query)
        {
            if (query == null) return null;

            return query.Select(x => new RegularExceptionItem
            {
                BillingID = x.BillingID,
                Period = x.Period,
                BillingCategory = x.BillingCategory,
                ReservationID = x.ReservationID,
                ClientID = x.ClientID,
                LName = x.LName,
                FName = x.FName,
                InviteeClientID = x.InviteeClientID,
                InviteeLName = x.InviteeLName,
                InviteeFName = x.InviteeFName,
                ResourceID = x.ResourceID,
                ResourceName = x.ResourceName,
                AccountID = x.AccountID,
                AccountName = x.AccountName,
                ShortCode = x.ShortCode,
                IsTemp = x.IsTemp
            }).ToList();
        }
    }
}
