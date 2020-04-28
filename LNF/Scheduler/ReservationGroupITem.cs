using System;

namespace LNF.Scheduler
{
    public class ReservationGroupItem : IReservationGroup
    {
        public int GroupID { get; set; }
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public int ActivityID { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
