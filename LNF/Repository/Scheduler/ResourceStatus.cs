using System;

namespace LNF.Repository.Scheduler
{
    public class ResourceStatus : IDataItem
    {
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual bool ResourceIsActive { get; set; }
        public virtual bool Available { get; set; }
        public virtual string ActiveLName { get; set; }
        public virtual string ActiveFName { get; set; }
        public virtual DateTime? ActiveBeginDateTime { get; set; }
        public virtual DateTime? ActiveEndDateTime { get; set; }
        public virtual string UpcomingLName { get; set; }
        public virtual string UpcomingFName { get; set; }
        public virtual DateTime? UpcomingBeginDateTime { get; set; }
        public virtual DateTime? UpcomingEndDateTime { get; set; }
    }
}
