using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Scheduler;
using LNF.Repository.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Scheduler
{
    public class Reservation : IDataItem
    {
        public static readonly DateTime MinReservationBeginDate = new DateTime(1900, 1, 1);
        public static readonly DateTime MaxReservationEndDate = new DateTime(3000, 1, 1);
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

        public virtual bool IsRunning()
        {
            return ActualBeginDateTime != null && ActualEndDateTime == null;
        }

        public virtual bool InCurrentPeriod()
        {
            DateTime d = ((ActualEndDateTime == null) ? EndDateTime : ActualEndDateTime.Value).Date;
            DateTime period = new DateTime(d.Year, d.Month, 1);
            return RepositoryUtility.IsCurrentPeriod(period);
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

            return RepositoryUtility.IsOverlapped(BeginDateTime, EndDateTime, sdate, edate)
                || (ActualBeginDateTime.HasValue ? RepositoryUtility.IsOverlapped(ActualBeginDateTime.Value, ActualEndDateTime, sdate, edate) : false);
        }

        public virtual IQueryable<ReservationHistory> GetHistory()
        {
            return DA.Current.Query<ReservationHistory>().Where(x => x.Reservation.ReservationID == ReservationID);
        }

        public virtual IQueryable<ReservationInvitee> GetInvitees()
        {
            return DA.Current.Query<ReservationInvitee>().Where(x => x.Reservation.ReservationID == ReservationID);
        }

        public virtual void Start(int? startedByClientId, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'Start'

            //UPDATE dbo.Reservation
            //SET ActualBeginDateTime = GETDATE(),
            //    ClientIDBegin = @ClientID,
            //    IsStarted = 1		
            //WHERE ReservationID = @ReservationID

            ActualBeginDateTime = DateTime.Now;
            ClientIDBegin = startedByClientId;
            IsStarted = true;

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("Start", "procReservationUpdate", this, modifiedByClientId);
        }

        public virtual void End(int? endedByClientId, int? modifiedByClientId)
        {
            ActualEndDateTime = DateTime.Now;
            ClientIDEnd = endedByClientId;

            if (ActualBeginDateTime.Value > ActualEndDateTime.Value)
                throw new Exception(string.Format("ActualDateTime is greater than ActualEndDateTime [ReservationID: {0}, ResourceID: {1}, ActualBeginDateTime: {2:yyyy-MM-dd HH:mm:ss}, ActualEndDateTime: {3:yyyy-MM-dd HH:mm:ss}", ReservationID, Resource.ResourceID, ActualBeginDateTime, ActualEndDateTime));

            if (!Activity.Editable)
                Resource.State = ResourceState.Online;

            DA.Scheduler.ReservationHistory.Insert("End", "procReservationUpdate", this, modifiedByClientId);
        }

        public virtual void EndForRepair(int? endedByClientId, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'EndForRepair'

            //UPDATE dbo.Reservation
            //SET ActualEndDateTime = GETDATE(),
            //    ChargeMultiplier = 0,
            //    ApplyLateChargePenalty = 0,
            //    ClientIDEnd = @ClientID
            //WHERE ReservationID = @ReservationID

            ActualEndDateTime = DateTime.Now;
            ChargeMultiplier = 0;
            ApplyLateChargePenalty = false;
            ClientIDEnd = endedByClientId;

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("EndForRepair", "procReservationUpdate", this, modifiedByClientId);
        }

        public virtual void Insert(int? modifiedByClientId)
        {
            // procReservationInsert @Action = 'Insert'

            //INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
            //  BeginDateTime, EndDateTime, CreatedOn, LastModifiedOn,
            //  Duration, Notes, AutoEnd, ChargeMultiplier, RecurrenceID, ApplyLateChargePenalty, 
            //  HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, KeepAlive, MaxReservedDuration)
            //VALUES(@ResourceID, @ClientID, @AccountID, @ActivityID,
            //  @BeginDateTime, @EndDateTime, @CreatedOn, GETDATE(),
            //  @Duration, @Notes, @AutoEnd, 1.00, @RecurrenceID, 1,
            //  @HasProcessInfo, @HasInvitees, @IsActive, 0, 0, @KeepAlive, @MaxReservedDuration)

            CanCreateCheck();

            LastModifiedOn = DateTime.Now;
            ChargeMultiplier = 1;
            ApplyLateChargePenalty = true;
            IsStarted = false;
            IsUnloaded = false;

            DA.Current.Insert(this);

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("Insert", "procReservationInsert", this, modifiedByClientId);
        }

        public virtual void InsertForModification(int linkedReservationId, int? modifiedByClientId)
        {
            // The idea here is that linkedReservation is an existing reservation that has been modified (all propeties already set), and
            // we are using it to create a new reservation for modification. The ReservationID of linkedReservation will be used to link the
            // two reservations in ReservationHistory and the newly created reservation will be returned.

            // procReservationInsert @Action = 'InsertForModification'

            //INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
            //  BeginDateTime, EndDateTime, CreatedOn, LastModifiedOn,
            //  Duration, Notes, AutoEnd, ChargeMultiplier, RecurrenceID, ApplyLateChargePenalty, 
            //  HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, KeepAlive, MaxReservedDuration, TotalProcessRuns)
            //VALUES(@ResourceID, @ClientID, @AccountID, @ActivityID,
            //  @BeginDateTime, @EndDateTime, @CreatedOn, GETDATE(),
            //  @Duration, @Notes, @AutoEnd, 1.00, @RecurrenceID, 1,
            //  @HasProcessInfo, @HasInvitees, @IsActive, 0, 0, @KeepAlive, @MaxReservedDuration, @TotalProcessRuns)

            bool isInLab = CacheManager.Current.ClientInLab(Resource.ProcessTech.Lab.LabID);

            CanCreateCheck();

            DA.Current.Insert(this);

            DA.Scheduler.ReservationHistory.Insert("InsertForModification", "procReservationInsert", this, modifiedByClientId, linkedReservationId);
        }

        public virtual void CanCreateCheck()
        {
            // ignore recurring reservations
            if (!RecurrenceID.HasValue)
            {
                // These two activites allow for creating a reservation in the past
                if (Activity != Properties.Current.Activities.FacilityDownTime && Activity != Properties.Current.Activities.Repair)
                {
                    // Granularity			: stored in minutes and entered in minutes
                    // Offset				: stored in hours and entered in hours 
                    DateTime granStartTime = ResourceUtility.GetNextGranularity(TimeSpan.FromMinutes(Resource.Granularity), TimeSpan.FromHours(Resource.Offset), DateTime.Now, NextGranDir.Previous);

                    if (BeginDateTime < granStartTime)
                    {
                        string body = string.Format("Unable to create a reservation. BeginDateTime is in the past.\n--------------------\nClient: {0} [{1}]\nResource: {2} [{3}]\nBeginDateTime: {4:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {5:yyyy-MM-dd HH:mm:ss}", Client.DisplayName, Client.ClientID, Resource.ResourceName, Resource.ResourceID, BeginDateTime, EndDateTime);
                        Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Repository.Scheduler.Reservation.CanCreateCheck()", "Create reservation failed", body, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                        throw new Exception("Your reservation was not created. Cannot create a reservation in the past.");
                    }
                }
            }

            if (EndDateTime <= BeginDateTime)
            {
                string body = string.Format("Unable to create a reservation. EndDateTime is before BeginDateTime.\n--------------------\nClient: {0} [{1}]\nResource: {2} [{3}]\nBeginDateTime: {4:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {5:yyyy-MM-dd HH:mm:ss}", Client.DisplayName, Client.ClientID, Resource.ResourceName, Resource.ResourceID, BeginDateTime, EndDateTime);
                Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Repository.Scheduler.Reservation.CanCreateCheck()", "Create reservation failed", body, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                throw new Exception("Your reservation was not created. Cannot create a reservation that ends before it starts.");
            }

            // conflicting reservations must be:
            //      1) same resource
            //      2) not canceled
            //      3) date ranges overlap

            var conflict = DA.Current.Query<Reservation>().Where(x =>
                x.Resource == Resource // same resource
                && x.IsActive // not canceled
                && !x.ActualEndDateTime.HasValue // not ended
                && (x.BeginDateTime < EndDateTime && x.EndDateTime > BeginDateTime) // date ranges overlap
            ).FirstOrDefault();

            if (conflict != null)
            {
                string body = string.Format("Unable to create a reservation. There is a conflict with an existing reservation.\n--------------------\nClient: {0} [{1}]\nResource: {2} [{3}]\nBeginDateTime: {4:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {5:yyyy-MM-dd HH:mm:ss}\nConflicting ReservationID: {6}", Client.DisplayName, Client.ClientID, Resource.ResourceName, Resource.ResourceID, BeginDateTime, EndDateTime, conflict.ReservationID);
                Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Repository.Scheduler.Reservation.CanCreateCheck()", "Create reservation failed", body, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                throw new Exception(string.Format("Your reservation was not created. There is a conflict with an existing reservation [#{0}].", conflict.ReservationID));
            }
        }

        public virtual void Update(int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'Update'

            //DECLARE @MaxReservedDuration FLOAT
            //SELECT @MaxReservedDuration = MaxReservedDuration
            //FROM Reservation 
            //WHERE ReservationID = @ReservationID

            //DECLARE @OriginalModifiedDateTime DATETIME2 = NULL
            //IF @OriginalBeginDateTime IS NOT NULL
            //BEGIN
            //    SELECT @OriginalModifiedDateTime = OriginalModifiedOn FROM Reservation WHERE ReservationID = @ReservationID
            //    IF @OriginalModifiedDateTime IS NULL	
            //        SET @OriginalModifiedDateTime = GETDATE()
            //END

            if (OriginalBeginDateTime.HasValue)
            {
                if (!OriginalModifiedOn.HasValue)
                    OriginalModifiedOn = DateTime.Now;
            }

            //UPDATE dbo.Reservation
            //SET AccountID = @AccountID,
            //    ActivityID = @ActivityID,
            //    BeginDateTime = @BeginDateTime,
            //    EndDateTime = @EndDateTime,
            //    LastModifiedOn = GETDATE(),
            //    Duration = @Duration,
            //    MaxReservedDuration = CASE WHEN @Duration > @MaxReservedDuration THEN @Duration ELSE MaxReservedDuration END,
            //    Notes = @Notes,
            //    AutoEnd = @AutoEnd,
            //    HasProcessInfo = @HasProcessInfo,
            //    HasInvitees = @HasInvitees,
            //    RecurrenceID = @RecurrenceID,
            //    KeepAlive = @KeepAlive,
            //    OriginalBeginDateTime = @OriginalBeginDateTime,
            //    OriginalEndDateTime = @OriginalEndDateTime,
            //    OriginalModifiedOn = @OriginalModifiedDateTime
            //WHERE ReservationID = @ReservationID

            Duration = EndDateTime.Subtract(BeginDateTime).TotalMinutes;
            MaxReservedDuration = Math.Max(Duration, MaxReservedDuration);
            LastModifiedOn = DateTime.Now;

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("Update", "procReservationUpdate", this, modifiedByClientId);
        }

        public virtual void Delete(int? modifiedByClientId)
        {
            // This is all that happens in procReservationDelete @Action = 'ByReservationID'

            //-- Set Reservation to inactive
            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE(), CancelledDateTime = GETDATE()
            //WHERE ReservationID = @ReservationID

            IsActive = false;
            LastModifiedOn = DateTime.Now;
            CancelledDateTime = DateTime.Now;

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("ByReservationID", "procReservationDelete", this, modifiedByClientId);
        }

        public virtual void DeleteAndForgive(int? modifiedByClientId)
        {
            // This is all that happens in procReservationDelete @Action = 'WithForgive'

            //-- Set Reservation to inactive
            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE(), CancelledDateTime = GETDATE(), ChargeMultiplier = 0
            //WHERE ReservationID = @ReservationID

            IsActive = false;
            LastModifiedOn = DateTime.Now;
            CancelledDateTime = DateTime.Now;
            ChargeMultiplier = 0;

            //-- Delete Reservation ProcessInfos
            //DELETE FROM dbo.ReservationProcessInfo
            //WHERE ReservationID = @ReservationID

            DA.Current.Delete(DA.Current.Query<ReservationProcessInfo>().Where(x => x.Reservation.ReservationID == ReservationID));

            //-- Delete Reservation Invitees
            //DELETE FROM dbo.ReservationInvitee
            //WHERE ReservationID = @ReservationID

            DA.Current.Delete(DA.Current.Query<ReservationInvitee>().Where(x => x.Reservation.ReservationID == ReservationID));

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("WithForgive", "procReservationDelete", this, modifiedByClientId);
        }

        public virtual void UpdateCharges(double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId)
        {
            // procReservationUpdate @Action = 'UpdateCharges'

            //UPDATE dbo.Reservation
            //SET ChargeMultiplier = @ChargeMultiplier,
            //    ApplyLateChargePenalty = @ApplyLateChargePenalty
            //WHERE ReservationID = @ReservationID

            ChargeMultiplier = chargeMultiplier;
            ApplyLateChargePenalty = applyLateChargePenalty;

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("UpdateCharges", "procReservationUpdate", this, modifiedByClientId);
        }

        public virtual void UpdateFacilityDownTime(int? modifiedByClientId)
        {
            // this is all that happens in procReservationUpdate @Action = 'UpdateFacilityDownTime'

            //UPDATE dbo.Reservation
            //SET AccountID = @AccountID,
            //    ActivityID = @ActivityID,
            //    BeginDateTime = @BeginDateTime,
            //    EndDateTime = @EndDateTime,
            //    ActualBeginDateTime = @ActualBeginDateTime,
            //    ActualEndDateTime = @ActualEndDateTime,		
            //    LastModifiedOn = GETDATE(),
            //    Duration = @Duration,
            //    Notes = @Notes,
            //    AutoEnd = @AutoEnd,
            //    HasProcessInfo = @HasProcessInfo,
            //    HasInvitees = @HasInvitees,
            //    RecurrenceID = @RecurrenceID,
            //    TotalProcessRuns = @TotalProcessRuns
            //WHERE ReservationID = @ReservationID

            LastModifiedOn = DateTime.Now;

            // also an entry into history is made
            DA.Scheduler.ReservationHistory.Insert("UpdateFacilityDownTime", "procReservationUpdate", this, modifiedByClientId);
        }

        public virtual bool IsRecurring()
        {
            return RecurrenceID.HasValue && RecurrenceID.Value > 0;
        }

        public virtual IList<ClientAccount> AvailableAccounts()
        {
            IList<ClientAccount> result = null;

            DateTime sd = CreatedOn.Date;
            DateTime ed = sd.AddDays(1);

            if (Activity.AccountType == ActivityAccountType.Reserver || Activity.AccountType == ActivityAccountType.Both)
            {
                //Load reserver's accounts
                result = Client.ActiveClientAccounts(sd, ed).ToList();
            }

            if (Activity.AccountType == ActivityAccountType.Invitee || Activity.AccountType == ActivityAccountType.Both)
            {
                //Loads each of the invitee's accounts
                foreach (ReservationInvitee ri in GetInvitees())
                {
                    IQueryable<ClientAccount> temp = ri.Invitee.ActiveClientAccounts(sd, ed);

                    if (result == null)
                        result = temp.ToList();
                    else
                    {
                        foreach (ClientAccount t in temp)
                        {
                            //Check if account already exists
                            if (!result.Any(i => i.Account == t.Account))
                            {
                                result.Add(t);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public virtual void InsertFacilityDownTime(int? modifiedByClientId)
        {
            // This is all that happens in procReservationInsert @Action = 'InsertFacilityDownTime'

            //INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
            //  BeginDateTime, EndDateTime, ActualBeginDateTime, ActualEndDateTime, ClientIDBegin, ClientIDEnd, CreatedOn, LastModifiedOn,
            //  Duration, Notes, AutoEnd, ChargeMultiplier, RecurrenceID, GroupID, ApplyLateChargePenalty, 
            //  HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, MaxReservedDuration, KeepAlive)
            //VALUES(@ResourceID, @ClientID, @AccountID, @ActivityID,
            //  @BeginDateTime, @EndDateTime, @ActualBeginDateTime, @ActualEndDateTime, @ClientID, @ClientID, @CreatedOn, GETDATE(),
            //  @Duration, @Notes, @AutoEnd, 1.00, @RecurrenceID, @GroupID, 1,
            //  @HasProcessInfo, @HasInvitees, @IsActive, 0, 0, @Duration, 0)

            CanCreateCheck();

            LastModifiedOn = DateTime.Now;
            ChargeMultiplier = 1;
            ApplyLateChargePenalty = true;
            IsStarted = false;
            IsUnloaded = false;
            KeepAlive = false;

            DA.Current.Insert(this);

            DA.Scheduler.ReservationHistory.Insert("InsertFacilityDownTime", "procReservationInsert", this, modifiedByClientId);
        }

        public virtual bool IsInvited(Client c)
        {
            return DA.Current.Query<ReservationInvitee>().Any(x => x.Reservation.ReservationID == ReservationID && x.Invitee == c);
        }

        public virtual bool IsInLab()
        {
            string kioskIp = Providers.Context.Current.UserHostAddress;
            return KioskUtility.ClientInLab(Resource.ProcessTech.Lab.LabID, Client.ClientID, kioskIp);
        }
    }
}
