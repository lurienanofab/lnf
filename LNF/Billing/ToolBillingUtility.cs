using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Billing
{
    public static class ToolBillingUtility
    {
        public static Models.Billing.IToolBilling CreateToolBillingItem(bool temp)
        {
            return new Models.Billing.ToolBillingItem { IsTemp = temp };
        }

        public static IEnumerable<Models.Billing.IToolBilling> CreateToolBillingFromDataTable(DataTable dt, bool temp)
        {
            foreach (DataRow dr in dt.Rows)
            {
                var item = CreateToolBillingFromDataRow(dr, temp);
                yield return item;
            }
        }

        public static Models.Billing.IToolBilling CreateToolBillingFromDataRow(DataRow dr, bool temp)
        {
            var item = CreateToolBillingItem(temp);

            item.ToolBillingID = 0;
            item.Period = dr.Field<DateTime>("Period");
            item.ReservationID = dr.Field<int>("ReservationID");
            item.ClientID = dr.Field<int>("ClientID");
            item.AccountID = dr.Field<int>("AccountID");
            item.ChargeTypeID = dr.Field<int>("ChargeTypeID");
            item.BillingTypeID = dr.Field<int>("BillingTypeID");
            item.RoomID = dr.Field<int>("RoomID");
            item.ResourceID = dr.Field<int>("ResourceID");
            item.ActDate = dr.Field<DateTime>("ActDate");
            item.IsStarted = dr.Field<bool>("IsStarted");
            item.IsActive = dr.Field<bool>("IsActive");
            item.IsFiftyPenalty = dr.Field<bool>("IsFiftyPenalty");
            item.ChargeMultiplier = Convert.ToDecimal(dr["ChargeMultiplier"]);
            item.Uses = Convert.ToDecimal(dr["Uses"]);
            item.SchedDuration = Convert.ToDecimal(dr["SchedDuration"]);
            item.ActDuration = Convert.ToDecimal(dr["ActDuration"]);
            item.ChargeDuration = Convert.ToDecimal(dr["ChargeDuration"]);
            item.TransferredDuration = Convert.ToDecimal(dr["TransferredDuration"]);
            item.ForgivenDuration = Convert.ToDecimal(dr["ForgivenDuration"]);
            item.MaxReservedDuration = Convert.ToDecimal(dr["MaxReservedDuration"]);
            item.OverTime = dr.Field<decimal>("OverTime");
            item.RatePeriod = dr.Field<string>("RatePeriod");
            item.PerUseRate = dr.Field<decimal>("PerUseRate");
            item.ResourceRate = dr.Field<decimal>("ResourceRate");
            item.ReservationRate = dr.Field<decimal>("ReservationRate");
            item.OverTimePenaltyPercentage = Convert.ToDecimal(dr["OverTimePenaltyPercentage"]);
            item.UncancelledPenaltyPercentage = Convert.ToDecimal(dr["UncancelledPenaltyPercentage"]);
            item.UsageFeeCharged = dr.Field<decimal>("UsageFeeCharged");
            item.UsageFee20110401 = dr.Field<decimal>("UsageFee20110401");
            item.UsageFee = dr.Field<decimal>("UsageFee");
            item.UsageFeeOld = dr.Field<decimal>("UsageFeeOld");
            item.OverTimePenaltyFee = dr.Field<decimal>("OverTimePenaltyFee");
            item.UncancelledPenaltyFee = dr.Field<decimal>("UncancelledPenaltyFee");
            item.BookingFee = dr.Field<decimal>("BookingFee");
            item.TransferredFee = dr.Field<decimal>("TransferredFee");
            item.ForgivenFee = dr.Field<decimal>("ForgivenFee");
            item.SubsidyDiscount = dr.Field<decimal>("SubsidyDiscount");
            item.IsCancelledBeforeAllowedTime = dr.Field<bool>("IsCancelledBeforeAllowedTime");
            item.ReservationFeeOld = dr.Field<decimal>("ReservationFeeOld");
            item.ReservationFee2 = dr.Field<decimal>("ReservationFee2");
            item.UsageFeeFiftyPercent = dr.Field<decimal>("UsageFeeFiftyPercent");

            return item;
        }
    }
}
