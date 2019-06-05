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
        public static bool GetRegularFromRecurring(IReservationRecurrence rr, DateTime currentDate, DataTable dtOut)
        {
            // first, find out the pattern type
            if (rr.PatternID == (int)PatternType.Weekly)
            {
                if (rr.PatternParam1 == (int)currentDate.DayOfWeek)
                {
                    if ((currentDate >= rr.BeginDate && rr.EndDate == null) || (currentDate >= rr.BeginDate && currentDate <= rr.EndDate.Value))
                        AddNewRow(rr, currentDate, dtOut);
                }
            }
            else if (rr.PatternID == (int)PatternType.Monthly)
            {
                if ((currentDate >= rr.BeginDate && rr.EndDate == null) || (currentDate >= rr.BeginDate && currentDate <= rr.EndDate))
                {
                    DateTime d = GetDate(new DateTime(currentDate.Year, currentDate.Month, 1), rr.PatternParam1, (DayOfWeek)rr.PatternParam2);
                    if (currentDate.Date == d)
                        AddNewRow(rr, currentDate, dtOut);
                }
            }
            else
                return false; //currently only supports two types

            return true;
        }

        private static void AddNewRow(IReservationRecurrence rr, DateTime currentDate, DataTable dtOut)
        {
            DataRow ndr = dtOut.NewRow();
            ndr["ReservationID"] = DBNull.Value;
            ndr["ResourceID"] = rr.ResourceID;
            ndr["ClientID"] = rr.ClientID;
            ndr["AccountID"] = rr.AccountID;
            ndr["ActivityID"] = rr.ActivityID;
            ndr["BeginDateTime"] = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, rr.BeginTime.Hour, rr.BeginTime.Minute, rr.BeginTime.Second);
            ndr["EndDateTime"] = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, rr.EndTime.Hour, rr.EndTime.Minute, rr.EndTime.Second);
            ndr["ActualBeginDateTime"] = DBNull.Value;
            ndr["ActualEndDateTime"] = DBNull.Value;
            ndr["ClientIDBegin"] = DBNull.Value;
            ndr["ClientIDEnd"] = DBNull.Value;
            ndr["CreatedOn"] = rr.CreatedOn;
            ndr["LastModifiedOn"] = DateTime.Now;
            ndr["Duration"] = rr.Duration;
            ndr["ChargeMultiplier"] = 1;
            ndr["RecurrenceID"] = rr.RecurrenceID;
            ndr["ApplyLateChargePenalty"] = 1;
            ndr["AutoEnd"] = rr.AutoEnd;
            ndr["KeepAlive"] = rr.KeepAlive;
            ndr["HasProcessInfo"] = 0;
            ndr["HasInvitees"] = 0;
            ndr["IsActive"] = rr.IsActive;
            ndr["IsStarted"] = false;
            ndr["IsUnloaded"] = false;
            ndr["ResourceName"] = rr.ResourceName;
            ndr["IsSchedulable"] = true;
            ndr["Editable"] = true;
            ndr["DisplayName"] = string.Empty;
            ndr["Notes"] = rr.Notes;

            dtOut.Rows.Add(ndr);
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
