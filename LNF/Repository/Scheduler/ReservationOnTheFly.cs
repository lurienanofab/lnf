using System; 
using System.Collections.Generic; 
using System.Text; 

namespace LNF.Repository.Scheduler
{    
    public class ReservationOnTheFly : IDataItem
    {
        public ReservationOnTheFly() { }
        public virtual int ReservationOnTheFlyID { get; set; }
        public virtual Reservation Reservation { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual string OnTheFlyName { get; set; }
		public virtual string IPAddress { get; set; }
		public virtual string CardReaderName { get; set; }
    }
}
