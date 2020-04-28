using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class ToolData : IDataItem
    {
        public virtual int ToolDataID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual int? RoomID { get; set; }
        public virtual DateTime ActDate { get; set; }
        public virtual int AccountID { get; set; }
        public virtual double Uses { get; set; }
        public virtual double SchedDuration { get; set; }
        public virtual double ActDuration { get; set; }
        public virtual double OverTime { get; set; }
        public virtual double? Days { get; set; }
        public virtual double? Months { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual int? ReservationID { get; set; }
        public virtual double ChargeDuration { get; set; }
        public virtual double TransferredDuration { get; set; }
        public virtual double MaxReservedDuration { get; set; }
        public virtual DateTime? ChargeBeginDateTime { get; set; }
        public virtual DateTime? ChargeEndDateTime { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool? IsCancelledBeforeAllowedTime { get; set; }
    }
}
