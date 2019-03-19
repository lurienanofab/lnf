using System.Linq;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Data;
using LNF.Models.Scheduler;

namespace LNF.Scheduler
{
    public enum PatternType
    {
        Weekly = 1,
        Monthly = 2
    }

    public static class RecurringReservationTransform
    {
        public static bool GetRegularFromRecurring(ReservationRecurrence rr, DateTime currentDate, DataTable dtOut)
        {
            // first, find out the pattern type
            if (rr.Pattern.PatternID == (int)PatternType.Weekly)
            {
                if (rr.PatternParam1 == (int)currentDate.DayOfWeek)
                {
                    if ((currentDate >= rr.BeginDate && rr.EndDate == null) || (currentDate >= rr.BeginDate && currentDate <= rr.EndDate.Value))
                        AddNewRow(rr, currentDate, dtOut);
                }
            }
            else if (rr.Pattern.PatternID == (int)PatternType.Monthly)
            {
                if ((currentDate >= rr.BeginDate && rr.EndDate == null) || (currentDate >= rr.BeginDate && currentDate <= rr.EndDate))
                {
                    DateTime d = ReservationRecurrenceUtility.GetDate(new DateTime(currentDate.Year, currentDate.Month, 1), rr.PatternParam1, (DayOfWeek)rr.PatternParam2);
                    if (currentDate.Date == d)
                        AddNewRow(rr, currentDate, dtOut);
                }
            }
            else
                return false; //currently only supports two types

            return true;
        }

        private static void AddNewRow(ReservationRecurrence rr, DateTime currentDate, DataTable dtOut)
        {
            DataRow ndr = dtOut.NewRow();
            ndr["ReservationID"] = DBNull.Value;
            ndr["ResourceID"] = rr.Resource.ResourceID;
            ndr["ClientID"] = rr.Client.ClientID;
            ndr["AccountID"] = rr.Account.AccountID;
            ndr["ActivityID"] = Properties.Current.Activities.ScheduledMaintenance.ActivityID;
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
            ndr["HasProcessInfo"] = 0;
            ndr["HasInvitees"] = 0;
            ndr["IsActive"] = rr.IsActive;
            ndr["IsStarted"] = false;
            ndr["IsUnloaded"] = false;
            ndr["ResourceName"] = rr.Resource.ResourceName;
            ndr["IsSchedulable"] = true;
            ndr["Editable"] = true;
            ndr["DisplayName"] = string.Empty;
            ndr["KeepAlive"] = rr.KeepAlive;
            ndr["Notes"] = rr.Notes;

            dtOut.Rows.Add(ndr);
        }

        public static void CopyProcessInfo(int recurrenceId, ReservationItem rsv)
        {
            var previousRecurrence = GetPreviousRecurrence(recurrenceId, rsv.ReservationID);

            if (previousRecurrence != null)
            {
                var rsvProcInfos = DA.Current.Query<ReservationProcessInfo>().Where(x => x.Reservation.ReservationID == previousRecurrence.ReservationID);
                if (rsvProcInfos.Count() > 0)
                {
                    foreach (var item in rsvProcInfos)
                    {
                        ReservationProcessInfo rpi = new ReservationProcessInfo()
                        {
                            Active = item.Active,
                            ChargeMultiplier = item.ChargeMultiplier,
                            ProcessInfoLine = item.ProcessInfoLine,
                            Reservation = DA.Current.Single<Reservation>(rsv.ReservationID),
                            RunNumber = item.RunNumber,
                            Special = item.Special,
                            Value = item.Value
                        };

                        DA.Current.Insert(rpi);
                        rsv.HasProcessInfo = true;
                    }
                }
            }
        }

        public static void CopyInvitees(int recurrenceId, ReservationItem rsv)
        {
            var previousRecurrence = GetPreviousRecurrence(recurrenceId, rsv.ReservationID);

            if (previousRecurrence != null)
            {
                var rsvInvitees = DA.Current.Query<ReservationInvitee>().Where(x => x.Reservation.ReservationID == previousRecurrence.ReservationID);
                if (rsvInvitees.Count() > 0)
                {
                    foreach (var item in rsvInvitees)
                    {
                        ReservationInvitee ri = new ReservationInvitee()
                        {
                            Invitee = item.Invitee,
                            Reservation = DA.Current.Single<Reservation>(rsv.ReservationID)
                        };

                        DA.Current.Insert(ri);
                        rsv.HasInvitees = true;
                    }
                }
            }
        }

        public static Reservation GetPreviousRecurrence(int recurrenceId, int reservationId)
        {
            var result = DA.Current.Query<Reservation>()
                .Where(x => x.RecurrenceID == recurrenceId && x.ReservationID != reservationId)
                .OrderByDescending(x => x.BeginDateTime)
                .FirstOrDefault();

            return result;
        }
    }
}
