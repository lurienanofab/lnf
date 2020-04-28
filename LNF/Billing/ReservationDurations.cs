using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class ReservationDurations : IEnumerable<ReservationDurationItem>
    {
        private Dictionary<int, List<ReservationDurationItem>> _items;
        private List<ReservationDateRangeItem> _anomalies;
        private List<ReservationDateRangeItem> _reservations;
        private ReservationDateRange _range;

        public IEnumerable<ReservationDateRangeItem> Anomalies { get { return _anomalies.AsEnumerable(); } }
        public IEnumerable<ReservationDateRangeItem> Reservations { get { return _reservations.AsEnumerable(); } }

        public ReservationDurations(ReservationDateRange range)
        {
            CreateItems(range);
        }

        public ReservationDurations(IEnumerable<IReservation> reservations, IEnumerable<ICost> costs, DateTime sd, DateTime ed)
        {
            var dateRange = DateRange.ExpandRange(reservations, sd, ed);
            var items = ReservationDateRangeItem.GetReservationDateRangeItems(reservations, costs, dateRange);
            var range = new ReservationDateRange(items, dateRange);
            CreateItems(range);
        }

        private void CreateItems(ReservationDateRange range)
        {
            _range = range;
            _items = new Dictionary<int, List<ReservationDurationItem>>();
            _reservations = new List<ReservationDateRangeItem>();
            _anomalies = new List<ReservationDateRangeItem>();

            if (_range.Count == 0)
                throw new NullReferenceException("range.Count is zero. This should be checked for and handled before this point.");

            foreach (int resourceId in _range.Select(x => x.ResourceID).Distinct())
            {
                _range.Clear();

                /**********
                    * each priority group must be mutually exclusive, i.e. no single reservation should
                      be in two groups, and all reservations must be included across all groups

                    * "started normal" means started on or after the scheduled start time

                    * "ended normal" means ended on or before the scheduled end time

                    * the lowest priority group is written to the array first, then the next highest 
                      priorty group is written over that and then the next until all groups have been written
                ***********/

                var query = _range.Where(x => x.ResourceID == resourceId).ToList();

                // These are normal reservations for which we can calculate ranges.
                List<ReservationDateRangeItem> include = new List<ReservationDateRangeItem>();

                // These are reservations that we can't calculate ranges for, probably because they are still running.
                List<ReservationDateRangeItem> exclude = new List<ReservationDateRangeItem>();

                // Priority 0 (lowest): unstarted, cancelled
                include.AddRange(_range.Apply(0, query.Where(x => !x.IsStarted && !x.IsActive).OrderBy(x => x.LastModifiedOn).ToList()));

                // Priority 1: unstarted, uncancelled
                include.AddRange(_range.Apply(1, query.Where(x => !x.IsStarted && x.IsActive).OrderBy(x => x.LastModifiedOn).ToList()));

                // Priority 2: started normal, ended normal
                include.AddRange(_range.Apply(2, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value >= x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value <= x.EndDateTime).OrderBy(x => x.LastModifiedOn).ToList()));

                // Priority 3: started early, ended normal
                include.AddRange(_range.Apply(3, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value < x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value <= x.EndDateTime).OrderBy(x => x.ActualBeginDateTime).ToList()));

                // Priority 4: started normal, ended late
                include.AddRange(_range.Apply(4, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value >= x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value > x.EndDateTime).OrderByDescending(x => x.ActualBeginDateTime).ToList()));

                // Priority 5: started early, ended late
                include.AddRange(_range.Apply(5, query.Where(x => x.IsStarted && x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value < x.BeginDateTime && x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value > x.EndDateTime).OrderBy(x => x.LastModifiedOn).ToList()));

                // catch reservations that were not selected in one of the above groups (includes reservations that were started but have not ended yet)
                exclude = query.Where(x => x.IsStarted && (!x.ActualBeginDateTime.HasValue || !x.ActualEndDateTime.HasValue)).ToList();

                _reservations.AddRange(include);
                _anomalies.AddRange(exclude);

                int count = include.Count() + exclude.Count();

                int totalCount = query.Count();

                if (count < totalCount)
                    throw new InvalidOperationException(string.Format("ReservationDuration: Reservation count validation failed. Some reservations not selected. [ResourceID = {0}, StartDate = {1:yyyy-MM-dd HH:mm:ss}, EndDate = {2:yyyy-MM-dd HH:mm:ss}, count = {3}, totalCount = {4}]", resourceId, _range.DateRange.StartDate, _range.DateRange.EndDate, count, totalCount));
                else if (count > totalCount)
                    throw new InvalidOperationException(string.Format("ReservationDuration: Reservation count validation falied. Some reservations selected more than once. [ResourceID = {0}, StartDate = {1:yyyy-MM-dd HH:mm:ss}, EndDate = {2:yyyy-MM-dd HH:mm:ss}, count = {3}, totalCount = {4}]", resourceId, _range.DateRange.StartDate, _range.DateRange.EndDate, count, totalCount));

                var items = include.OrderBy(x => x.ChargeBeginDateTime).ThenBy(x => x.ChargeEndDateTime).ThenBy(x => x.ReservationID).Select(x => new ReservationDurationItem(x, _range.GetUtilizedDuration(x.ReservationID))).ToList();
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


        public DurationInfo GetDurationInfo(ReservationDateRangeItem item)
        {
            return _range.GetDurationInfo(item);
        }

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
