using System;

namespace LNF.Scheduler
{
    public class ResourceStatusItem : IResourceStatus
    {
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public bool ResourceIsActive { get; set; }
        public bool Available { get; set; }
        public string ActiveLName { get; set; }
        public string ActiveFName { get; set; }
        public DateTime? ActiveBeginDateTime { get; set; }
        public DateTime? ActiveEndDateTime { get; set; }
        public string UpcomingLName { get; set; }
        public string UpcomingFName { get; set; }
        public DateTime? UpcomingBeginDateTime { get; set; }
        public DateTime? UpcomingEndDateTime { get; set; }
    }
}
