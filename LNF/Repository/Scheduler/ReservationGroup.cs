using LNF.Repository.Data;
using System;

namespace LNF.Repository.Scheduler
{
    public class ReservationGroup : IDataItem
    {
        public virtual int GroupID { get; set; }
        public virtual Client Client { get; set; }
        public virtual Account Account { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
