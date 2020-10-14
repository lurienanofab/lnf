namespace LNF.Scheduler
{
    public interface IReservationProcessInfo
    {
        int ReservationProcessInfoID { get; set; }
        int ReservationID { get; set; }
        double Value { get; set; }
        bool Special { get; set; }
        int RunNumber { get; set; }
        double ChargeMultiplier { get; set; }
        bool Active { get; set; }
        int ProcessInfoLineID { get; set; }
        string Param { get; set; }
        int ProcessInfoID { get; set; }
        string ProcessInfoName { get; set; }
    }
}