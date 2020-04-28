using System;

namespace LNF.Scheduler
{
    public interface IResourceStatus
    {
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        bool ResourceIsActive { get; set; }
        bool Available { get; set; }
        string ActiveLName { get; set; }
        string ActiveFName { get; set; }
        DateTime? ActiveBeginDateTime { get; set; }
        DateTime? ActiveEndDateTime { get; set; }
        string UpcomingLName { get; set; }
        string UpcomingFName { get; set; }
        DateTime? UpcomingBeginDateTime { get; set; }
        DateTime? UpcomingEndDateTime { get; set; }
    }
}
