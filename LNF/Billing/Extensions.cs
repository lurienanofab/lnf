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
        public static void ItemType(this CacheManager cm, string value)
        {
            cm.SetSessionValue("ItemType", value);
        }

        public static string ItemType(this CacheManager cm)
        {
            return cm.GetSessionValue("ItemType", () => string.Empty);
        }

        public static void Exp(this CacheManager cm, string value)
        {
            cm.SetSessionValue("Exp", value);
        }

        public static string Exp(this CacheManager cm)
        {
            return cm.GetSessionValue("Exp", () => string.Empty);
        }

        public static void Updated(this CacheManager cm, bool value)
        {
            cm.SetSessionValue("Updated", value);
        }

        public static bool Updated(this CacheManager cm)
        {
            return cm.GetSessionValue("Updated", () => false);
        }

        public static void StartPeriod(this CacheManager cm, DateTime value)
        {
            cm.SetSessionValue("StartPeriod", value);
        }

        public static DateTime StartPeriod(this CacheManager cm)
        {
            return cm.GetSessionValue("StartPeriod", () => DateTime.Now.FirstOfMonth().AddMonths(-1));
        }

        public static void EndPeriod(this CacheManager cm, DateTime value)
        {
            cm.SetSessionValue("EndPeriod", value);
        }

        public static DateTime EndPeriod(this CacheManager cm)
        {
            return cm.GetSessionValue("EndPeriod", () => cm.StartPeriod().AddMonths(1));
        }

        public static void InvoiceReport(this CacheManager cm, DataSet ds)
        {
            cm.SetSessionValue("InvoiceReport", ds);
        }

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

            // ***** Tool *****
            var toolBilling = DA.Current.Query<ToolBilling>().Where(x => x.AccountID == item.AccountID && x.Period == period).ToArray();

            foreach (var tb in toolBilling)
                tb.SubsidyDiscount = tb.GetTotalCharge() * item.UserPaymentPercentage;

            // ***** Room *****
            var roomBilling = DA.Current.Query<RoomBilling>().Where(x => x.AccountID == item.AccountID && x.Period == period).ToArray();

            foreach (var rb in roomBilling)
                rb.SubsidyDiscount = rb.GetTotalCharge() * item.UserPaymentPercentage;

            // ***** Misc *****
            var miscBilling = DA.Current.Query<MiscBillingCharge>().Where(x => new[] { "Room", "Tool" }.Contains(x.SUBType) && x.Account.AccountID == item.AccountID && x.Period == period).ToArray();

            foreach (var mb in miscBilling)
                mb.SubsidyDiscount = mb.GetTotalCost() * item.UserPaymentPercentage;
        }
    }

    public static class RoomBillingExtensions
    {
        /// <summary>
        /// The final billed amount based on BillingType and Room
        /// </summary>
        public static decimal GetLineCost(this IRoomBilling item)
        {
            // [2015-11-13 jg] this is identical to the logic originally in:
            //      1) sselFinOps.AppCode.BLL.FormulaBL.ApplyRoomFormula (for External Invoice)
            //      2) sselIndReports.AppCode.Bll.RoomBillingBL.GetRoomBillingDataByClientID (for User Usage Summary)
            //      3) LNF.WebApi.Billing.Models.ReportUtility.ApplyRoomFormula (for SUB reports)

            decimal result = 0;

            int cleanRoomId = 6;
            int organicsBayId = 6;

            //1. Find out all Monthly type users and apply to Clean room
            if (BillingTypeUtility.IsMonthlyUserBillingType(item.BillingTypeID))
            {
                if (item.RoomID == cleanRoomId) //Clean Room
                    result = item.MonthlyRoomCharge;
                else
                    result = item.GetTotalCharge();
            }
            //2. The growers are charged with room fee only when they reserve and activate a tool
            else if (BillingTypeUtility.IsGrowerUserBillingType(item.BillingTypeID))
            {
                if (item.RoomID == organicsBayId) //Organics Bay
                    result = item.RoomCharge; //organics bay must be charged for growers as well
                else
                    result = item.AccountDays * item.RoomRate + item.EntryCharge;
            }
            else if (item.BillingTypeID == BillingType.Other)
            {
                result = 0;
            }
            else if (item.BillingTypeID == BillingType.Grower_Observer)
            {
                result = item.GetTotalCharge();
            }
            else
            {
                //Per Use types
                result = item.GetTotalCharge();
            }

            return result;
        }

        public static Account GetAccount(this RoomBillingBase item)
        {
            return DA.Current.Single<Account>(item.AccountID);
        }

        public static Org GetOrg(this RoomBillingBase item)
        {
            return DA.Current.Single<Org>(item.OrgID);
        }
    }

    public static class ToolBillingExtensions
    {
        /// <summary>
        /// The final billed amount based on BillingType and Room
        /// </summary>
        public static decimal GetLineCost(this IToolBilling item)
        {
            // [2015-11-13 jg] this is identical to the logic originally in:
            //      1) sselFinOps.AppCode.BLL.FormulaBL.ApplyToolFormula (for External Invoice)
            //      2) sselIndReports.AppCode.Bll.ToolBillingBL.GetToolBillingDataByClientID20110701 (for User Usage Summary)
            //      3) LNF.WebApi.Billing.Models.ReportUtility.ApplyToolFormula (for SUB reports, note: this is the replacement for the Billing WCF service)
            //
            //      I think at this point all the formulas can be replaced by GetTotalCharge()
            //      because each value used by the formula should correctly reflect the rules
            //      in place during the given period (or at least that is the goal).

            decimal result = 0;

            //if rates are 0 everything must be 0 (this was at the end, but why not do it at the beginning?)
            if (item.ResourceRate + item.PerUseRate == 0)
                result = 0;

            int cleanRoomId = 6;
            int maskMakerId = 56000;

            if (BillingTypeUtility.IsMonthlyUserBillingType(item.BillingTypeID)) //not used at this point but maybe in the future
            {
                // Monthly User, charge mask maker for everyone
                if (item.RoomID == cleanRoomId) //Clean Room
                {
                    if (item.ResourceID == maskMakerId) //Mask Maker
                    {
                        if (item.IsStarted)
                            result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2);
                        else
                            result = item.UncancelledPenaltyFee + item.ReservationFee2;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    //non clean room tools are always charged for usage fee
                    if (item.IsStarted)
                        result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2);
                    else
                        result = item.UncancelledPenaltyFee + item.ReservationFee2;
                }
            }
            else if (item.BillingTypeID == BillingType.Other)
            {
                //based on sselIndReports.AppCode.BLL.ToolBillingBL.GetToolBillingDataByClientID20110701 the Other billing type is not set to zero any longer
                result = item.GetTotalCharge();
            }
            else
            {
                //Per Use types
                if (item.Period >= new DateTime(2010, 7, 1))
                {
                    //2011-05 New tool billing started on 2011-04
                    if (item.Period >= new DateTime(2011, 4, 1))
                    {
                        if (!item.IsCancelledBeforeAllowedTime)
                            result = item.UsageFeeCharged + item.OverTimePenaltyFee + item.BookingFee; //should be the same as GetTotalCharge()
                        else
                            result = item.BookingFee; //Cancelled before two hours - should be the same as GetTotalCharge()
                    }
                    else
                    {
                        if (item.IsStarted)
                            result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2); //should be the same as GetTotalCharge()
                        else
                            result = item.UncancelledPenaltyFee; //should be the same as GetTotalCharge()
                    }
                }
                else
                {
                    if (item.IsStarted)
                        result = item.UsageFeeCharged + item.OverTimePenaltyFee + (item.ResourceRate == 0 ? 0 : item.ReservationFee2); //should be the same as GetTotalCharge()
                    else
                        result = item.UncancelledPenaltyFee + item.ReservationFee2; //should be the same as GetTotalCharge()
                }
            }

            return result;
        }
    }
}
