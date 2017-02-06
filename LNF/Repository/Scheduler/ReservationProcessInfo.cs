namespace LNF.Repository.Scheduler
{
    public class ReservationProcessInfo : IDataItem
    {
        public virtual int ReservationProcessInfoID { get; set; }
        public virtual Reservation Reservation { get; set; }
        public virtual ProcessInfoLine ProcessInfoLine { get; set; }
        public virtual double Value { get; set; }
        public virtual bool Special { get; set; }
        public virtual int RunNumber { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual bool Active { get; set; }
    }
}
