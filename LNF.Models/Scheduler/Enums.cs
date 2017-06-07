using System;

namespace LNF.Models.Scheduler
{
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
    }
}
