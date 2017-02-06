using LNF.CommonTools;
using LNF.Data;
using LNF.Logging;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Billing
{
    public static class ToolBillingUtility
    {
        #region ToolBilling

        /// <summary>
        /// This is the date we started charging users with original reservation time instead of usage time
        /// </summary>
        public static readonly DateTime April2011 = DateTime.Parse("2011-04-01");

        public static IEnumerable<IToolBilling> SelectToolBilling(DateTime period)
        {
            IToolBilling[] items;

            if (period >= DateTime.Now.FirstOfMonth())
                items = DA.Current.Query<ToolBillingTemp>().Where(x => x.Period == period).ToArray();
            else
                items = DA.Current.Query<ToolBilling>().Where(x => x.Period == period).ToArray();

            return items;
        }

        public static IEnumerable<IToolBilling> SelectToolBilling(DateTime period, int clientId)
        {
            IToolBilling[] items;

            if (period >= DateTime.Now.FirstOfMonth())
                items = DA.Current.Query<ToolBillingTemp>().Where(x => x.Period == period && x.ClientID == clientId).ToArray();
            else
                items = DA.Current.Query<ToolBilling>().Where(x => x.Period == period && x.ClientID == clientId).ToArray();

            return items;
        }

        /// <summary>
        /// This is the date we attempted to start using the per use fee. However it didn't actually work until 2011-10-01
        /// </summary>
        public static readonly DateTime June2013 = DateTime.Parse("2013-06-01");

        /// <summary>
        /// This is the date we started using per use fees correctly
        /// </summary>
        public static readonly DateTime October2015 = DateTime.Parse("2015-10-01"); //as of 2011-04-01 and before 2015-10-01 the value of UsageFee2011401 should be used for UsageFeeCharged because billing was based on this column, and therefore we were not charging the per use fees correctly

        public static int UpdateBillingType(Client client, Account acct, BillingType billingType, DateTime period)
        {
            string queryName = "UpdateBillingTypeToolBilling" + ((RepositoryUtility.IsCurrentPeriod(period)) ? "Temp" : string.Empty);
            return DA.Current.QueryBuilder().ApplyParameters(new
            {
                ClientID = client.ClientID,
                AccountID = acct.AccountID,
                BillingTypeID = billingType.BillingTypeID,
                Period = period
            }).NamedQuery(queryName).Update();
        }

        public static int UpdateChargeMultiplierByReservationToolBilling(Reservation rsv)
        {
            // We used to use named queries here which called stored procs to update the ChargeMultiplier, but this was overly
            // complicated. All the stored procs did was just change the ChargeMultiplier so it's a lot simpler to do that here
            // without calling a stored proc, espeically now because we have to get the IToolBilling record anyway to update
            // the UsageFeeCharged amount.

            IToolBilling tb;

            if (rsv.InCurrentPeriod())
                tb = DA.Current.Query<ToolBillingTemp>().FirstOrDefault(x => x.ReservationID == rsv.ReservationID);
            else
                tb = DA.Current.Query<ToolBilling>().FirstOrDefault(x => x.ReservationID == rsv.ReservationID);

            if (tb != null)
            {
                // update the new forgiven pct
                tb.ChargeMultiplier = Convert.ToDecimal(rsv.ChargeMultiplier);

                // calculate the new UsageFeeCharged amount
                tb.CalculateUsageFeeCharged();

                return 1;
            }

            return 0;
        }

        public static int UpdateAccountByReservationToolBilling(this Reservation rsv)
        {
            string queryName = "UpdateAccountToolBilling" + (rsv.InCurrentPeriod() ? "Temp" : string.Empty);
            return DA.Current.QueryBuilder()
                .ApplyParameters(new { ReservationID = rsv.ReservationID, AccountID = rsv.Account.AccountID })
                .NamedQuery(queryName)
                .Update();
        }

        public static IList<ToolBilling> ForSUBReport(DateTime StartPeriod, DateTime EndPeriod, ref IList<SubLineItem> lineItems)
        {
            int[] excludedBillingTypeIds = new int[] { 14, 99 };
            IList<ToolBilling> result = DA.Current.Query<ToolBilling>().Where(x => x.Period >= StartPeriod && x.Period < EndPeriod && x.ChargeTypeID == 5 && !excludedBillingTypeIds.Contains(x.BillingTypeID)).ToList();

            foreach (ToolBilling tb in result)
            {
                if (lineItems.FirstOrDefault(x => x.Period == tb.Period && x.ClientID == tb.ClientID && x.AccountID == tb.AccountID) == null)
                {
                    lineItems.Add(new SubLineItem() { Period = tb.Period, ClientID = tb.ClientID, AccountID = tb.AccountID });
                }
            }

            IList<MiscBillingCharge> miscQuery = DA.Current.Query<MiscBillingCharge>().Where(x => x.Period >= StartPeriod && x.Period < EndPeriod && x.SUBType == "tool" && x.Account.Org.OrgType.ChargeType.ChargeTypeID == 5).ToList();

            foreach (MiscBillingCharge mb in miscQuery)
            {
                if (lineItems.FirstOrDefault(x => x.Period == mb.Period && x.ClientID == mb.Client.ClientID && x.AccountID == mb.Account.AccountID) == null)
                {
                    lineItems.Add(new SubLineItem() { Period = mb.Period, ClientID = mb.Client.ClientID, AccountID = mb.Account.AccountID });
                }
            }

            return result;
        }

        public static void CalculateReservationFee(this IToolBilling item)
        {
            if (item.Period < April2011)
            {
                //2010-11 ReservationFee needs to be caluclated here [not sure why, the formula is exactly the same as the computed column ReservationFeeOld]
                if (item.IsStarted && item.ResourceRate > 0)
                    item.ReservationFee2 = Convert.ToDecimal(item.ReservationRate * item.Uses * item.ChargeMultiplier);
                else
                    item.ReservationFee2 = 0;
            }
            else
            {
                //we stopped charging a reservation fee as of 2011-04-01
                item.ReservationFee2 = 0;
            }

        }

        public static void CalculateUsageFeeCharged(this IToolBilling item)
        {
            //before April 2011
            if (item.Period < April2011)
                item.UsageFeeCharged = Convert.ToDecimal(((item.ChargeMultiplier * ((item.ActDuration - item.OverTime) / 60)) * item.ResourceRate) * (item.IsStarted ? 1 : 0));
            //as of April 2011 and before June 2013
            else if (item.Period >= April2011 && item.Period < June2013)
                item.UsageFeeCharged = Convert.ToDecimal(((item.ChargeMultiplier * (((item.ChargeDuration - item.OverTime) - item.TransferredDuration) / 60)) * item.ResourceRate) * (1 - (item.IsCancelledBeforeAllowedTime ? 1 : 0)));
            //as of October 2015 ["as of June 2013" is no longer true because we continued to use UsageFee20110401 for billing after this date until 2015-10-01. The UsageFeeCharged column should always have the value of *what we actually billed* so I have to overwrite the UsageFeeCharged column with the values in UsageFee20110401 for the date range 2013-06-01 to 2015-10-01]
            else
            {
                decimal amount = 0;

                //first start with AddVal
                amount += Convert.ToDecimal(item.Uses) * item.PerUseRate;

                //next add any charges based on AcctPer and MulVal
                //item.ResourceRate is the MulVal (i.e. per period charge) and item.RatePeriod is AcctPer
                decimal rpc = item.RatePeriodCharge(Convert.ToDecimal(item.ChargeDuration) - item.OverTime - Convert.ToDecimal(item.TransferredDuration));
                amount += rpc;

                item.UsageFeeCharged = Convert.ToDecimal(amount * Convert.ToDecimal(item.ChargeMultiplier) * (1 - (item.IsCancelledBeforeAllowedTime ? 1 : 0)));
            }
        }

        public static void CalculateBookingFee(this IToolBilling item)
        {
            decimal bookingFeePercentage = 0.1M;

            //before April 2011 there was no booking fee concept
            if (item.Period < April2011)
                item.BookingFee = 0;
            //as of April 2011 and before June 2013
            else if (item.Period >= April2011 && item.Period < June2013)
            {
                if (item.IsCancelledBeforeAllowedTime)
                    item.BookingFee = Convert.ToDecimal(item.ResourceRate * (item.MaxReservedDuration / 60)) * bookingFeePercentage * Convert.ToDecimal(item.ChargeMultiplier);
                else
                {
                    //if user made smaller reservation and used it, we still charge those old booking fee
                    if (item.MaxReservedDuration > item.ChargeDuration)
                        item.BookingFee = Convert.ToDecimal(item.ResourceRate * ((item.MaxReservedDuration - item.ChargeDuration) / 60)) * bookingFeePercentage * Convert.ToDecimal(item.ChargeMultiplier);
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
                    //only charge a booking fee on the PerUseRate when the reservation was canceled
                    amount += Convert.ToDecimal(item.Uses) * item.PerUseRate;
                    amount += item.RatePeriodCharge(Convert.ToDecimal(item.MaxReservedDuration));
                }
                else
                {
                    if (item.MaxReservedDuration > item.ChargeDuration)
                    {
                        //User modified the reservation to make it smaller so booking fee gets
                        //charged on the difference. In this case it doesn't make sense to also
                        //inlcude the per use charge (if any) because they will be charged for
                        //that regardless.
                        amount += item.RatePeriodCharge(Convert.ToDecimal(item.MaxReservedDuration - item.ChargeDuration - item.TransferredDuration));
                    }
                }

                item.BookingFee = amount * bookingFeePercentage * Convert.ToDecimal(item.ChargeMultiplier);
            }
        }

        public static decimal RatePeriodCharge(this IToolBilling item, decimal duration)
        {
            decimal factor;

            if (duration == 0) return 0;

            switch (item.RatePeriod)
            {
                case "Hourly":
                    factor = 1M / 60M;
                    break;
                case "Daily":
                    factor = 1M / 60M / 24M;
                    break;
                //are these the same?
                case "Monthly":
                case "Per Month":
                    factor = 1M / 60M / 24M / NumberOfDaysInMonth(item.Period);
                    break;
                case "Per Use":
                    factor = 1M / duration;
                    break;
                case "None":
                default:
                    factor = 0M;
                    break;
            }

            // Rounding to two decmial places because that is what is displayed on the User Usage Summary
            decimal result = Math.Round(duration * factor, 2) * Convert.ToDecimal(item.ResourceRate);

            return result;
        }

        public static int NumberOfDaysInMonth(DateTime d)
        {
            DateTime fom = new DateTime(d.Year, d.Month, 1);
            DateTime nextMonth = fom.AddMonths(1);
            DateTime lastDayOfMonth = nextMonth.AddDays(-1);
            return lastDayOfMonth.Day;
        }

        public static IToolBilling CreateToolBillingItem(bool isTemp)
        {
            IToolBilling result;

            if (isTemp)
                result = new ToolBillingTemp();
            else
                result = new ToolBilling();

            return result;
        }

        public static IToolBilling CreateToolBillingFromDataRow(DataRow dr, bool isTemp)
        {
            IToolBilling item = CreateToolBillingItem(isTemp);

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

        #endregion

        #region ToolDataRaw

        public static IList<ToolDataRaw> DataFiltered(DateTime sd, DateTime ed, int clientId, int resourceId)
        {
            //if a ActDuration is longer than the max schedulable, chop it
            var query = DA.Scheduler.Reservation.Query().Where(x =>
                x.ActualEndDateTime != null &&
                ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)));

            var result = ToolBillingUtility.DataRaw(sd, query);

            foreach (var item in result)
            {
                //2009-08-02 Sandrine said there is no upper limit on any reservation, so i have to comment out this code
                if (item.OverTime < 0)
                    item.OverTime = 0;
            }

            return result;
        }

        public static IList<ToolDataRaw> DataRaw(DateTime period, IEnumerable<Reservation> reservations)
        {
            //this does all the same stuff as sselScheduler.dbo.SSEL_DataRead @Action = 'ToolDataRaw'

            List<ToolDataRaw> result = new List<ToolDataRaw>();

            foreach (Reservation rsv in reservations)
            {
                //first, update reservations that have a bad/wrong ActualEndDateTime
                //1) was not cancelled, activity does not allow exceeding the max, was started and has not ended
                if (rsv.IsActive && rsv.Activity.NoMaxSchedAuth == 0 && (rsv.ActualBeginDateTime != null && rsv.ActualEndDateTime == null))
                {
                    //2) use BeginDateTime when the reservation was started early, otherwise use ActualEndDateTime
                    DateTime sdate = (rsv.ActualBeginDateTime.Value < rsv.BeginDateTime) ? rsv.BeginDateTime : rsv.ActualBeginDateTime.Value;
                    if (sdate.AddMinutes(rsv.Resource.MaxReservTime) < DateTime.Now) //is exceeding the resource max duration
                    {
                        //3) use the duration when the resource config was changed, otherwise use the resource max reservation timem (duration can exceed max when tool config is changed after reservation was created)
                        double minutesToAdd = (rsv.Duration < rsv.Resource.MaxReservTime) ? Convert.ToDouble(rsv.Resource.MaxReservTime) : rsv.Duration;
                        rsv.ActualEndDateTime = sdate.AddMinutes(minutesToAdd);
                        rsv.ClientIDEnd = -1; //auto-end
                    }
                }

                //the next four are all possible combinations for reservations ended by other users, the +1 is to account for the time it takes the service to run
                //1) was not cancelled, activity does not allow exceeding the max, was started and has ended, started and ended by different clients
                if (rsv.IsActive && rsv.Activity.NoMaxSchedAuth == 0 && (rsv.ActualBeginDateTime != null && rsv.ActualEndDateTime != null) && rsv.ClientIDBegin != rsv.ClientIDEnd)
                {
                    //2) use BeginDateTime when the reservation was started early, otherwise use ActualEndDateTime
                    DateTime sdate = (rsv.ActualBeginDateTime.Value < rsv.BeginDateTime) ? rsv.BeginDateTime : rsv.ActualBeginDateTime.Value;
                    int autoEnd = Math.Max(0, rsv.Resource.AutoEnd); //use 0 when AutoEnd = -1
                    if ((rsv.ActualEndDateTime.Value - sdate).TotalMinutes > 1 + rsv.Resource.MaxReservTime + autoEnd) //exceeds resource max duration plus auto-end plus 1 for service overhead
                    {
                        //3) use the duration when the resource config was changed, otherwise use the resource max reservation timem (duration can exceed max when tool config is changed after reservation was created)
                        double minutesToAdd = (rsv.Duration < rsv.Resource.MaxReservTime) ? Convert.ToDouble(rsv.Resource.MaxReservTime) : rsv.Duration;
                        rsv.ActualEndDateTime = sdate.AddMinutes(minutesToAdd + Convert.ToDouble(autoEnd));
                    }
                }

                if (period >= April2011)
                {
                    DateTime sdate = period;
                    DateTime edate = sdate.AddMonths(1);

                    //2011-03-28 All other Unactivated Reservation
                    //1) either the scheduled range or actual range overlap with the period range
                    if (rsv.IsReservationInDateRange(sdate, edate))
                    {
                        //2) not started and not ended
                        if (rsv.ActualBeginDateTime == null && rsv.ActualEndDateTime == null)
                        {
                            //3) scheduled end time is in the past
                            if (rsv.EndDateTime < DateTime.Now)
                            {
                                rsv.ActualBeginDateTime = rsv.BeginDateTime;
                                rsv.ActualEndDateTime = rsv.EndDateTime;
                                rsv.ClientIDBegin = (rsv.ClientIDBegin == null) ? -1 : rsv.ClientIDBegin;
                                rsv.ClientIDEnd = (rsv.ClientIDEnd == null) ? -1 : rsv.ClientIDEnd;
                            }
                        }
                    }
                }

                result.Add(DA.Current.Single<ToolDataRaw>(rsv.ReservationID));
            }

            return result;
        }
        #endregion

        #region ToolDataClean
        public static IList<ToolDataClean> PopulateToolDataClean(DateTime period, IList<Reservation> reservations)
        {
            int reservationCount = reservations.Count;
            int rowsDeletedFromToolDataClean = 0;
            int rowsInsertedIntoToolDataClean = 0;

            using (LogTaskTimer.Start("ToolBillingUtility.PopulateToolDataClean", "period = '{0:yyyy-MM-dd}', reservationCount = {1}, rowsDeletedFromToolDataClean = {2}, rowsInsertedIntoToolDataClean = {3}", () => new object[] { period, reservationCount, rowsDeletedFromToolDataClean, rowsInsertedIntoToolDataClean }))
            {
                if (reservationCount == 0) return null;

                //does the same crap as in sselScheduler.dbo.SSEL_DataRead @Action = 'ToolDataRaw'
                IList<ToolDataRaw> data = ToolBillingUtility.DataRaw(period, reservations);

                List<ToolDataClean> insert = new List<ToolDataClean>();
                List<ToolDataClean> delete = new List<ToolDataClean>();
                int minReservationId = reservations.Select(r => r.ReservationID).Min();

                IList<ToolDataClean> all = DA.Current.Query<ToolDataClean>().Where(x => x.ReservationID >= minReservationId).ToArray();

                //add new rows
                foreach (ToolDataRaw item in data)
                {
                    //delete existing rows
                    IEnumerable<ToolDataClean> existing = all.Where(x => x.ReservationID == item.ReservationID);
                    if (existing.Count() > 0) delete.AddRange(existing);

                    //ActualBeginDateTime and ActualEndDateTime should both have values because of ReservationUtility.PrepareReservationsForBilling
                    ToolDataClean tdc = new ToolDataClean()
                    {
                        ClientID = item.ClientID,
                        ResourceID = item.ResourceID,
                        RoomID = item.RoomID,
                        BeginDateTime = item.BeginDateTime,
                        EndDateTime = item.EndDateTime,
                        ActualBeginDateTime = item.ActualBeginDateTime.Value,
                        ActualEndDateTime = item.ActualEndDateTime.Value,
                        AccountID = item.AccountID,
                        ActivityID = item.ActivityID,
                        SchedDuration = item.SchedDuration,
                        ActDuration = item.ActDuration,
                        OverTime = item.OverTime,
                        IsStarted = item.IsStarted,
                        ChargeMultiplier = item.ChargeMultiplier,
                        ReservationID = item.ReservationID,
                        MaxReservedDuration = item.MaxReservedDuration,
                        IsActive = item.IsActive,
                        CancelledDateTime = item.CancelledDateTime,
                        OriginalBeginDateTime = item.OriginalBeginDateTime,
                        OriginalEndDateTime = item.OriginalEndDateTime,
                        OriginalModifiedOn = item.OriginalModifiedOn,
                        CreatedOn = item.CreatedOn
                    };

                    insert.Add(tdc);
                }

                DA.Current.Delete(delete);
                rowsDeletedFromToolDataClean = delete.Count;

                DA.Current.Insert(insert);
                rowsInsertedIntoToolDataClean = insert.Count;

                return insert;
            }
        }

        public static int UpdateChargeMultiplierByReservationToolDataClean(Reservation rsv)
        {
            return DA.Current.QueryBuilder()
                .ApplyParameters(new
                {
                    ReservationID = rsv.ReservationID,
                    ChargeMultiplier = rsv.ChargeMultiplier
                }).NamedQuery("UpdateChargeMultiplierToolDataClean").Update();
        }

        public static int UpdateAccountByReservationToolDataClean(Reservation rsv)
        {
            return DA.Current.QueryBuilder()
                .ApplyParameters(new
                {
                    ReservationID = rsv.ReservationID,
                    AccountID = rsv.Account.AccountID
                }).NamedQuery("UpdateAccountToolDataClean").Update();
        }
        #endregion

        #region ToolData
        public static int UpdateChargeMultiplierByReservationToolData(Reservation rsv)
        {
            return DA.Current.QueryBuilder()
                .ApplyParameters(new
                {
                    ReservationID = rsv.ReservationID,
                    ChargeMultiplier = rsv.ChargeMultiplier
                }).NamedQuery("UpdateChargeMultiplierToolData").Update();
        }

        public static int UpdateAccountByReservationToolData(Reservation rsv)
        {
            return DA.Current.QueryBuilder()
                .ApplyParameters(new
                {
                    ReservationID = rsv.ReservationID,
                    AccountID = rsv.Account.AccountID
                }).NamedQuery("UpdateAccountToolData").Update();
        }

        public static int MinimumDaysForApportionment(this ClientOrg co, Room r, DateTime period)
        {
            IList<ToolData> query = DA.Current.Query<ToolData>().Where(x => x.ClientID == co.Client.ClientID && x.Period == period).Where(x => DA.Current.Single<Account>(x.AccountID).Org == co.Org).ToList();
            IEnumerable<DateTime> days = query.Where(x => x.GetReservation().Resource.ProcessTech.Lab.Room == r && x.GetReservation().IsStarted).Select(x => x.ActDate);
            IEnumerable<int> distinctDays = days.Select(x => x.Day).Distinct();
            int result = distinctDays.Count();
            return result;
        }
        #endregion
    }
}
