using LNF.Repository;
using LNF.Repository.Data;
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
        private IEnumerable<Reservation> _Reservations;

        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public ReservationDateRange(int resourceId, DateTime sd, DateTime ed) : this(GetReservations(resourceId, sd, ed), sd, ed) { }

        public ReservationDateRange(DateTime sd, DateTime ed) : this(GetReservations(0, sd, ed), sd, ed) { }

        public ReservationDateRange(IEnumerable<Reservation> reservations, DateTime sd, DateTime ed)
        {
            StartDate = sd;
            EndDate = ed;
            int size = Convert.ToInt32((EndDate - StartDate).TotalSeconds);
            _buffer = new int[size];
            _Reservations = reservations;
        }

        public IEnumerable<Reservation> Reservations
        {
            get { return _Reservations; }
        }

        public void Clear()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            _lookup = new Dictionary<int, Tuple<int, int>>();
        }

        public IEnumerable<Reservation> Apply(IEnumerable<Reservation> reservations)
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

                result.Add(rsv);
            }

            return result;
        }

        private int GetIndex(DateTime d)
        {
            TimeSpan span = d - StartDate;
            return Convert.ToInt32(span.TotalSeconds);
        }

        public TimeSpan GetDuration(int reservationId)
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

        public static IEnumerable<Reservation> GetReservations(int resourceId, DateTime sd, DateTime ed)
        {
            IQueryable<Repository.Scheduler.Reservation> query;

            if (resourceId == 0)
                query = DA.Scheduler.Reservation.Query().Where(x =>
                    (x.BeginDateTime < ed && x.EndDateTime > sd)
                    || (x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value < ed && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value > sd)
                );
            else
            {
                query = DA.Scheduler.Reservation.Query().Where(x =>
                    x.Resource.ResourceID == resourceId
                    && ((x.BeginDateTime < ed && x.EndDateTime > sd)
                    || (x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value < ed && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value > sd))
                );
            }

            var result = query.Select(x => new Reservation()
            {
                ReservationID = x.ReservationID,
                ResourceID = x.Resource.ResourceID,
                ResourceName = x.Resource.ResourceName,
                ProcessTechID = x.Resource.ProcessTech.ProcessTechID,
                ProcessTechName = x.Resource.ProcessTech.ProcessTechName,
                ClientID = x.Client.ClientID,
                UserName = x.Client.UserName,
                DisplayName = Client.GetDisplayName(x.Client.LName, x.Client.FName),
                ActivityID = x.Activity.ActivityID,
                ActivityName = x.Activity.ActivityName,
                AccountID = x.Account.AccountID,
                AccountName = x.Account.Name,
                ShortCode = x.Account.ShortCode,
                IsActive = x.IsActive,
                IsStarted = x.IsStarted,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ActualBeginDateTime = x.ActualBeginDateTime,
                ActualEndDateTime = x.ActualEndDateTime,
                ChargeBeginDateTime = x.ChargeBeginDateTime(),
                ChargeEndDateTime = x.ChargeEndDateTime(),
                LastModifiedOn = x.LastModifiedOn,
                IsCancelledBeforeAllowedTime = x.IsCancelledBeforeAllowedTime(),
                ChargeMultiplier = x.ChargeMultiplier,
                Cost = x.GetResourceCost()
            }).ToList();

            return result;
        }

        public class Reservation
        {
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
            public bool IsCancelledBeforeAllowedTime { get; set; }
            public double ChargeMultiplier { get; set; }
            public ResourceCost Cost { get; set; }
        }
    }
}
