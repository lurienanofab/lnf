namespace LNF.Models.Scheduler
{
    public class ReservationProcessInfoModel
    {
        public virtual int ReservationProcessInfoID { get; set; }
        public virtual int ReservationID { get; set; }
        public virtual int ProcessInfoLineID { get; set; }
        public virtual double Value { get; set; }
        public virtual bool Special { get; set; }
        public virtual int RunNumber { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual int ProcessInfoID { get; set; } // this variable is not used in saving, only for compatability
    }
}
