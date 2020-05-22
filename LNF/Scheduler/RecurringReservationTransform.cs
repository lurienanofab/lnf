using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public enum PatternType
    {
        Weekly = 1,
        Monthly = 2
    }

    public static class RecurringReservationTransform
    {
        // Fix this dependency
        public static IProvider Provider => ServiceProvider.Current;

        /// <summary>
        /// Returns true if a new reservation is created.
        /// </summary>
        public static bool AddRegularFromRecurring(ReservationCollection reservations, IReservationRecurrence rr, DateTime d)
        {
            // reservations should contain canceled reservations

            // first, find out the pattern type
            if (rr.PatternID == (int)PatternType.Weekly)
            {
                if (rr.PatternParam1 == (int)d.DayOfWeek)
                {
                    if ((d >= rr.BeginDate && rr.EndDate == null) || (d >= rr.BeginDate && d <= rr.EndDate.Value))
                        return AddNewRecurringReservation(reservations, GetReservationData(rr, d));
                }
            }
            else if (rr.PatternID == (int)PatternType.Monthly)
            {
                if ((d >= rr.BeginDate && rr.EndDate == null) || (d >= rr.BeginDate && d <= rr.EndDate))
                {
                    DateTime dayOfMonth = GetDate(new DateTime(d.Year, d.Month, 1), rr.PatternParam1, (DayOfWeek)rr.PatternParam2);
                    if (d.Date == dayOfMonth)
                        return AddNewRecurringReservation(reservations, GetReservationData(rr, d));
                }
            }

            // currently only supports two types

            return false;
        }

        private static ReservationData GetReservationData(IReservationRecurrence rr, DateTime d)
        {
            var processInfos = GetProcessInfos(rr.RecurrenceID);
            var invitees = GetInvitees(rr.RecurrenceID);

            var beginDateTime = new DateTime(d.Year, d.Month, d.Day, rr.BeginTime.Hour, rr.BeginTime.Minute, rr.BeginTime.Second);
            var duration = TimeSpan.FromMinutes(rr.Duration);

            return new ReservationData(processInfos, invitees)
            {
                AccountID = rr.AccountID,
                ResourceID = rr.ResourceID,
                ClientID = rr.ClientID,
                ActivityID = rr.ActivityID,
                RecurrenceID = rr.RecurrenceID,
                Duration = new ReservationDuration(beginDateTime, duration),
                AutoEnd = rr.AutoEnd,
                KeepAlive = rr.KeepAlive,
                Notes = rr.Notes
            };
        }

        private static bool AddNewRecurringReservation(ReservationCollection reservations, ReservationData data)
        {
            // [2013-05-16 jg] We need find any existing, unended and uncancelled reservations in the
            // same time slot. This can happen if there are overlapping recurring reservation patterns.
            // To determine if two reservations overlap I'm using this logic: (StartA < EndB) and(EndA > StartB)
            // [2019-06-10 jg] Can also overlap if the RecurrenceID and time slots are the same,
            // regardless of IsActive. This will prevent previously cancelled recurrences from being
            // created again.

            // Step 1. Get any overlapping by resource and date range.
            var overlapping = reservations.Where(x => x.ResourceID == data.ResourceID
                && (x.BeginDateTime < data.Duration.EndDateTime && x.EndDateTime > data.Duration.BeginDateTime)).ToList();

            // Step 2. Check for any active (uncancelled) or a reservation with the same RecurrenceID (cancelled or uncancelled).
            //      The idea here is a recurrence should be created if the time slot is unused or there is a cancelled reservation unless
            //      the cancelled reservation is the same recurrence, in which case we do not want to create the recurrence again.
            var isOverlapped = overlapping.Any(x => x.IsActive || x.RecurrenceID == data.RecurrenceID);

            //var overlapping = reservations.Any(x =>
            //    (x.IsActive || x.RecurrenceID == data.RecurrenceID)
            //    && x.ResourceID == data.ResourceID
            //    && (x.BeginDateTime < data.Duration.EndDateTime && x.EndDateTime > data.Duration.BeginDateTime)
            //    && x.ActualEndDateTime == null); <-- this was causing a problem when an existing cancelled recurrence was present because in this case ActualEndDateTime was not null so the recurrence was created again.

            if (!isOverlapped)
            {
                var util = Reservations.Create(Provider, DateTime.Now);
                var rsv = util.CreateReservation(data);
                reservations.Add(rsv);
                return true;
            }

            return false;
        }

        public static IEnumerable<IReservationInvitee> GetInvitees(int recurrenceId)
        {
            // get any invitees for the most recent recurrence reservation
            var prev = GetPreviousRecurrence(recurrenceId);

            IList<IReservationInvitee> result;

            if (prev != null)
                result = Provider.Scheduler.Reservation.GetInvitees(prev.ReservationID).ToList();
            else
                result = new List<IReservationInvitee>();

            return result;
        }

        public static IEnumerable<IReservationProcessInfo> GetProcessInfos(int recurrenceId)
        {
            // get any process infos for the most recent recurrence reservation
            var prev = GetPreviousRecurrence(recurrenceId);

            IList<IReservationProcessInfo> result;

            if (prev != null)
                result = Provider.Scheduler.ProcessInfo.GetReservationProcessInfos(prev.ReservationID).ToList();
            else
                result = new List<IReservationProcessInfo>();

            return result;
        }

        public static void CopyProcessInfo(int reservationId, IEnumerable<IReservationProcessInfo> processInfos)
        {
            foreach (var item in processInfos)
            {
                Provider.Scheduler.ProcessInfo
                    .AddReservationProcessInfo(reservationId, item.ProcessInfoLineID, item.Value, item.Special, item.RunNumber, item.ChargeMultiplier, item.Active);
            }
        }

        public static void CopyInvitees(int reservationId, IEnumerable<IReservationInvitee> invitees)
        {
            foreach (var item in invitees)
            {
                Provider.Scheduler.Reservation.AddInvitee(reservationId, item.InviteeID);
            }
        }

        public static IReservation GetPreviousRecurrence(int recurrenceId, int notReservationId = 0)
        {
            return Provider.Scheduler.Reservation.GetPreviousRecurrence(recurrenceId, notReservationId);
        }

        public static DateTime GetDate(DateTime period, int n, DayOfWeek dow)
        {
            var edate = period.AddMonths(1);
            var days = (int)(edate - period).TotalDays;
            var result = Enumerable.Range(0, days).Select(x => period.AddDays(x)).Where(d => d.DayOfWeek == dow).ToList();

            if (result.Count >= n)
                return result[n - 1];
            else
                return result.Last();
        }
    }
}
