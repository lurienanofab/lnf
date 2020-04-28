using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationRecurrenceInfo : IReservationRecurrence, IDataItem
    {
        public virtual int RecurrenceID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual bool AutoEnd { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual DateTime BeginDate { get; set; }
        public virtual DateTime BeginTime { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual double Duration { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual string Notes { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string FName { get; set; }
        public virtual string LName { get; set; }
        public virtual int PatternID { get; set; }
        public virtual string PatternName { get; set; }
        public virtual int PatternParam1 { get; set; }
        public virtual int? PatternParam2 { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual int LabID { get; set; }
        public virtual int BuildingID { get; set; }
    }
}
