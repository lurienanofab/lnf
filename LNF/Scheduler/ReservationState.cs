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
}
