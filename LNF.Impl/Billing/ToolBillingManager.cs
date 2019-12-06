﻿using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class ToolBillingManager : ManagerBase, IToolBillingManager
    {
        public ToolBillingManager(IProvider provider) : base(provider) { }

        public IEnumerable<IToolBilling> CreateToolBilling(DateTime period, int clientId = 0)
        {
            var temp = period == DateTime.Now.FirstOfMonth();
            var source = BillingDataProcessStep1.GetToolData(period, clientId, 0, temp);
            var result = CreateToolBillingItems(source, temp);
            return result;
        }

        public IEnumerable<IToolBilling> CreateToolBilling(int reservationId)
        {
            var td = Session.Query<ToolData>().FirstOrDefault(x => x.ReservationID == reservationId);
            if (td == null) return null;
            var period = td.Period;
            var temp = period == DateTime.Now.FirstOfMonth();
            var source = BillingDataProcessStep1.GetToolData(period, 0, reservationId, temp);
            var result = CreateToolBillingItems(source, temp);
            return result;
        }

        public IEnumerable<IToolData> CreateToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            var proc = new WriteToolDataProcess(period, clientId, resourceId);
            var dtExtract = proc.Extract();
            var dtTransform = proc.Transform(dtExtract);

            var result = dtTransform.AsEnumerable().Select(x => new ToolDataItem
            {
                ToolDataID = x.Field<int>("ToolDataID"),
                Period = x.Field<DateTime>("Period"),
                ClientID = x.Field<int>("ClientID"),
                ResourceID = x.Field<int>("ResourceID"),
                RoomID = x.Field<int?>("RoomID"),
                ActDate = x.Field<DateTime>("ActDate"),
                AccountID = x.Field<int>("AccountID"),
                Uses = x.Field<double>("Uses"),
                SchedDuration = x.Field<double>("SchedDuration"),
                ActDuration = x.Field<double>("ActDuration"),
                OverTime = x.Field<double>("OverTime"),
                Days = x.Field<double?>("Days"),
                Months = x.Field<double?>("Months"),
                IsStarted = x.Field<bool>("IsStarted"),
                ChargeMultiplier = x.Field<double>("ChargeMultiplier"),
                ReservationID = x.Field<int?>("ReservationID"),
                ChargeDuration = x.Field<double>("ChargeDuration"),
                TransferredDuration = x.Field<double>("TransferredDuration"),
                MaxReservedDuration = x.Field<double>("MaxReservedDuration"),
                ChargeBeginDateTime = x.Field<DateTime?>("ChargeBeginDateTime"),
                ChargeEndDateTime = x.Field<DateTime?>("ChargeEndDateTime"),
                IsActive = x.Field<bool>("IsActive"),
                IsCancelledBeforeAllowedTime = x.Field<bool?>("IsCancelledBeforeAllowedTime")
            }).ToList();

            return result;
        }

        public IEnumerable<IToolData> CreateToolData(int reservationId)
        {
            var tdc = Session.Query<ToolDataClean>().FirstOrDefault(x => x.ReservationID == reservationId);
            if (tdc == null) return null;
            var period = tdc.GetChargeBeginDateTime().FirstOfMonth();
            // Doing it the lazy way for one reservation: create all for the client/tool and then return one.
            var items = CreateToolData(period, tdc.ClientID, tdc.ResourceID);
            var result = items.Where(x => x.ReservationID == reservationId);
            return result;
        }

        public IEnumerable<IToolBilling> GetToolBilling(DateTime period, int clientId = 0, int resourceId = 0)
        {
            var temp = period == DateTime.Now.FirstOfMonth();

            var query = GetToolBillingQuery(temp).Where(x =>
                x.Period == period
                && x.ClientID == (clientId > 0 ? clientId : x.ClientID)
                && x.ResourceID == (resourceId > 0 ? resourceId : x.ResourceID));

            var result = query.CreateModels<IToolBilling>();

            return result;
        }

        public IEnumerable<IToolBilling> GetToolBilling(int reservationId)
        {
            var td = Session.Query<ToolData>().Where(x => x.ReservationID == reservationId).ToList();
            if (td.Count == 0) return null;
            var period = td.First().Period;
            var temp = period == DateTime.Now.FirstOfMonth();
            var query = GetToolBillingQuery(temp).Where(x => x.ReservationID == reservationId);
            var result = query.CreateModels<IToolBilling>();
            return result;
        }

        public IEnumerable<IToolData> GetToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            var query = Session.Query<ToolData>()
                .Where(x => x.Period == period
                    && x.ClientID == (clientId > 0 ? clientId : x.ClientID)
                    && x.ResourceID == (resourceId > 0 ? resourceId : x.ResourceID));

            var result = query.CreateModels<IToolData>();

            return result;
        }

        public IEnumerable<IToolData> GetToolData(int reservationId)
        {
            var query = Session.Query<ToolData>().Where(x => x.ReservationID == reservationId);
            var result = query.CreateModels<IToolData>();
            return result;
        }

        public IEnumerable<IToolDataClean> GetToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0)
        {
            var query = Session.Query<ToolDataClean>()
                .Where(x => (x.BeginDateTime < ed && x.EndDateTime > sd || x.ActualBeginDateTime < ed && x.ActualEndDateTime > ed)
                    && x.ClientID == (clientId > 0 ? clientId : x.ClientID)
                    && x.ResourceID == (resourceId > 0 ? resourceId : x.ResourceID));

            var result = query.CreateModels<IToolDataClean>();

            return result;
        }

        public IToolDataClean GetToolDataClean(int reservationId)
        {
            var tdc = Session.Query<ToolDataClean>().FirstOrDefault(x => x.ReservationID == reservationId);
            var result = tdc.CreateModel<IToolDataClean>();
            return result;
        }

        public int MinimumDaysForApportionment(IClient co, IRoom r, DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBilling> SelectToolBilling(DateTime period)
        {
            return GetToolBillingQuery(Utility.IsCurrentPeriod(period)).Where(x => x.Period == period).CreateModels<IToolBilling>();
        }

        public IEnumerable<IToolBilling> SelectToolBilling(DateTime period, int clientId)
        {
            return GetToolBillingQuery(Utility.IsCurrentPeriod(period)).Where(x => x.Period == period && x.ClientID == clientId).CreateModels<IToolBilling>();
        }

        public int UpdateAccountByReservationToolBilling(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolData(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolDataClean(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolData(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolDataClean(IReservation rsv)
        {
            throw new NotImplementedException();
        }

        #region ToolBilling

        /// <summary>
        /// This is the date we started charging users with original reservation time instead of usage time
        /// </summary>
        public static readonly DateTime April2011 = DateTime.Parse("2011-04-01");

        /// <summary>
        /// This is the date we attempted to start using the per use fee. However it didn't actually work until 2011-10-01
        /// </summary>
        public static readonly DateTime June2013 = DateTime.Parse("2013-06-01");

        /// <summary>
        /// This is the date we started using per use fees correctly
        /// </summary>
        public static readonly DateTime October2015 = DateTime.Parse("2015-10-01"); //as of 2011-04-01 and before 2015-10-01 the value of UsageFee2011401 should be used for UsageFeeCharged because billing was based on this column, and therefore we were not charging the per use fees correctly

        public int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period)
        {
            string queryName = "UpdateBillingTypeToolBilling" + ((Utility.IsCurrentPeriod(period)) ? "Temp" : string.Empty);

            var query = Session.NamedQuery(queryName)
                .SetParameter("ClientID", clientId)
                .SetParameter("AccountID", accountId)
                .SetParameter("BillingTypeID", billingTypeId)
                .SetParameter("Period", period);

            return query.Update();
        }

        public int UpdateChargeMultiplierByReservationToolBilling(IReservation rsv)
        {
            // We used to use named queries here which called stored procs to update the ChargeMultiplier, but this was overly
            // complicated. All the stored procs did was just change the ChargeMultiplier so it's a lot simpler to do that here
            // without calling a stored proc, espeically now because we have to get the IToolBilling record anyway to update
            // the UsageFeeCharged amount.

            var tb = GetToolBillingQuery(Utility.InCurrentPeriod(rsv)).FirstOrDefault(x => x.ReservationID == rsv.ReservationID).CreateModel<IToolBilling>();

            if (tb != null)
            {
                // update the new forgiven pct
                tb.ChargeMultiplier = Convert.ToDecimal(rsv.ChargeMultiplier);

                // calculate the new UsageFeeCharged amount
                CalculateUsageFeeCharged(tb);

                return 1;
            }

            return 0;
        }

        public int UpdateAccountByReservationToolBilling(Reservation rsv)
        {
            string queryName = "UpdateAccountToolBilling" + (rsv.InCurrentPeriod() ? "Temp" : string.Empty);
            return Session.NamedQuery(queryName)
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("AccountID", rsv.Account.AccountID)
                .Update();
        }

        public IEnumerable<IToolBilling> ForSUBReport(DateTime StartPeriod, DateTime EndPeriod, IList<SubLineItem> lineItems)
        {
            int[] excludedBillingTypeIds = new int[] { 14, 99 };
            var result = Session.Query<Repository.Billing.ToolBilling>().Where(x => x.Period >= StartPeriod && x.Period < EndPeriod && x.ChargeTypeID == 5 && !excludedBillingTypeIds.Contains(x.BillingTypeID)).ToList();

            foreach (var tb in result)
            {
                if (lineItems.FirstOrDefault(x => x.Period == tb.Period && x.ClientID == tb.ClientID && x.AccountID == tb.AccountID) == null)
                {
                    lineItems.Add(new SubLineItem() { Period = tb.Period, ClientID = tb.ClientID, AccountID = tb.AccountID });
                }
            }

            var miscQuery = Session.Query<Repository.Billing.MiscBillingCharge>().Where(x => x.Period >= StartPeriod && x.Period < EndPeriod && x.SubType == "tool" && x.Account.Org.OrgType.ChargeType.ChargeTypeID == 5).ToList();

            foreach (var mb in miscQuery)
            {
                if (lineItems.FirstOrDefault(x => x.Period == mb.Period && x.ClientID == mb.Client.ClientID && x.AccountID == mb.Account.AccountID) == null)
                {
                    lineItems.Add(new SubLineItem() { Period = mb.Period, ClientID = mb.Client.ClientID, AccountID = mb.Account.AccountID });
                }
            }

            return result.CreateModels<IToolBilling>();
        }

        public void CalculateReservationFee(IToolBilling item)
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

        public void CalculateUsageFeeCharged(IToolBilling item)
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

                //next add any charges based on AcctPer and MulVal
                //item.ResourceRate is the MulVal (i.e. per period charge) and item.RatePeriod is AcctPer
                decimal rpc = RatePeriodCharge(item, item.ChargeDuration - item.OverTime - item.TransferredDuration);
                amount += rpc;

                item.UsageFeeCharged = amount * item.ChargeMultiplier * (1 - (item.IsCancelledBeforeAllowedTime ? 1 : 0));
            }
        }

        public void CalculateBookingFee(IToolBilling item)
        {
            decimal bookingFeePercentage = 0.1M;

            //before April 2011 there was no booking fee concept
            if (item.Period < April2011)
                item.BookingFee = 0;
            //as of April 2011 and before June 2013
            else if (item.Period >= April2011 && item.Period < June2013)
            {
                if (item.IsCancelledBeforeAllowedTime)
                    item.BookingFee = item.ResourceRate * (item.MaxReservedDuration / 60) * bookingFeePercentage * item.ChargeMultiplier;
                else
                {
                    //if user made smaller reservation and used it, we still charge those old booking fee
                    if (item.MaxReservedDuration > item.ChargeDuration)
                        item.BookingFee = item.ResourceRate * ((item.MaxReservedDuration - item.ChargeDuration) / 60) * bookingFeePercentage * item.ChargeMultiplier;
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

                item.BookingFee = amount * bookingFeePercentage * item.ChargeMultiplier;
            }
        }

        public decimal RatePeriodCharge(IToolBilling item, decimal duration)
        {
            double factor;
            double dur = Convert.ToDouble(duration);
            double rate = Convert.ToDouble(item.ResourceRate);

            if (dur == 0) return 0;

            switch (item.RatePeriod)
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
                    factor = 1D / 60D / 24D / Utility.NumberOfDaysInMonth(item.Period);
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

            var method = item.Period < DateTime.Parse("2018-07-01") ? 1 : 2;
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

        #endregion

        #region ToolDataRaw

        public IEnumerable<IToolDataRaw> DataFiltered(DateTime sd, DateTime ed, int clientId, int resourceId)
        {
            //if a ActDuration is longer than the max schedulable, chop it
            var query = Session.Query<ReservationInfo>()
                .Where(x =>
                    x.ActualEndDateTime != null &&
                    ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)))
                .CreateModels<IReservation>();

            var result = DataRaw(sd, query);

            foreach (var item in result)
            {
                //2009-08-02 Sandrine said there is no upper limit on any reservation, so i have to comment out this code
                if (item.OverTime < 0)
                    item.OverTime = 0;
            }

            return result;
        }

        public IEnumerable<IToolDataRaw> DataRaw(DateTime period, IEnumerable<IReservation> reservations)
        {
            //this does all the same stuff as sselScheduler.dbo.SSEL_DataRead @Action = 'ToolDataRaw'

            var result = new List<IToolDataRaw>();

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

                var tdr = Session.Single<Repository.Billing.ToolDataRaw>(rsv.ReservationID).CreateModel<IToolDataRaw>();
                result.Add(tdr);
            }

            return result;
        }
        #endregion

        #region ToolDataClean

        public int UpdateChargeMultiplierByReservationToolDataClean(Reservation rsv)
        {
            return Session.NamedQuery("UpdateChargeMultiplierToolDataClean")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("ChargeMultiplier", rsv.ChargeMultiplier)
                .Update();
        }

        public int UpdateAccountByReservationToolDataClean(Reservation rsv)
        {
            return Session.NamedQuery("UpdateAccountToolDataClean")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("AccountID", rsv.Account.AccountID)
                .Update();
        }
        #endregion

        #region ToolData
        public int UpdateChargeMultiplierByReservationToolData(Reservation rsv)
        {
            return Session.NamedQuery("UpdateChargeMultiplierToolData")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("ChargeMultiplier", rsv.ChargeMultiplier)
                .Update();
        }

        public int UpdateAccountByReservationToolData(Reservation rsv)
        {
            return Session.NamedQuery("UpdateAccountToolData")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("AccountID", rsv.Account.AccountID)
                .Update();
        }

        public int MinimumDaysForApportionment(ClientOrg co, Room r, DateTime period)
        {
            var query = Session.Query<ToolData>().Where(x => x.ClientID == co.Client.ClientID && x.Period == period).Where(x => Session.Single<Account>(x.AccountID).Org == co.Org);

            var join = query.Join(Session.Query<Reservation>(), o => o.ReservationID, i => i.ReservationID, (o, i) => new { ToolData = o, Reservation = i });

            var days = join
                .Where(x => x.Reservation.Resource.ProcessTech.Lab.Room == r && x.Reservation.IsStarted)
                .Select(x => x.ToolData.ActDate).ToList();

            var distinctDays = days.Select(x => x.Day).Distinct();

            var result = distinctDays.Count();

            return result;
        }
        #endregion

        private IEnumerable<IToolBilling> CreateToolBillingItems(IEnumerable<IToolBilling> source, bool temp)
        {
            var step1 = new BillingDataProcessStep1(DateTime.Now, ServiceProvider.Current);

            foreach (var tb in source)
                step1.CalculateToolBillingCharges(tb);

            return source;
        }

        private IQueryable<Repository.Billing.IToolBilling> GetToolBillingQuery(bool temp)
        {
            if (temp)
                return Session.Query<Repository.Billing.ToolBillingTemp>();
            else
                return Session.Query<Repository.Billing.ToolBilling>();
        }

        public IEnumerable<IToolDataRaw> DataRaw(DateTime period, IEnumerable<IToolBillingReservation> reservations)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolBilling(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolData(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateAccountByReservationToolDataClean(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolBilling(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolData(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public int UpdateChargeMultiplierByReservationToolDataClean(IToolBillingReservation rsv)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IToolBillingReservation> SelectReservations(DateTime startDate, DateTime endDate, int resourceId)
        {
            var dt = DA.Command(CommandType.StoredProcedure)
                .Param("Action", "SelectReservationsForTransferredDuration")
                .Param("StartDate", startDate)
                .Param("EndDate", endDate)
                .Param("ResourceID", resourceId > 0, resourceId)
                .FillDataTable("sselScheduler.dbo.procReservationSelect");

            var result = new List<ToolBillingReservation>();

            foreach(DataRow dr in dt.Rows)
            {
                var item = new ToolBillingReservation
                {
                    ReservationID = dr.Field<int>("ReservationID"),
                    ResourceID = dr.Field<int>("ResourceID"),
                    ResourceName = dr.Field<string>("ResourceName"),
                    ProcessTechID = dr.Field<int>("ProcessTechID"),
                    ProcessTechName = dr.Field<string>("ProcessTechName"),
                    ClientID = dr.Field<int>("ClientID"),
                    UserName = dr.Field<string>("UserName"),
                    LName = dr.Field<string>("LName"),
                    FName = dr.Field<string>("FName"),
                    ActivityID = dr.Field<int>("ActivityID"),
                    ActivityName = dr.Field<string>("ActivityName"),
                    AccountID = dr.Field<int>("AccountID"),
                    AccountName = dr.Field<string>("AccountName"),
                    ShortCode = dr.Field<string>("ShortCode"),
                    ChargeTypeID = dr.Field<int>("ChargeTypeID"),
                    IsActive = dr.Field<bool>("IsActive"),
                    IsStarted = dr.Field<bool>("IsStarted"),
                    BeginDateTime = dr.Field<DateTime>("BeginDateTime"),
                    EndDateTime = dr.Field<DateTime>("EndDateTime"),
                    ActualBeginDateTime = dr.Field<DateTime?>("ActualBeginDateTime"),
                    ActualEndDateTime = dr.Field<DateTime?>("ActualEndDateTime"),
                    CancelledDateTime = dr.Field<DateTime?>("CancelledDateTime"),
                    ChargeBeginDateTime = dr.Field<DateTime>("ChargeBeginDateTime"),
                    ChargeEndDateTime = dr.Field<DateTime>("ChargeEndDateTime"),
                    LastModifiedOn = dr.Field<DateTime>("LastModifiedOn"),
                    ChargeMultiplier = dr.Field<double>("ChargeMultiplier")
                };

                result.Add(item);
            }

            return result;
        }
    }
}
