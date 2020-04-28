using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ResourceInfo : IResource, IDataItem
    {
        public virtual int ResourceID { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual int LabID { get; set; }
        public virtual int BuildingID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual string ProcessTechDescription { get; set; }
        public virtual int ProcessTechGroupID { get; set; }
        public virtual string ProcessTechGroupName { get; set; }
        public virtual bool ProcessTechIsActive { get; set; }
        public virtual string LabName { get; set; }
        public virtual string LabDisplayName { get; set; }
        public virtual string LabDescription { get; set; }
        public virtual bool LabIsActive { get; set; }
        public virtual int RoomID { get; set; }
        public virtual string RoomName { get; set; }
        public virtual string RoomDisplayName { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual string BuildingDescription { get; set; }
        public virtual bool BuildingIsActive { get; set; }
        public virtual bool ResourceIsActive { get; set; }
        public virtual bool IsSchedulable { get; set; }
        public virtual string ResourceDescription { get; set; }
        public virtual string HelpdeskEmail { get; set; }
        public virtual string WikiPageUrl { get; set; }
        public virtual ResourceState State { get; set; }
        public virtual string StateNotes { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual bool AuthState { get; set; }
        public virtual int ReservFence { get; set; }
        public virtual int MaxAlloc { get; set; }
        public virtual int MinCancelTime { get; set; }
        public virtual int ResourceAutoEnd { get; set; }
        public virtual int? UnloadTime { get; set; }
        public virtual int? OTFSchedTime { get; set; }
        public virtual int Granularity { get; set; }
        public virtual int Offset { get; set; }
        public virtual bool IsReady { get; set; }
        public virtual int MinReservTime { get; set; }
        public virtual int MaxReservTime { get; set; }
        public virtual int GracePeriod { get; set; }
        public virtual bool HasState(ResourceState state) => Resources.HasState(State, state);
        public virtual string ResourceDisplayName => Resources.GetResourceDisplayName(ResourceName, ResourceID);
        public virtual string GetResourceName(ResourceNamePartial part) => Resources.GetResourceName(this, part);
        public virtual DateTime GetNextGranularity(DateTime now, GranularityDirection dir) => Resources.GetNextGranularity(Granularity, Offset, now, dir);
        public virtual string GetImageUrl() => $"//ssel-sched.eecs.umich.edu/sselscheduler/images/Resource/Resource{ResourceID:000000}.png";
        public override string ToString() => ResourceDisplayName;
    }
}
