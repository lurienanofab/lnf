using System;

namespace LNF.Repository.Scheduler
{
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
}
