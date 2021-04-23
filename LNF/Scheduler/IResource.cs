namespace LNF.Scheduler
{
    public interface IResource : IResourceItem, IProcessTech
    {
        bool ResourceIsActive { get; set; }

        bool IsSchedulable { get; set; }

        string ResourceDescription { get; set; }

        string HelpdeskEmail { get; set; }

        string WikiPageUrl { get; set; }

        ResourceState State { get; set; }

        string StateNotes { get; set; }

        /// <summary>
        /// Minutes a user may reserve in total (limits the total duration of unstarted reservations within the fence). Stored in minutes and entered in hours (20 hours = 1200 minutes).
        /// </summary>
        int MaxAlloc { get; set; }

        /// <summary>
        /// Minutes required to unload the tool (used by interlock actions). Stored in minutes and entered in minutes.
        /// </summary>
        int? UnloadTime { get; set; }

        /// <summary>
        /// Minutes that on-the-fly reservations should be scheduled for (not currently used by on-the-fly system). Stored in minutes and entered in minutes.
        /// </summary>
        int? OTFSchedTime { get; set; }

        bool IsReady { get; set; }

        int RoomID { get; set; }

        string RoomName { get; set; }

        string RoomDisplayName { get; set; }

        bool HasState(ResourceState state);

        string GetResourceName(ResourceNamePartial part);
    }
}
