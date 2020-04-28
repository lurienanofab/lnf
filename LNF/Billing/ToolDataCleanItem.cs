using System;

namespace LNF.Billing
{
    public class ToolDataCleanItem : IToolDataClean
    {
        public int ToolDataID { get; set; }
        public int ClientID { get; set; }
        public int ResourceID { get; set; }
        public int RoomID { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime ActualBeginDateTime { get; set; }
        public DateTime ActualEndDateTime { get; set; }
        public int AccountID { get; set; }
        public int ActivityID { get; set; }
        public double SchedDuration { get; set; }
        public double ActDuration { get; set; }
        public double OverTime { get; set; }
        public bool IsStarted { get; set; }
        public double ChargeMultiplier { get; set; }
        public int ReservationID { get; set; }
        public double MaxReservedDuration { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public DateTime? OriginalBeginDateTime { get; set; }
        public DateTime? OriginalEndDateTime { get; set; }
        public DateTime? OriginalModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }

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
