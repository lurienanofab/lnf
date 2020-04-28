namespace LNF.Scheduler
{
    public interface IReservationProcessInfo
    {
        bool Active { get; set; }
        double ChargeMultiplier { get; set; }
        string Param { get; set; }
        string ParameterName { get; set; }
        int ProcessInfoID { get; set; }
        int ProcessInfoLineID { get; set; }
        int ProcessInfoLineParamID { get; set; }
        string ProcessInfoName { get; set; }
        int ReservationID { get; set; }
        int ReservationProcessInfoID { get; set; }
        int RunNumber { get; set; }
        bool Special { get; set; }
        double Value { get; set; }
    }
}