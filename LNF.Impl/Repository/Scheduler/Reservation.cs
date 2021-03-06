using LNF.CommonTools;
using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using LNF.Scheduler;
using System;
using System.Linq;

namespace LNF.Impl.Repository.Scheduler
{
    public class Reservation : IDataItem
    {
        private ClientAccountInfo _clientAccountInfo = null;

        public static readonly string DateFormat = "MM/dd/yyyy hh:mm tt";

        public Reservation()
        {
            // these default values were used in ReservationDB...
            IsActive = true;
            IsStarted = false;
            IsUnloaded = true;
        }

        public virtual int ReservationID { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual Client Client { get; set; }
        public virtual Account Account { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual int? ClientIDBegin { get; set; }
        public virtual int? ClientIDEnd { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime LastModifiedOn { get; set; }
        public virtual double Duration { get; set; }
        public virtual string Notes { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual bool ApplyLateChargePenalty { get; set; }
        public virtual bool AutoEnd { get; set; }
        public virtual bool HasProcessInfo { get; set; }
        public virtual bool HasInvitees { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual bool IsUnloaded { get; set; }
        public virtual int? RecurrenceID { get; set; }
        public virtual int? GroupID { get; set; }
        public virtual double MaxReservedDuration { get; set; }
        public virtual DateTime? CancelledDateTime { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual DateTime? OriginalBeginDateTime { get; set; }
        public virtual DateTime? OriginalEndDateTime { get; set; }
        public virtual DateTime? OriginalModifiedOn { get; set; }

        public virtual void UpdateNotes(string notes)
        {
            // procReservationUpdate @Action = 'UpdateNotes'

            //UPDATE dbo.Reservation
            //SET Notes = @Notes
            //WHERE ReservationID = @ReservationID

            Notes = notes;
        }

        public virtual void AppendNotes(string notes)
        {
            if (!string.IsNullOrEmpty(notes))
            {
                if (!string.IsNullOrEmpty(Notes))
                    Notes += Environment.NewLine + Environment.NewLine + notes;
                else
                    Notes = notes;
            }
        }

        public virtual double ReservedDuration()
        {
            return (EndDateTime - BeginDateTime).TotalMinutes;
        }

        public virtual double ActualDuration()
        {
            if (ActualBeginDateTime != null && ActualEndDateTime != null)
                return (ActualEndDateTime.Value - ActualBeginDateTime.Value).TotalMinutes;
            return 0;
        }

        public virtual DateTime ChargeBeginDateTime()
        {
            DateTime result;
            if (ActualBeginDateTime == null)
                result = BeginDateTime;
            else if (BeginDateTime < ActualBeginDateTime.Value)
                result = BeginDateTime;
            else
                result = ActualBeginDateTime.Value;
            return result;
        }

        public virtual ClientAccountInfo GetClientAccount(NHibernate.ISession session)
        {
            // client/account mismatches are possible because of remote processing, so this may be null
            if (_clientAccountInfo == null)
                _clientAccountInfo = session.Query<ClientAccountInfo>().FirstOrDefault(x => x.ClientID == Client.ClientID && x.AccountID == Account.AccountID);
            return _clientAccountInfo;
        }

        public virtual string GetPhone(NHibernate.ISession session)
        {
            var ca = GetClientAccount(session);
            if (ca != null)
                return ca.Phone;
            else
                return session.Get<ClientInfo>(Client.ClientID).Phone;
        }

        public virtual string GetEmail(NHibernate.ISession session)
        {
            var ca = GetClientAccount(session);
            if (ca != null)
                return ca.Email;
            else
                return session.Get<ClientInfo>(Client.ClientID).Email;
        }

        public virtual DateTime ChargeEndDateTime()
        {
            if (ActualEndDateTime == null) return EndDateTime;
            return (EndDateTime > ActualEndDateTime.Value) ? EndDateTime : ActualEndDateTime.Value;
        }

        public virtual double ChargeDuration()
        {
            return (ChargeEndDateTime() - ChargeBeginDateTime()).TotalMinutes;
        }

        public virtual double Overtime()
        {
            if (ActualEndDateTime == null) return 0;
            double result = (ActualEndDateTime.Value - EndDateTime).TotalMinutes;
            return Math.Max(result, 0);
        }

        public virtual bool IsRunning() => Reservations.GetIsRunning(ActualBeginDateTime, ActualEndDateTime);

        public virtual bool InCurrentPeriod()
        {
            DateTime d = ((ActualEndDateTime == null) ? EndDateTime : ActualEndDateTime.Value).Date;
            DateTime period = new DateTime(d.Year, d.Month, 1);
            return Utility.IsCurrentPeriod(period);
        }

        public virtual DateTime GetBeginDateTime()
        {
            return (ActualBeginDateTime == null) ? BeginDateTime : ActualBeginDateTime.Value;
        }

        public virtual DateTime GetEndDateTime()
        {
            return (ActualEndDateTime == null) ? EndDateTime : ActualEndDateTime.Value;
        }

        public virtual bool IsReservationInDateRange(DateTime sdate, DateTime edate)
        {
            //original: I think this is wrong
            //(BeginDateTime >= @sDate and BeginDateTime < @eDate) OR 
            //(ActualEndDateTime > @sDate and ActualEndDateTime <= @eDate) OR
            //(BeginDateTime < @sDate AND EndDateTime > @sDate) OR			
            //(ActualBeginDateTime >= @sDate AND ActualBeginDateTime <= @eDate)

            //this is the correct way to check if two date ranges overlap
            //StartA = (Actual)BeginDateTime
            //EndA   = (Actual)EndDateTime
            //StartB = @sDate
            //EndB   = @eDate
            //(StartA <= EndB) and (EndA > StartB)

            //CORRECTION
            //(StartA < EndB) and (EndA > StartB) because the range StartB to EndB is also exclusive

            return Utility.Overlap(BeginDateTime, EndDateTime, sdate, edate)
                || (ActualBeginDateTime.HasValue ? Utility.Overlap(ActualBeginDateTime.Value, ActualEndDateTime, sdate, edate) : false);
        }

        public virtual bool IsRecurring()
        {
            return RecurrenceID.HasValue && RecurrenceID.Value > 0;
        }

        public virtual string GetClientBeginLName(NHibernate.ISession session)
        {
            var c = GetClientBegin(session);
            if (c == null) return string.Empty;
            return c.LName;
        }

        public virtual string GetClientBeginFName(NHibernate.ISession session)
        {
            var c = GetClientBegin(session);
            if (c == null) return string.Empty;
            return c.FName;
        }

        public virtual string GetClientEndLName(NHibernate.ISession session)
        {
            var c = GetClientEnd(session);
            if (c == null) return string.Empty;
            return c.LName;
        }

        public virtual string GetClientEndFName(NHibernate.ISession session)
        {
            var c = GetClientEnd(session);
            if (c == null) return string.Empty;
            return c.FName;
        }

        public virtual Client GetClientBegin(NHibernate.ISession session)
        {
            if (!ClientIDBegin.HasValue) return null;
            return session.Get<Client>(ClientIDBegin.Value);
        }

        public virtual Client GetClientEnd(NHibernate.ISession session)
        {
            if (!ClientIDEnd.HasValue) return null;
            return session.Get<Client>(ClientIDEnd.Value);
        }
    }
}
