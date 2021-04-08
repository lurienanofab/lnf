using LNF.CommonTools;
using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class ReservationDateRange : IEnumerable<ReservationDateRangeItem>
    {
        private int[] _buffer;
        private Dictionary<int, BufferSegment> _lookup;
        private readonly List<ReservationDateRangeItem> _reservations = new List<ReservationDateRangeItem>();

        public DateRange DateRange { get; }

        //public ReservationDateRange(DateTime period) : this(0, period, period.AddMonths(1)) { }

        //public ReservationDateRange(DateTime sd, DateTime ed) : this(0, sd, ed) { }

        //public ReservationDateRange(int resourceId, DateTime sd, DateTime ed) : this(resourceId, new DateRange(sd, ed)) { }

        //public ReservationDateRange(DateRange range) : this(0, range) { }

        //public ReservationDateRange(int resourceId, DateRange range) : this(GetReservations(resourceId, range), range) { }

        public ReservationDateRange(IEnumerable<ReservationDateRangeItem> reservations) : this(reservations, DateRange.GetDateRange(reservations)) { }

        //public ReservationDateRange(IEnumerable<ReservationDateRangeItem> reservations, DateTime period) : this(reservations, GetDateRange(period)) { }

        public ReservationDateRange(IEnumerable<ReservationDateRangeItem> reservations, DateRange range)
        {
            DateRange = range;

            int size = Convert.ToInt32(DateRange.Span.TotalSeconds);
            _buffer = new int[size];

            _reservations = reservations.ToList();
        }

        public int Count => _reservations.Count;

        public void Clear()
        {
            Array.Clear(_buffer, 0, _buffer.Length);

            // the key is a ReservationID
            _lookup = new Dictionary<int, BufferSegment>();
        }

        public IEnumerable<ReservationDateRangeItem> Apply(int priorityGroup, IEnumerable<ReservationDateRangeItem> reservations)
        {
            List<ReservationDateRangeItem> result = new List<ReservationDateRangeItem>();

            foreach (var rsv in reservations)
            {
                DateTime sd = rsv.ChargeBeginDateTime;
                DateTime ed = rsv.ChargeEndDateTime;

                int a = Math.Max(GetIndex(sd), 0);
                int b = Math.Min(GetIndex(ed), _buffer.Length);

                _lookup.Add(rsv.ReservationID, new BufferSegment(a, b));

                for (int x = a; x < b; x++)
                    _buffer[x] = rsv.ReservationID;

                rsv.PriorityGroup = priorityGroup;

                result.Add(rsv);
            }

            return result;
        }

        private int GetIndex(DateTime d)
        {
            TimeSpan span = d - DateRange.StartDate;
            return Convert.ToInt32(span.TotalSeconds);
        }

        public TimeSpan GetUtilizedDuration(int reservationId)
        {
            // To speed up searching the array we have a lookup dictionary to get the min and max index for a reservation.
            // This way we only have to search small parts of the array to get the total duration, not the whole thing.
            // The dictionary is built when we write to the buffer (in Apply)

            if (!_lookup.ContainsKey(reservationId))
                return TimeSpan.Zero;

            var bufferSegment = _lookup[reservationId];

            int totalSeconds = 0;

            for (int x = bufferSegment.Start; x < bufferSegment.End; x++)
            {
                if (_buffer[x] == reservationId)
                    totalSeconds += 1;
            }

            TimeSpan result = TimeSpan.FromSeconds(totalSeconds);

            return result;
        }

        public DurationInfo GetDurationInfo(ReservationDateRangeItem rsv)
        {
            return DurationInfo.Create(rsv, GetDurationParts(rsv.ReservationID, rsv.ChargeBeginDateTime, rsv.ChargeEndDateTime, rsv.EndDateTime));
        }

        private IEnumerable<DurationPart> GetDurationParts(int reservationId, DateTime chargeBegin, DateTime chargeEnd, DateTime scheduledEnd)
        {
            var result = new List<DurationPart>();

            DurationPart prev = null;

            int start = Math.Max(GetIndex(chargeBegin), 0);
            int end = Math.Min(GetIndex(chargeEnd), _buffer.Length);

            for (int x = start; x < end; x++)
            {
                var d = DateRange.StartDate.AddSeconds(x);
                var id = _buffer[x];

                string current;

                if (id != reservationId)
                    current = "T";
                else if (d >= scheduledEnd)
                    current = "O";
                else
                    current = "S";

                if (prev == null || prev.DurationType != current)
                {
                    prev = new DurationPart() { DurationType = current, Duration = TimeSpan.Zero };
                    result.Add(prev);
                }

                prev.Duration = prev.Duration.Add(TimeSpan.FromSeconds(1));
            }

            return result;
        }

        private static IEnumerable<ReservationDateRangeItem> GetReservations(int resourceId, DateRange range)
        {
            var costs = ServiceProvider.Current.Data.Cost.FindToolCosts(resourceId, range.EndDate);

            var reservations = ServiceProvider.Current.Scheduler.Reservation.GetReservations(range.StartDate, range.EndDate, resourceId: resourceId);

            var result = reservations.ToList().Select(x => CreateReservation(x, costs)).ToList();

            return result;
        }

        private static ReservationDateRangeItem CreateReservation(IReservationItem rsv, IEnumerable<ICost> costs)
        {
            var filtered = costs.Where(c => (c.RecordID == rsv.ResourceID || c.RecordID == 0) && c.ChargeTypeID == rsv.ChargeTypeID).ToList();
            IResourceCost rc = ResourceCost.CreateResourceCosts(filtered).FirstOrDefault();

            if (rc == null)
                throw new Exception($"Cannot determine cost for Resource: [{rsv.ResourceID}] {rsv.ResourceName}");

            return new ReservationDateRangeItem()
            {
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.ResourceID,
                ClientID = rsv.ClientID,
                ActivityID = rsv.ActivityID,
                AccountID = rsv.AccountID,
                ChargeTypeID = rsv.ChargeTypeID,
                IsActive = rsv.IsActive,
                IsStarted = rsv.IsStarted,
                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ActualBeginDateTime = rsv.ActualBeginDateTime,
                ActualEndDateTime = rsv.ActualEndDateTime,
                ChargeBeginDateTime = rsv.ChargeBeginDateTime,
                ChargeEndDateTime = rsv.ChargeEndDateTime,
                LastModifiedOn = rsv.LastModifiedOn,
                IsCancelledBeforeCutoff = rsv.IsCancelledBeforeCutoff,
                ChargeMultiplier = rsv.ChargeMultiplier,
                Cost = rc
            };
        }

        public IEnumerator<ReservationDateRangeItem> GetEnumerator()
        {
            return _reservations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ReservationDateRangeItem
    {
        public int PriorityGroup { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public int ClientID { get; set; }
        public int ActivityID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int ChargeTypeID { get; set; }
        public bool IsStarted { get; set; }
        public bool IsActive { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime ChargeBeginDateTime { get; set; }
        public DateTime ChargeEndDateTime { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public bool IsCancelledBeforeCutoff { get; set; }
        public double ChargeMultiplier { get; set; }
        public IResourceCost Cost { get; set; }
        public DateTime ActDate => ActualBeginDateTime.GetValueOrDefault(BeginDateTime).Date;

        public override string ToString()
        {
            return $"[{ReservationID}:{ResourceID}] {ChargeBeginDateTime:yyyy-MM-dd HH:mm:ss} - {ChargeEndDateTime:yyyy-MM-dd HH:mm:ss}";
        }

        public static IEnumerable<ReservationDateRangeItem> GetReservationDateRangeItems(IEnumerable<IToolBillingReservation> reservations, IEnumerable<ICost> costs, DateRange range)
        {
            var result = reservations.Select(r => Create(
                r.ReservationID,
                r.ResourceID,
                r.ClientID,
                r.ActivityID,
                r.AccountID,
                r.AccountName,
                r.ShortCode,
                r.ChargeTypeID,
                r.IsActive,
                r.IsStarted,
                r.BeginDateTime,
                r.EndDateTime,
                r.ActualBeginDateTime,
                r.ActualEndDateTime,
                r.LastModifiedOn,
                r.CancelledDateTime,
                r.ChargeMultiplier,
                costs)).ToList();

            return result;
        }

        public static IEnumerable<ReservationDateRangeItem> GetReservationDateRangeItems(IEnumerable<IReservationItem> reservations, IEnumerable<ICost> costs, DateRange range)
        {
            //var costs = ServiceProvider.Current.Data.Cost.FindToolCosts(ResourceID, range.EndDate);
            //var reservations = ServiceProvider.Current.Billing.Tool.SelectReservations(range.StartDate, range.EndDate, ResourceID);

            var result = reservations.Select(r => Create(
                r.ReservationID,
                r.ResourceID,
                r.ClientID,
                r.ActivityID,
                r.AccountID,
                r.AccountName,
                r.ShortCode,
                r.ChargeTypeID,
                r.IsActive,
                r.IsStarted,
                r.BeginDateTime,
                r.EndDateTime,
                r.ActualBeginDateTime,
                r.ActualEndDateTime,
                r.LastModifiedOn,
                r.CancelledDateTime,
                r.ChargeMultiplier,
                costs)).ToList();

            return result;
        }

        public static ReservationDateRangeItem Create(int reservationId, int resourceId, int clientId, int activityId, int accountId, string accountName, string shortCode, int chargeTypeId, bool isActive, bool isStarted, DateTime beginDateTime, DateTime endDateTime, DateTime? actualBeginDateTime, DateTime? actualEndDateTime, DateTime lastModifiedOn, DateTime? cancelledDateTime, double chargeMultiplier, IEnumerable<ICost> costs)
        {
            // we need to truncate dates to remove milliseconds or else we end up with very small
            // transfer durations (the difference between charged and utilized durations)

            var truncatedCancelledDateTime = Utility.Truncate(cancelledDateTime);
            var truncatedBeginDateTime = Utility.Truncate(beginDateTime);
            var truncatedEndDateTime = Utility.Truncate(endDateTime);
            var truncatedActualBeginDateTime = Utility.Truncate(actualBeginDateTime);
            var truncatedActualEndDateTime = Utility.Truncate(actualEndDateTime);

            bool isCancelledBeforeCutoff = Reservations.GetIsCancelledBeforeCutoff(truncatedCancelledDateTime, truncatedBeginDateTime);
            IResourceCost cost = ResourceCost.CreateResourceCosts(costs.Where(c => (c.RecordID == resourceId || c.RecordID == 0) && c.ChargeTypeID == chargeTypeId)).FirstOrDefault();

            var chargeBeginDateTime = (truncatedActualBeginDateTime.HasValue && truncatedActualBeginDateTime < truncatedBeginDateTime) ? truncatedActualBeginDateTime.Value : truncatedBeginDateTime;
            var chargeEndDateTime = (truncatedActualEndDateTime.HasValue && truncatedActualEndDateTime > truncatedEndDateTime) ? truncatedActualEndDateTime.Value : truncatedEndDateTime;

            var item = new ReservationDateRangeItem
            {
                ReservationID = reservationId,
                ResourceID = resourceId,
                ClientID = clientId,
                ActivityID = activityId,
                AccountID = accountId,
                AccountName =accountName,
                ShortCode = shortCode,
                ChargeTypeID = chargeTypeId,
                IsActive = isActive,
                IsStarted = isStarted,
                BeginDateTime = truncatedBeginDateTime,
                EndDateTime = truncatedEndDateTime,
                ActualBeginDateTime = truncatedActualBeginDateTime,
                ActualEndDateTime = truncatedActualEndDateTime,
                ChargeBeginDateTime = chargeBeginDateTime,
                ChargeEndDateTime = chargeEndDateTime,
                LastModifiedOn = lastModifiedOn,
                IsCancelledBeforeCutoff = isCancelledBeforeCutoff,
                ChargeMultiplier = chargeMultiplier,
                Cost = cost
            };

            return item;
        }
    }

    public struct DurationInfo
    {
        public static DurationInfo Create(ReservationDateRangeItem rsv, IEnumerable<DurationPart> parts)
        {
            int repairActivityId = 14;

            return new DurationInfo()
            {
                PriorityGroup = rsv.PriorityGroup,
                ReservationID = rsv.ReservationID,
                //DisplayName = rsv.DisplayName,
                IsStarted = rsv.IsStarted,
                IsActive = rsv.IsActive,
                IsRepair = rsv.ActivityID == repairActivityId,
                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ActualBeginDateTime = rsv.ActualBeginDateTime,
                ActualEndDateTime = rsv.ActualEndDateTime,
                ChargeBeginDateTime = rsv.ChargeBeginDateTime,
                ChargeEndDateTime = rsv.ChargeEndDateTime,
                ChargeMultiplier = rsv.ChargeMultiplier,
                Parts = parts
            };
        }

        public int PriorityGroup { get; private set; }
        public int ReservationID { get; private set; }
        //public string DisplayName { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsRepair { get; private set; }
        public DateTime BeginDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
        public DateTime? ActualBeginDateTime { get; private set; }
        public DateTime? ActualEndDateTime { get; private set; }
        public DateTime ChargeBeginDateTime { get; private set; }
        public DateTime ChargeEndDateTime { get; private set; }
        public double ChargeMultiplier { get; private set; }
        public IEnumerable<DurationPart> Parts { get; private set; }
    }

    public struct BufferSegment
    {
        public BufferSegment(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; }
        public int End { get; }
    }

    public class DurationPart
    {
        public string DurationType { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public struct DateRange
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public TimeSpan Span { get { return EndDate - StartDate; } }

        public DateRange(DateTime period) : this(period, period.AddMonths(1)) { }

        public DateRange(DateTime sd, DateTime ed)
        {

            StartDate = sd;
            EndDate = ed;
        }

        public static bool operator ==(DateRange dr1, DateRange dr2)
        {
            return dr1.StartDate == dr2.StartDate && dr1.EndDate == dr2.EndDate;
        }

        public static bool operator !=(DateRange dr1, DateRange dr2)
        {
            return !(dr1.StartDate == dr2.StartDate && dr1.EndDate == dr2.EndDate);
        }

        public override bool Equals(object obj)
        {
            var dr = (DateRange)obj;
            return dr == this;
        }

        public override int GetHashCode()
        {
            return new { StartDate, EndDate }.GetHashCode();
        }

        /// <summary>
        /// Gets the minimum start date and maximum end date of all overlapping reservations. Set parameter resourceId to zero to ignore.
        /// </summary>
        public static DateRange ExpandRange(IEnumerable<IReservation> reservations, DateTime sd, DateTime ed)
        {
            int count = 0;
            int max = 100;

            while (count < max)
            {
                var minBeginDateTime = reservations.Min(x => (DateTime?)x.ChargeBeginDateTime);
                var maxEndDateTime = reservations.Max(x => (DateTime?)x.ChargeEndDateTime);

                bool ok = true;

                if (minBeginDateTime.HasValue && minBeginDateTime < sd)
                {
                    sd = minBeginDateTime.Value;
                    ok = false;
                }

                if (maxEndDateTime.HasValue && maxEndDateTime > ed)
                {
                    ed = maxEndDateTime.Value;
                    ok = false;
                }

                if (ok) break;

                count++;
            }

            if (count == max)
                throw new Exception("Cannot determine expanded start and end dates for reservation date range.");

            var expanded = new DateRange(sd, ed);

            return expanded;
        }

        public static DateRange GetDateRange(IEnumerable<ReservationDateRangeItem> reservations)
        {
            var min = reservations.Min(x => x.ChargeBeginDateTime).Date;
            var max = reservations.Max(x => x.ChargeEndDateTime).Date.AddDays(1);
            return new DateRange(min, max);
        }

        public static DateRange GetDateRange(DateTime period) => new DateRange(period, period.AddMonths(1));
    }
}
