using LNF.Models.Scheduler;
using LNF.Repository.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Repository.Scheduler
{
    public class ReservationInfo : IDataItem
    {
        public virtual int ReservationID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int GracePeriod { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual bool NNINOrg { get; set; }
        public virtual bool PrimaryOrg { get; set; }
        public virtual int OrgTypeID { get; set; }
        public virtual string OrgTypeName { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual string ChargeTypeName { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual DateTime ChargeBeginDateTime { get; set; }
        public virtual DateTime ChargeEndDateTime { get; set; }
        public virtual int? ClientIDBegin { get; set; }
        public virtual string ClientBeginLName { get; set; }
        public virtual string ClientBeginFName { get; set; }
        public virtual int? ClientIDEnd { get; set; }
        public virtual string ClientEndLName { get; set; }
        public virtual string ClientEndFName { get; set; }
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
        public virtual int RecurrenceID { get; set; }
        public virtual int GroupID { get; set; }
        public virtual double MaxReservedDuration { get; set; }
        public virtual DateTime? CancelledDateTime { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual DateTime? OriginalBeginDateTime { get; set; }
        public virtual DateTime? OriginalEndDateTime { get; set; }
        public virtual DateTime? OriginalModifiedOn { get; set; }

        public virtual ResourceCost GetResourceCost(IEnumerable<Cost> costs)
        {
            ResourceCost result = new ResourceCost(costs, ResourceID, ChargeTypeID, ChargeBeginDateTime);
            return result;
        }

        public virtual bool IsCurrentlyOutsideGracePeriod()
        {
            DateTime gp = BeginDateTime.AddMinutes(GracePeriod);
            return DateTime.Now > gp;
        }

        public virtual bool IsCancelledBeforeCutoff()
        {
            if (!CancelledDateTime.HasValue)
                return false;
            else
                return CancelledDateTime.Value.AddHours(2) < BeginDateTime;
        }
    }
}
