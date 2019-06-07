using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
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
        /// <summary>
        /// Returns true if a new reservation is created.
        /// </summary>
        public static bool AddRegularFromRecurring(IList<IReservation> reservations, IReservationRecurrence rr, DateTime startTime)
        {
            // first, find out the pattern type
            if (rr.PatternID == (int)PatternType.Weekly)
            {
                if (rr.PatternParam1 == (int)startTime.DayOfWeek)
                {
                    if ((startTime >= rr.BeginDate && rr.EndDate == null) || (startTime >= rr.BeginDate && startTime <= rr.EndDate.Value))
                        return AddNewRecurringReservation(GetReservationData(rr, startTime), reservations);
                }
            }
            else if (rr.PatternID == (int)PatternType.Monthly)
            {
                if ((startTime >= rr.BeginDate && rr.EndDate == null) || (startTime >= rr.BeginDate && startTime <= rr.EndDate))
                {
                    DateTime d = GetDate(new DateTime(startTime.Year, startTime.Month, 1), rr.PatternParam1, (DayOfWeek)rr.PatternParam2);
                    if (startTime.Date == d)
                        return AddNewRecurringReservation(GetReservationData(rr, startTime), reservations);
                }
            }

            // currently only supports two types

            return false;
        }

        private static ReservationData GetReservationData(IReservationRecurrence rr, DateTime startTime)
        {
            var processInfos = GetProcessInfos(rr.RecurrenceID);
            var invitees = GetInvitees(rr.RecurrenceID);

            return new ReservationData(processInfos, invitees)
            {
                AccountID = rr.AccountID,
                ActivityID = rr.ActivityID,
                AutoEnd = rr.AutoEnd,
                ClientID = rr.ClientID,
                Duration = new ReservationDuration(startTime, TimeSpan.FromMinutes(rr.Duration)),
                KeepAlive = rr.KeepAlive,
                Notes = rr.Notes,
                RecurrenceID = rr.RecurrenceID,
                ResourceID = rr.ResourceID
            };
        }

        private static bool AddNewRecurringReservation(ReservationData data, IList<IReservation> reservations)
        {
            // [2013-05-16 jg] We need find any existing, unended and uncancelled reservations in the
            // same time slot. This can happen if there are overlapping recurring reservation patterns.
            // To determine if two reservations overlap I'm using this logic: (StartA < EndB) and(EndA > StartB)
            var overlapping = reservations.Any(x =>
                x.IsActive
                && x.ResourceID == data.ResourceID
                && (x.BeginDateTime < data.Duration.EndDateTime && x.EndDateTime > data.Duration.BeginDateTime)
                && x.ActualEndDateTime == null);

            if (!overlapping)
            {
                if (!reservations.Any(x => x.RecurrenceID == data.RecurrenceID))
                {
                    var util = new ReservationUtility(DateTime.Now, ServiceProvider.Current);
                    var rsv = util.Create(data);
                    reservations.Add(rsv);
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<IReservationInvitee> GetInvitees(int recurrenceId)
        {
            // get any invitees for the most recent recurrence reservation
            var prev = GetPreviousRecurrence(recurrenceId);

            IList<IReservationInvitee> result;

            if (prev != null)
                result = ServiceProvider.Current.Scheduler.Reservation.GetReservationInvitees(prev.ReservationID).ToList();
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
                result = ServiceProvider.Current.Scheduler.ProcessInfo.GetReservationProcessInfos(prev.ReservationID).ToList();
            else
                result = new List<IReservationProcessInfo>();

            return result;
        }

        public static void CopyProcessInfo(int reservationId, IEnumerable<IReservationProcessInfo> processInfos)
        {
            foreach (var item in processInfos)
            {
                var rpi = new ReservationProcessInfo()
                {
                    Active = item.Active,
                    ChargeMultiplier = item.ChargeMultiplier,
                    ProcessInfoLine = DA.Current.Single<ProcessInfoLine>(item.ProcessInfoLineID),
                    Reservation = DA.Current.Single<Reservation>(reservationId),
                    RunNumber = item.RunNumber,
                    Special = item.Special,
                    Value = item.Value
                };

                DA.Current.Insert(rpi);
            }
        }

        public static void CopyInvitees(int reservationId, IEnumerable<IReservationInvitee> invitees)
        {

            foreach (var item in invitees)
            {
                var ri = new ReservationInvitee()
                {
                    Invitee = DA.Current.Single<Client>(item.InviteeID),
                    Reservation = DA.Current.Single<Reservation>(reservationId)
                };

                DA.Current.Insert(ri);
            }
        }

        public static Reservation GetPreviousRecurrence(int recurrenceId, int notReservationId = 0)
        {
            var result = DA.Current.Query<Reservation>()
                .Where(x => x.RecurrenceID == recurrenceId && x.ReservationID != notReservationId)
                .OrderByDescending(x => x.BeginDateTime)
                .FirstOrDefault();

            return result;
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
