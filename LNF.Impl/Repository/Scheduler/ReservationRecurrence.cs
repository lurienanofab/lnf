using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationRecurrence : IDataItem
    {
        public virtual int RecurrenceID { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual Client Client { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual Account Account { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual bool AutoEnd { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual RecurrencePattern Pattern { get; set; }
        public virtual int PatternParam1 { get; set; }
        public virtual int? PatternParam2 { get; set; }
        public virtual DateTime BeginDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DateTime BeginTime { get; set; }
        public virtual double Duration { get; set; }
        public virtual string Notes { get; set; }
    }
}