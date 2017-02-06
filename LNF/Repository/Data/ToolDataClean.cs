using System;

namespace LNF.Repository.Data
{
    public class ToolDataClean : IDataItem
    {
        public virtual int ToolDataID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual int RoomID { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime ActualBeginDateTime { get; set; }
        public virtual DateTime ActualEndDateTime { get; set; }
        public virtual int AccountID { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual double SchedDuration { get; set; }
        public virtual double ActDuration { get; set; }
        public virtual double OverTime { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual int ReservationID { get; set; }
        public virtual double MaxReservedDuration { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime? CancelledDateTime { get; set; }
        public virtual DateTime? OriginalBeginDateTime { get; set; }
        public virtual DateTime? OriginalEndDateTime { get; set; }
        public virtual DateTime? OriginalModifiedOn { get; set; }
        public virtual DateTime CreatedOn { get; set; }

        public virtual DateTime GetChargeBeginDateTime()
        {
            if (ActualBeginDateTime == default(DateTime))
                return BeginDateTime;

            if (ActualBeginDateTime < BeginDateTime)
                return ActualBeginDateTime;
            else
                return BeginDateTime;
        }

        public virtual DateTime GetChargeEndDateTime()
        {
            if (ActualEndDateTime == default(DateTime))
                return EndDateTime;

            if (ActualEndDateTime > EndDateTime)
                return ActualEndDateTime;
            else
                return EndDateTime;
        }
    }
}
