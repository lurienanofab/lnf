using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class ReservationDateRange
    {
        private int[] _buffer;
        private Dictionary<int, Tuple<int, int>> _lookup;

        //public DateTime StartDate { get; private set; }
        //public DateTime EndDate { get; private set; }
        public DateRange Range { get; }
        public IEnumerable<Reservation> Reservations { get; }

        public ReservationDateRange(DateTime period) : this(0, period, period.AddMonths(1)) { }

        public ReservationDateRange(DateTime sd, DateTime ed) : this(0, sd, ed) { }

        public ReservationDateRange(int resourceId, DateTime sd, DateTime ed) : this(resourceId, new DateRange(sd, ed)) { }

        public ReservationDateRange(DateRange range) : this(0, range) { }

        public ReservationDateRange(int resourceId, DateRange range) : this(GetReservations(resourceId, range), range) { }

        public ReservationDateRange(IEnumerable<Reservation> reservations, DateRange range)
        {
            Range = range;

            int size = Convert.ToInt32(Range.Span.TotalSeconds);
            _buffer = new int[size];

            Reservations = reservations;
        }

        public void Clear()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            _lookup = new Dictionary<int, Tuple<int, int>>();
        }

        public IEnumerable<Reservation> Apply(int priorityGroup, IEnumerable<Reservation> reservations)
        {
            List<Reservation> result = new List<Reservation>();

            foreach (var rsv in reservations)
            {
                DateTime sd = rsv.ChargeBeginDateTime;
                DateTime ed = rsv.ChargeEndDateTime;

                int a = Math.Max(GetIndex(sd), 0);
                int b = Math.Min(GetIndex(ed), _buffer.Length);

                _lookup.Add(rsv.ReservationID, new Tuple<int, int>(a, b));

                for (int x = a; x < b; x++)
                    _buffer[x] = rsv.ReservationID;

                rsv.PriorityGroup = priorityGroup;

                result.Add(rsv);
            }

            return result;
        }

        private int GetIndex(DateTime d)
        {
            TimeSpan span = d - Range.StartDate;
            return Convert.ToInt32(span.TotalSeconds);
        }

        public ReservationDurations CreateReservationDurations()
        {
            return new ReservationDurations(this);
        }

        public TimeSpan GetUtilizedDuration(int reservationId)
        {
            // To speed up searching the array we have a lookup dictionary to get the min and max index for a reservation.
            // This way we only have to search small parts of the array to get the total duration, not the whole thing.
            // The dictionary is built when we write to the buffer (in Apply)

            if (!_lookup.ContainsKey(reservationId))
                return TimeSpan.Zero;

            var tuple = _lookup[reservationId];

            int totalSeconds = 0;

            for (int x = tuple.Item1; x < tuple.Item2; x++)
            {
                if (_buffer[x] == reservationId)
                    totalSeconds += 1;
            }

            TimeSpan result = TimeSpan.FromSeconds(totalSeconds);

            return result;
        }

        public DurationInfo GetDurationInfo(Reservation rsv)
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
                var d = Range.StartDate.AddSeconds(x);
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

        /// <summary>
        /// Gets the minimum start date and maximum end date of all overlapping reservations. Set parameter resourceId to zero to ignore.
        /// </summary>
        public static DateRange ExpandRange(int resourceId, DateTime sd, DateTime ed)
        {
            IQueryable<ReservationInfo> query = null;

            int count = 0;
            int max = 100;

            while (count < max)
            {
                if (resourceId == 0)
                    query = DA.Current.Query<ReservationInfo>().Where(x => x.ChargeBeginDateTime < ed && x.ChargeEndDateTime > sd);
                else
                    query = DA.Current.Query<ReservationInfo>().Where(x => x.ResourceID == resourceId && x.ChargeBeginDateTime < ed && x.ChargeEndDateTime > sd);

                var minBeginDateTime = query.Min(x => (DateTime?)x.ChargeBeginDateTime);
                var maxEndDateTime = query.Max(x => (DateTime?)x.ChargeEndDateTime);

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

        private static IEnumerable<Reservation> GetReservations(int resourceId, DateRange range)
        {
            var costs = ServiceProvider.Current.Data.GetResourceCosts(resourceId, range.EndDate);

            IQueryable<ReservationInfo> query = null;

            if (resourceId == 0)
                query = DA.Current.Query<ReservationInfo>().Where(x => x.ChargeBeginDateTime < range.EndDate && x.ChargeEndDateTime > range.StartDate);
            else
                query = DA.Current.Query<ReservationInfo>().Where(x => x.ResourceID == resourceId && x.ChargeBeginDateTime < range.EndDate && x.ChargeEndDateTime > range.StartDate);

            var result = query.ToList().Select(x => new Reservation()
            {
                ReservationID = x.ReservationID,
                ResourceID = x.ResourceID,
                ResourceName = x.ResourceName,
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                ClientID = x.ClientID,
                UserName = x.UserName,
                DisplayName = ClientItem.GetDisplayName(x.LName, x.FName),
                ActivityID = x.ActivityID,
                ActivityName = x.ActivityName,
                AccountID = x.AccountID,
                AccountName = x.AccountName,
                ShortCode = x.ShortCode,
                IsActive = x.IsActive,
                IsStarted = x.IsStarted,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ActualBeginDateTime = x.ActualBeginDateTime,
                ActualEndDateTime = x.ActualEndDateTime,
                ChargeBeginDateTime = x.ChargeBeginDateTime,
                ChargeEndDateTime = x.ChargeEndDateTime,
                LastModifiedOn = x.LastModifiedOn,
                IsCancelledBeforeCutoff = x.IsCancelledBeforeCutoff(),
                ChargeMultiplier = x.ChargeMultiplier,
                Cost = ResourceCost.CreateResourceCosts(costs.Where(c => (c.RecordID == x.ResourceID || c.RecordID == 0) && c.ChargeTypeID == x.ChargeTypeID)).FirstOrDefault()
            }).ToList();

            return result;
        }

        public class Reservation
        {
            public int PriorityGroup { get; set; }
            public int ReservationID { get; set; }
            public int ResourceID { get; set; }
            public string ResourceName { get; set; }
            public int ProcessTechID { get; set; }
            public string ProcessTechName { get; set; }
            public int ClientID { get; set; }
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public int ActivityID { get; set; }
            public string ActivityName { get; set; }
            public int AccountID { get; set; }
            public string AccountName { get; set; }
            public string ShortCode { get; set; }
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
            public ResourceCost Cost { get; set; }
            public DateTime ActDate => ActualBeginDateTime.GetValueOrDefault(BeginDateTime).Date;
        }

        public struct DurationInfo
        {
            public static DurationInfo Create(Reservation rsv, IEnumerable<DurationPart> parts)
            {
                int repairActivityId = 14;

                return new DurationInfo()
                {
                    PriorityGroup = rsv.PriorityGroup,
                    ReservationID = rsv.ReservationID,
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
        }
    }
}
