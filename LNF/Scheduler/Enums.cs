using System;

namespace LNF.Scheduler
{
    public enum ReservationState
    {
        Undefined = 0,          // LightYellow - also used after minCancel and before start
        Editable = 1,           // LightBlue 
        StartOrDelete = 2,      // LightGreen
        StartOnly = 3,          // LightGreen
        Endable = 4,            // OrangeRed
        PastSelf = 5,           // LightSteelBlue
        Other = 6,              // Plum
        Invited = 7,            // LightCoral - behaves like other
        PastOther = 8,          // MediumPurple
        Repair = 9,             // LightGray
        NotInLab = 10,          // xxxxx
        UnAuthToStart = 11,     // xxxxx
        ActiveNotEndable = 12,  // requested by Ed, so everyone can know the reservation is current active
        Meeting = 13
    }

    public enum ResourceNamePartial
    {
        /// <summary>
        /// The resource name of the Resource.
        /// </summary>
        ResourceName = 0,

        /// <summary>
        /// The process tech, and resource name of the Resource.
        /// </summary>
        ProcessTechName = 1,

        /// <summary>
        /// The lab, process tech, and resource name of the Resource.
        /// </summary>
        LabName = 2,

        /// <summary>
        /// The building, lab, process tech, and resource name of the Resource.
        /// </summary>
        BuildingName = 3
    }

    public enum GranularityDirection
    {
        Previous = 0,
        Next = 1
    }

    public enum ReservationModificationType
    {
        Created = 1,
        Modified = 2
    }

    public enum EmailNotify
    {
        Always = 1,
        OnOpening = 2
    }

    public enum OnTheFlyResourceType
    {
        Tool = 1,
        Cabinet = 2
    }

    public enum OnTheFlyCardSwipeAction
    {
        CreateAndStartReservation = 1,
        StartExistingReservation = 2
    }

    public enum ActivityAccountType
    {
        Reserver = 0,
        Invitee = 1,
        Both = 2
    }

    public enum ActivityInviteeType
    {
        None = 1,
        Optional = 2,
        Required = 3,
        Single = 4
    }

    [Flags]
    public enum ClientAuthLevel
    {
        UnauthorizedUser = 1,
        AuthorizedUser = 2,
        SuperUser = 4,
        Trainer = 8,
        ToolEngineer = 16,
        RemoteUser = 32
    }

    public enum ResourceState
    {
        Offline = 0,
        Online = 1,
        Limited = 2
    }

    public enum SchedulerTask
    {
        FiveMinute = 1,
        Daily = 2,
        Monthly = 3
    }

    public enum ViewType
    {
        DayView = 0,
        WeekView = 1,
        ProcessTechView = 2,
        UserView = 3,
        LocationView = 4
    }
}
