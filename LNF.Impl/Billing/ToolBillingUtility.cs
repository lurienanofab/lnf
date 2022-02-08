using LNF.Billing;
using LNF.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Impl.Billing
{
    public static class ToolBillingUtility
    {
        public static Dictionary<DateTime, decimal> BookingFeePercentages = new Dictionary<DateTime, decimal>
        {
            [DateTime.Parse("1900-01-01")] = 0.0M,
            [DateTime.Parse("2011-04-01")] = 0.1M,
            [DateTime.Parse("2020-05-01")] = 0.0M
        };

        /// <summary>
        /// This is the date we started charging users with original reservation time instead of usage time
        /// </summary>
        public static readonly DateTime April2011 = DateTime.Parse("2011-04-01");

        /// <summary>
        /// This is the date we attempted to start using the per use fee. However it didn't actually work until 2011-10-01
        /// </summary>
        public static readonly DateTime June2013 = DateTime.Parse("2013-06-01");

        public static void CalculateToolBillingCharges(IToolBilling tb)
        {
            CalculateReservationFee(tb);
            CalculateUsageFeeCharged(tb);
            CalculateBookingFee(tb);
        }

        public static void CalculateReservationFee(IToolBilling item)
        {
            if (item.Period < April2011)
            {
                //2010-11 ReservationFee needs to be caluclated here [not sure why, the formula is exactly the same as the computed column ReservationFeeOld]
                if (item.IsStarted && item.ResourceRate > 0)
                    item.ReservationFee2 = item.ReservationRate * item.Uses * item.ChargeMultiplier;
                else
                    item.ReservationFee2 = 0;
            }
            else
            {
                //we stopped charging a reservation fee as of 2011-04-01
                item.ReservationFee2 = 0;
            }
        }

        public static void CalculateUsageFeeCharged(IToolBilling item)
        {
            //before April 2011
            if (item.Period < April2011)
                item.UsageFeeCharged = ((item.ChargeMultiplier * ((item.ActDuration - item.OverTime) / 60)) * item.ResourceRate) * (item.IsStarted ? 1 : 0);
            //as of April 2011 and before June 2013
            else if (item.Period >= April2011 && item.Period < June2013)
                item.UsageFeeCharged = ((item.ChargeMultiplier * (((item.ChargeDuration - item.OverTime) - item.TransferredDuration) / 60)) * item.ResourceRate) * (1 - (item.IsCancelledBeforeAllowedTime ? 1 : 0));
            //as of October 2015 ["as of June 2013" is no longer true because we continued to use UsageFee20110401 for billing after this date until 2015-10-01. The UsageFeeCharged column should always have the value of *what we actually billed* so I have to overwrite the UsageFeeCharged column with the values in UsageFee20110401 for the date range 2013-06-01 to 2015-10-01]
            else
            {
                decimal amount = 0;

                //first start with AddVal
                amount += item.Uses * item.PerUseRate;

                // next add any charges based on AcctPer and MulVal
                // item.ResourceRate is the MulVal (i.e. per period charge) and item.RatePeriod is AcctPer
                // uses is already baked in to ChargeDuration, OverTime, and TrnasferredDuration
                var baseDuration = item.ChargeDuration - item.OverTime - item.TransferredDuration;
                decimal rpc = RatePeriodCharge(item, baseDuration);
                amount += rpc;

                item.UsageFeeCharged = amount * item.ChargeMultiplier * (1 - (item.IsCancelledBeforeAllowedTime ? 1 : 0));
            }
        }

        private static decimal GetBookingFeePercentage(DateTime period)
        {
            decimal result = 0;

            foreach (var kvp in BookingFeePercentages)
            {
                if (period >= kvp.Key)
                    result = kvp.Value;
            }

            return result;
        }

        public static void CalculateBookingFee(IToolBilling item)
        {
            //before April 2011 there was no booking fee concept
            if (item.Period < April2011)
                item.BookingFee = GetBookingFeePercentage(item.Period);

            //as of April 2011 and before June 2013
            else if (item.Period >= April2011 && item.Period < June2013)
            {
                if (item.IsCancelledBeforeAllowedTime)
                    item.BookingFee = item.ResourceRate * (item.MaxReservedDuration / 60) * GetBookingFeePercentage(item.Period) * item.ChargeMultiplier;
                else
                {
                    //if user made smaller reservation and used it, we still charge those old booking fee
                    if (item.MaxReservedDuration > item.ChargeDuration)
                        item.BookingFee = item.ResourceRate * ((item.MaxReservedDuration - item.ChargeDuration) / 60) * GetBookingFeePercentage(item.Period) * item.ChargeMultiplier;
                    else
                        item.BookingFee = 0;
                }
            }

            //as of June 2013
            else
            {
                decimal amount = 0;

                //next add any charges based on AcctPer and MulVal (item.ResourceRate is the MulVal i.e. per period charge)
                if (item.IsCancelledBeforeAllowedTime)
                {
                    // [2018-08-01 jg] Commenting out the line below because we should not charge a booking
                    //      fee on the per use charge. This is because if someone modifies a reservation
                    //      they will receive the booking fee on each canceled-for-modification reservation,
                    //      plus they will be charged the full booking fee on the final reservation. Transferred
                    //      duration is not a factor here becuase that only affects the hourly charge.

                    // only charge a booking fee on the PerUseRate when the reservation was canceled
                    //amount += item.Uses * item.PerUseRate;

                    // [2018-08-01 jg] We should subtract the transferred duration. Otherwise we risk
                    //      charging the booking fee on used time, which is ok unless the time is used
                    //      by a reservation created for modification. Since we don't know at this point
                    //      where the transferred duration is coming from (was it from another user's
                    //      reservation, the same user not for modification, the same user for modification,
                    //      who knows?) we can't determine what amount of transferred duration should be
                    //      removed. Therefore it must always be removed to err on the side of caution.
                    //      Also need to use SchedDuration now because at this time MaxReservedDuration
                    //      is still determined based on prior modifications. Use SchedDuration instead
                    //      of ChargeDuration because the reservation was never started if we are here.
                    amount += RatePeriodCharge(item, item.SchedDuration - item.TransferredDuration);
                }
                else
                {
                    // [2018-08-01 jg] Skipping this part as of 2018-07-01.
                    //      Reason: we now handle reservation modification by cancelling the existing
                    //      reservation and creating a new one. Therefore the booking fee will be applied to
                    //      the original reservation and we no longer have to track MaxReservedDuration.
                    //      Otherwise we are double dipping - charging a booking fee on the original
                    //      "cancelled for modification" reservation and also the unused portion of
                    //      MaxReservedDuration of the new modification reservation (which is included in the
                    //      reservation that was canceled for modification by definition).
                }

                // As of May 2020 the percentage is zero because booking fees are being waived due to COVID-19.
                item.BookingFee = amount * GetBookingFeePercentage(item.Period) * item.ChargeMultiplier;
            }
        }

        public static decimal RatePeriodCharge(IToolBilling item, decimal duration) => RatePeriodCharge(item.Period, item.RatePeriod, item.ResourceRate, duration);

        public static decimal RatePeriodCharge(DateTime period, string ratePeriod, decimal resourceRate, decimal duration)
        {
            if (duration == 0) return 0;

            double factor;
            var dur = Convert.ToDouble(duration);
            var rate = Convert.ToDouble(resourceRate);

            switch (ratePeriod)
            {
                case "Hourly":
                    factor = 1D / 60D;
                    break;
                case "Daily":
                    factor = 1D / 60D / 24D;
                    break;
                //are these the same?
                case "Monthly":
                case "Per Month":
                    factor = 1D / 60D / 24D / CommonTools.Utility.NumberOfDaysInMonth(period);
                    break;
                case "Per Use":
                    factor = 1D / dur;
                    break;
                case "None":
                default:
                    factor = 0D;
                    break;
            }

            // Rounding to two decmial places because that is what is displayed on the User Usage Summary

            // [2018-08-01 jg] During the July 2018 audit we discovered a problem with this calcuation. The original
            //      method (#1) produced slightly inaccurate results due to rounding. See below for details.

            var method = period < DateTime.Parse("2018-07-01") ? 1 : 2;
            double amount;
            decimal result;

            if (method == 1)
            {
                // Method 1 was used until 2018-07-01
                amount = Math.Round(dur * factor, 2) * rate;
                result = Convert.ToDecimal(amount);
            }
            else
            {
                // Method 2 is used starting 2018-07-01, this way matches the computed column UsageFee20110401 (from ToolBilling)
                //      The difference between the methods is that in #1 the duration is converted to hours and rounded to two
                //      decimals before it is multipled by the hourly rate. In #2 the duration is converted to hours and multipled
                //      by the hourly rate and then the final product is rounded to two decimals. So #2 is more accurate because
                //      we are not losing minutes due to rounding before multiplying the hourly rate.
                amount = dur * factor * rate;
                result = Convert.ToDecimal(Math.Round(amount, 2));
            }

            return result;
        }

        public static IEnumerable<IToolBilling> CreateToolBillingFromDataTable(DataTable dt, bool temp)
        {
            foreach (DataRow dr in dt.Rows)
            {
                var item = CreateToolBillingFromDataRow(dr, temp);
                yield return item;
            }
        }

        public static IToolBilling CreateToolBillingFromDataRow(DataRow dr, bool temp)
        {
            var item = new ToolBillingItem
            {
                ToolBillingID = 0,
                Period = dr.Field<DateTime>("Period"),
                ReservationID = dr.Field<int>("ReservationID"),
                ClientID = dr.Field<int>("ClientID"),
                AccountID = dr.Field<int>("AccountID"),
                ChargeTypeID = dr.Field<int>("ChargeTypeID"),
                BillingTypeID = dr.Field<int>("BillingTypeID"),
                RoomID = dr.Field<int>("RoomID"),
                ResourceID = dr.Field<int>("ResourceID"),
                ActDate = dr.Field<DateTime>("ActDate"),
                IsStarted = dr.Field<bool>("IsStarted"),
                IsActive = dr.Field<bool>("IsActive"),
                IsFiftyPenalty = dr.Field<bool>("IsFiftyPenalty"),
                ChargeMultiplier = Convert.ToDecimal(dr["ChargeMultiplier"]),
                Uses = Convert.ToDecimal(dr["Uses"]),
                SchedDuration = Convert.ToDecimal(dr["SchedDuration"]),
                ActDuration = Convert.ToDecimal(dr["ActDuration"]),
                ChargeDuration = Convert.ToDecimal(dr["ChargeDuration"]),
                TransferredDuration = Convert.ToDecimal(dr["TransferredDuration"]),
                ForgivenDuration = Convert.ToDecimal(dr["ForgivenDuration"]),
                MaxReservedDuration = Convert.ToDecimal(dr["MaxReservedDuration"]),
                OverTime = dr.Field<decimal>("OverTime"),
                RatePeriod = dr.Field<string>("RatePeriod"),
                PerUseRate = dr.Field<decimal>("PerUseRate"),
                ResourceRate = dr.Field<decimal>("ResourceRate"),
                ReservationRate = dr.Field<decimal>("ReservationRate"),
                OverTimePenaltyPercentage = Convert.ToDecimal(dr["OverTimePenaltyPercentage"]),
                UncancelledPenaltyPercentage = Convert.ToDecimal(dr["UncancelledPenaltyPercentage"]),
                UsageFeeCharged = dr.Field<decimal>("UsageFeeCharged"),
                UsageFee20110401 = dr.Field<decimal>("UsageFee20110401"),
                UsageFee = dr.Field<decimal>("UsageFee"),
                UsageFeeOld = dr.Field<decimal>("UsageFeeOld"),
                OverTimePenaltyFee = dr.Field<decimal>("OverTimePenaltyFee"),
                UncancelledPenaltyFee = dr.Field<decimal>("UncancelledPenaltyFee"),
                BookingFee = dr.Field<decimal>("BookingFee"),
                TransferredFee = dr.Field<decimal>("TransferredFee"),
                ForgivenFee = dr.Field<decimal>("ForgivenFee"),
                SubsidyDiscount = dr.Field<decimal>("SubsidyDiscount"),
                IsCancelledBeforeAllowedTime = dr.Field<bool>("IsCancelledBeforeAllowedTime"),
                ReservationFeeOld = dr.Field<decimal>("ReservationFeeOld"),
                ReservationFee2 = dr.Field<decimal>("ReservationFee2"),
                UsageFeeFiftyPercent = dr.Field<decimal>("UsageFeeFiftyPercent"),
                IsTemp = temp
            };

            return item;
        }

        public static void CalculateToolLineCost(DataTable dt)
        {
            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            if (!dt.Columns.Contains("Room"))
                dt.Columns.Add("Room", typeof(string));

            //Part I: Get the true cost based on billing types
            foreach (DataRow dr in dt.Rows)
            {
                var item = CreateToolBillingFromDataRow(dr, false);
                dr.SetField("Room", Rooms.GetRoomDisplayName(item.RoomID));

                // |
                // | this is the line of code that sets the the final amount billed for tool usage used in SUB and JU reports sent to UofM FinOps
                // ▼
                dr.SetField("LineCost", GetLineCost(item));
            }
        }

        public static decimal GetLineCost(IToolBilling item) => GetLineCost(new ToolLineCostParameters(item));

        /// <summary>
        /// This method computes the total amount charged for tool usage. This is used in all billing reports sent to UofM FinOps
        /// </summary>
        public static decimal GetLineCost(ToolLineCostParameters p)
        {
            // [2015-11-13 jg] this is identical to the logic originally in:
            //      1) sselFinOps.AppCode.BLL.FormulaBL.ApplyToolFormula (for External Invoice)
            //      2) sselIndReports.AppCode.Bll.ToolBillingBL.GetToolBillingDataByClientID20110701 (for User Usage Summary)
            //      3) LNF.WebApi.Billing.Models.ReportUtility.ApplyToolFormula (for SUB reports, note: this is the replacement for the Billing WCF service)
            //
            //      I think at this point all the formulas can be replaced by GetTotalCharge()
            //      because each value used by the formula should correctly reflect the rules
            //      in place during the given period (or at least that is the goal).

            decimal result;

            //if rates are 0 everything must be 0 (this was at the end, but why not do it at the beginning?)
            if (p.ResourceRate + p.PerUseRate == 0)
                return 0;

            int cleanRoomId = 6;
            int maskMakerId = 56000;

            if (BillingTypes.IsMonthlyUserBillingType(p.BillingTypeID)) //not used at this point but maybe in the future
            {
                // Monthly User, charge mask maker for everyone
                if (p.RoomID == cleanRoomId) //Clean Room
                {
                    if (p.ResourceID == maskMakerId) //Mask Maker
                    {
                        if (p.IsStarted)
                            result = p.UsageFeeCharged + p.OverTimePenaltyFee + (p.ResourceRate == 0 ? 0 : p.ReservationFee2);
                        else
                            result = p.UncancelledPenaltyFee + p.ReservationFee2;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    //non clean room tools are always charged for usage fee
                    if (p.IsStarted)
                        result = p.UsageFeeCharged + p.OverTimePenaltyFee + (p.ResourceRate == 0 ? 0 : p.ReservationFee2);
                    else
                        result = p.UncancelledPenaltyFee + p.ReservationFee2;
                }
            }
            else if (p.BillingTypeID == BillingTypes.Other)
            {
                //based on sselIndReports.AppCode.BLL.ToolBillingBL.GetToolBillingDataByClientID20110701 the Other billing type is not set to zero any longer
                result = ToolBillingItem.GetTotalCharge(p.UsageFeeCharged, p.OverTimePenaltyFee, p.BookingFee, p.UncancelledPenaltyFee, p.ReservationFee2);
            }
            else
            {
                //Per Use types
                if (p.Period >= new DateTime(2010, 7, 1))
                {
                    //2011-05 New tool billing started on 2011-04
                    if (p.Period >= new DateTime(2011, 4, 1))
                    {
                        if (!p.IsCancelledBeforeAllowedTime)
                            result = p.UsageFeeCharged + p.OverTimePenaltyFee + p.BookingFee; //should be the same as GetTotalCharge()
                        else
                            result = p.BookingFee; //Cancelled before two hours - should be the same as GetTotalCharge()
                    }
                    else
                    {
                        if (p.IsStarted)
                            result = p.UsageFeeCharged + p.OverTimePenaltyFee + (p.ResourceRate == 0 ? 0 : p.ReservationFee2); //should be the same as GetTotalCharge()
                        else
                            result = p.UncancelledPenaltyFee; //should be the same as GetTotalCharge()
                    }
                }
                else
                {
                    if (p.IsStarted)
                        result = p.UsageFeeCharged + p.OverTimePenaltyFee + (p.ResourceRate == 0 ? 0 : p.ReservationFee2); //should be the same as GetTotalCharge()
                    else
                        result = p.UncancelledPenaltyFee + p.ReservationFee2; //should be the same as GetTotalCharge()
                }
            }

            return result;
        }
    }
}
