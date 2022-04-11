using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class ToolBillingRepository : BillingRepository, IToolBillingRepository
    {
        public ICostRepository Cost { get; }

        public ToolBillingRepository(ICostRepository cost, ISessionManager mgr) : base(mgr)
        {
            Cost = cost;
        }

        public IEnumerable<IToolBilling> CreateToolBilling(DateTime period, int clientId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var now = DateTime.Now;
                var temp = period == now.FirstOfMonth();
                var step1 = new BillingDataProcessStep1(new Step1Config { Connection = conn, Context = "ToolBillingRepository.CreateToolBilling", Period = period, Now = now, ClientID = clientId, IsTemp = temp });
                var source = step1.GetToolData(0);
                var result = CreateToolBillingItems(source);
                conn.Close();
                return result;
            }
        }

        public IEnumerable<IToolBilling> CreateToolBilling(int reservationId)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var now = DateTime.Now;
                var period = GetPeriod(conn, reservationId);
                var temp = period == now.FirstOfMonth();
                var step1 = new BillingDataProcessStep1(new Step1Config { Connection = conn, Context = "ToolBillingRepository.CreateToolBilling", Period = period, Now = now, ClientID = 0, IsTemp = temp });
                var source = step1.GetToolData(reservationId);
                var result = CreateToolBillingItems(source);
                conn.Close();
                return result;
            }
        }

        public IEnumerable<IToolData> CreateToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();

                var proc = new WriteToolDataProcess(WriteToolDataConfig.Create(conn, "ToolBillingRepository.CreateToolData", period, clientId, resourceId, Cost.GetToolCosts(period, resourceId)));
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

                conn.Close();

                return result;
            }
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

        public IEnumerable<IToolBilling> GetToolBilling(DateTime period, int clientId = 0, int resourceId = 0, int accountId = 0)
        {
            var temp = period == DateTime.Now.FirstOfMonth();

            var result = GetToolBillingQuery(temp).Where(x =>
                x.Period == period
                && x.ClientID == (clientId > 0 ? clientId : x.ClientID)
                && x.ResourceID == (resourceId > 0 ? resourceId : x.ResourceID)
                && x.AccountID == (accountId > 0 ? accountId : x.AccountID));

            return result;
        }

        public IEnumerable<IToolBilling> GetToolBilling(int reservationId)
        {
            var td = Session.Query<ToolData>().Where(x => x.ReservationID == reservationId).ToList();
            if (td.Count == 0) return null;
            var period = td.First().Period;
            var temp = period == DateTime.Now.FirstOfMonth();
            var result = GetToolBillingQuery(temp).Where(x => x.ReservationID == reservationId).ToList();
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
            return GetToolBillingQuery(Utility.IsCurrentPeriod(period)).Where(x => x.Period == period);
        }

        public IEnumerable<IToolBilling> SelectToolBilling(DateTime period, int clientId)
        {
            return GetToolBillingQuery(Utility.IsCurrentPeriod(period)).Where(x => x.Period == period && x.ClientID == clientId);
        }

        #region ToolBilling

        /// <summary>
        /// This is the date we started using per use fees correctly
        /// </summary>
        public static readonly DateTime October2015 = DateTime.Parse("2015-10-01"); //as of 2011-04-01 and before 2015-10-01 the value of UsageFee2011401 should be used for UsageFeeCharged because billing was based on this column, and therefore we were not charging the per use fees correctly

        public int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period) => Session.UpdateToolBillingType(clientId, accountId, billingTypeId, period);

        public int UpdateChargeMultiplierByReservationToolBilling(IReservation rsv)
        {
            // We used to use named queries here which called stored procs to update the ChargeMultiplier, but this was overly
            // complicated. All the stored procs did was just change the ChargeMultiplier so it's a lot simpler to do that here
            // without calling a stored proc, espeically now because we have to get the IToolBilling record anyway to update
            // the UsageFeeCharged amount.

            var tb = GetToolBillingQuery(Utility.InCurrentPeriod(rsv)).FirstOrDefault(x => x.ReservationID == rsv.ReservationID);

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
            return Session.GetNamedQuery(queryName)
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("AccountID", rsv.Account.AccountID)
                .ExecuteUpdate();
        }

        public IEnumerable<IToolBilling> ForSUBReport(DateTime StartPeriod, DateTime EndPeriod, IList<SubLineItem> lineItems)
        {
            int[] excludedBillingTypeIds = new int[] { 14, 99 };
            var result = Session.Query<ToolBilling>().Where(x => x.Period >= StartPeriod && x.Period < EndPeriod && x.ChargeTypeID == 5 && !excludedBillingTypeIds.Contains(x.BillingTypeID)).ToList();

            foreach (var tb in result)
            {
                if (lineItems.FirstOrDefault(x => x.Period == tb.Period && x.ClientID == tb.ClientID && x.AccountID == tb.AccountID) == null)
                {
                    lineItems.Add(new SubLineItem() { Period = tb.Period, ClientID = tb.ClientID, AccountID = tb.AccountID });
                }
            }

            MiscBillingCharge mbc = null;
            Account acct = null;
            Org org = null;
            OrgType ot = null;
            ChargeType ct = null;

            var miscQuery = Session.QueryOver(() => mbc)
                .JoinEntityAlias(() => acct, () => mbc.AccountID == acct.AccountID)
                .JoinEntityAlias(() => org, () => acct.Org == org)
                .JoinEntityAlias(() => ot, () => org.OrgType == ot)
                .JoinEntityAlias(() => ct, () => ot.ChargeType == ct)
                .Where(() => mbc.Period >= StartPeriod && mbc.Period < EndPeriod && mbc.SUBType == "tool" && ct.ChargeTypeID == 5)
                .List<MiscBillingCharge>();

            foreach (var mb in miscQuery)
            {
                if (lineItems.FirstOrDefault(x => x.Period == mb.Period && x.ClientID == mb.ClientID && x.AccountID == mb.AccountID) == null)
                {
                    lineItems.Add(new SubLineItem() { Period = mb.Period, ClientID = mb.ClientID, AccountID = mb.AccountID });
                }
            }

            return result;
        }

        public void CalculateReservationFee(IToolBilling item) => ToolBillingUtility.CalculateReservationFee(item);

        public void CalculateUsageFeeCharged(IToolBilling item) => ToolBillingUtility.CalculateUsageFeeCharged(item);

        public void CalculateBookingFee(IToolBilling item) => ToolBillingUtility.CalculateBookingFee(item);

        public decimal RatePeriodCharge(IToolBilling item, decimal duration) => ToolBillingUtility.RatePeriodCharge(item, duration);

        public decimal GetLineCost(ToolLineCostParameters parameters) => ToolBillingUtility.GetLineCost(parameters);

        #endregion

        #region ToolDataRaw

        public IEnumerable<IToolDataRaw> DataFiltered(DateTime sd, DateTime ed, int clientId, int resourceId)
        {
            //if a ActDuration is longer than the max schedulable, chop it
            var query = Session.Query<ReservationInfo>()
                .Where(x =>
                    x.ActualEndDateTime != null &&
                    ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)))
                .ToList();

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

                if (period >= ToolBillingUtility.April2011)
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

                var tdr = Session.Get<ToolDataRaw>(rsv.ReservationID).CreateModel<IToolDataRaw>();
                result.Add(tdr);
            }

            return result;
        }
        #endregion

        #region ToolDataClean

        public int UpdateChargeMultiplierByReservationToolDataClean(Reservation rsv)
        {
            return Session.GetNamedQuery("UpdateChargeMultiplierToolDataClean")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("ChargeMultiplier", rsv.ChargeMultiplier)
                .ExecuteUpdate();
        }

        public int UpdateAccountByReservationToolDataClean(Reservation rsv)
        {
            return Session.GetNamedQuery("UpdateAccountToolDataClean")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("AccountID", rsv.Account.AccountID)
                .ExecuteUpdate();
        }
        #endregion

        #region ToolData
        public int UpdateChargeMultiplierByReservationToolData(Reservation rsv)
        {
            return Session.GetNamedQuery("UpdateChargeMultiplierToolData")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("ChargeMultiplier", rsv.ChargeMultiplier)
                .ExecuteUpdate();
        }

        public int UpdateAccountByReservationToolData(Reservation rsv)
        {
            return Session.GetNamedQuery("UpdateAccountToolData")
                .SetParameter("ReservationID", rsv.ReservationID)
                .SetParameter("AccountID", rsv.Account.AccountID)
                .ExecuteUpdate();
        }

        public int MinimumDaysForApportionment(ClientOrg co, Room r, DateTime period)
        {
            var query = Session.Query<ToolData>().Where(x => x.ClientID == co.Client.ClientID && x.Period == period).Where(x => Session.Get<Account>(x.AccountID).Org == co.Org);

            var join = query.Join(Session.Query<Reservation>(), o => o.ReservationID, i => i.ReservationID, (o, i) => new { ToolData = o, Reservation = i });

            var days = join
                .Where(x => x.Reservation.Resource.ProcessTech.Lab.Room == r && x.Reservation.IsStarted)
                .Select(x => x.ToolData.ActDate).ToList();

            var distinctDays = days.Select(x => x.Day).Distinct();

            var result = distinctDays.Count();

            return result;
        }
        #endregion

        private IEnumerable<IToolBilling> CreateToolBillingItems(IEnumerable<IToolBilling> source)
        {
            foreach (var tb in source)
                ToolBillingUtility.CalculateToolBillingCharges(tb);

            return source;
        }

        private IQueryable<IToolBilling> GetToolBillingQuery(bool temp)
        {
            if (temp)
                return Session.Query<ToolBillingTemp>();
            else
                return Session.Query<ToolBilling>();
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

        public IEnumerable<IToolBillingReservation> SelectReservations(DateTime sd, DateTime ed, int resourceId)
        {
            try
            {
                var dt = Session.Command(CommandType.StoredProcedure)
                    .Param("Action", "SelectReservationsForTransferredDuration")
                    .Param("StartDate", sd)
                    .Param("EndDate", ed)
                    .Param("ResourceID", resourceId > 0, resourceId)
                    .FillDataTable("Billing.dbo.ToolDataClean_Select");

                var result = new List<IToolBillingReservation>();

                foreach (DataRow dr in dt.Rows)
                {
                    IToolBillingReservation item = new ToolBillingReservation
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
            catch (Exception ex)
            {
                throw new Exception($"Error in LNF.Impl.Billing.ToolBillingManager.SelectReservations. sd = {sd:yyyy-MM-dd}, ed = {ed:yyyy-MM-dd}, resourceId = {resourceId}", ex);
            }
        }

        private DateTime GetPeriod(SqlConnection conn, int reservationId)
        {
            using (var cmd = conn.CreateCommand("SELECT Period FROM dbo.ToolData WHERE ReservationID = @ReservationID", CommandType.Text))
            {
                cmd.Parameters.AddWithValue("ReservationID", reservationId);
                var obj = cmd.ExecuteScalar();

                if (obj == null)
                    throw new Exception($"Cannot find Reservation with ReservationID = {reservationId}");

                var result = Convert.ToDateTime(obj);

                return result;
            }
        }
    }
}
