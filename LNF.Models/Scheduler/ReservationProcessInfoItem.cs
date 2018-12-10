namespace LNF.Models.Scheduler
{
    public class ReservationProcessInfoItem
    {
        public int ReservationProcessInfoID { get; set; }
        public int ProcessInfoLineID { get; set; }
        public int ProcessInfoLineParamID { get; set; }
        public int ProcessInfoID { get; set; }
        public int ReservationID { get; set; }
        public double Value { get; set; }
        public bool Special { get; set; }
        public int RunNumber { get; set; }
        public double ChargeMultiplier { get; set; }
        public bool Active { get; set; }
        public string ProcessInfoName { get; set; }
        public string Param { get; set; }
        public string ParameterName { get; set; }
    }
}
