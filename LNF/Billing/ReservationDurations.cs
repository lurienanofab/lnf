using LNF.Repository.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class ReservationDurations : IEnumerable<ReservationDurationItem>
    {
        private Dictionary<int, List<ReservationDurationItem>> _items;
        private List<ReservationDateRange.Reservation> _anomalies;
        private List<ReservationDateRange.Reservation> _reservations;

        public IEnumerable<ReservationDateRange.Reservation> Anomalies { get { return _anomalies.AsEnumerable(); } }
        public IEnumerable<ReservationDateRange.Reservation> Reservations { get { return _reservations.AsEnumerable(); } }

        internal ReservationDurations(ReservationDateRange range)
        {
            CreateItems(range);
        }

        private void CreateItems(ReservationDateRange range)
        {
            _items = new Dictionary<int, List<ReservationDurationItem>>();
            _reservations = new List<ReservationDateRange.Reservation>();
            _anomalies = new List<ReservationDateRange.Reservation>();

            if (range.Reservations == null)
                throw new NullReferenceException("Range.Reservations is null. This should be checked for and handled before this point.");

            foreach (int resourceId in range.Reservations.Select(x => x.ResourceID).Distinct())
            {
                range.Clear();

                /**********
                    * each priority group must be mutually exclusive, i.e. no single reservation should
                      be in two groups, and all reservations must be included across all groups

                    * "started normal" means started on or after the scheduled start time

                    * "ended normal" means ended on or before the scheduled end time

                    * the lowest priority group is written to the array first, then the next highest 
                      priorty group is written over that and then the next until all groups have been written
                ***********/

                IEnumerable<ReservationDateRange.Reservation> query = range.Reservations.Where(x => x.ResourceID == resourceId);

                // These are normal reservations for which we can calculate ranges.
                List<ReservationDateRange.Reservation> include = new List<ReservationDateRange.Reservation>();

                // These are reservations that we can't calculate ranges for, probably because they are still running.
                List<ReservationDateRange.Reservation> exclude = new List<ReservationDateRange.Reservation>();

                // Priority 0 (lowest): unstarted, cancelled
                include.AddRange(range.Apply(0, query.Where(x => !x.IsStarted && !x.IsActive).OrderBy(x => x.LastModifiedOn)));

                // Priority 1: unstarted, uncancelled
                include.AddRange(range.Apply(1, query.Where(x => !x.IsStarted && x.IsActive).OrderBy(x => x.LastModifiedOn)));

                // Priority 2: started normal, ended normal
                include.AddRange(range.Apply(2, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value >= x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value <= x.EndDateTime).OrderBy(x => x.LastModifiedOn)));

                // Priority 3: started early, ended normal
                include.AddRange(range.Apply(3, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value < x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value <= x.EndDateTime).OrderBy(x => x.ActualBeginDateTime)));

                // Priority 4: started normal, ended late
                include.AddRange(range.Apply(4, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value >= x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value > x.EndDateTime).OrderByDescending(x => x.ActualBeginDateTime)));

                // Priority 5: started early, ended late
                include.AddRange(range.Apply(5, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value < x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value > x.EndDateTime).OrderBy(x => x.LastModifiedOn)));

                // catch reservations that were not selected in one of the above groups (includes reservations that were started but have not ended yet)
                exclude = query.Where(x => x.IsStarted && (!x.ActualBeginDateTime.HasValue || !x.ActualEndDateTime.HasValue)).ToList();

                _reservations.AddRange(include);
                _anomalies.AddRange(exclude);

                int count = include.Count() + exclude.Count();

                int totalCount = query.Count();

                if (count < totalCount)
                    throw new InvalidOperationException(string.Format("ReservationDuration: Reservation count validation failed. Some reservations not selected. [ResourceID = {0}, StartDate = {1:yyyy-MM-dd HH:mm:ss}, EndDate = {2:yyyy-MM-dd HH:mm:ss}, count = {3}, totalCount = {4}]", resourceId, range.Range.StartDate, range.Range.EndDate, count, totalCount));
                else if (count > totalCount)
                    throw new InvalidOperationException(string.Format("ReservationDuration: Reservation count validation falied. Some reservations selected more than once. [ResourceID = {0}, StartDate = {1:yyyy-MM-dd HH:mm:ss}, EndDate = {2:yyyy-MM-dd HH:mm:ss}, count = {3}, totalCount = {4}]", resourceId, range.Range.StartDate, range.Range.EndDate, count, totalCount));

                var items = include.OrderBy(x => x.ChargeBeginDateTime).ThenBy(x => x.ChargeEndDateTime).ThenBy(x => x.ReservationID).Select(x => new ReservationDurationItem(x, range.GetUtilizedDuration(x.ReservationID))).ToList();
                _items.Add(resourceId, items);
            }
        }

        public IEnumerable<ReservationDurationItem> this[int resourceId]
        {
            get
            {
                if (_items.ContainsKey(resourceId))
                    return _items[resourceId];
                else
                    return new ReservationDurationItem[] { };
            }
        }

        public int Count { get { return _items.Count(); } }

        public IEnumerator<ReservationDurationItem> GetEnumerator()
        {
            return _items.SelectMany(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
