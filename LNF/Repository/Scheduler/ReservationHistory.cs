using LNF.Repository.Data;
using System;

namespace LNF.Repository.Scheduler
{
    public class ReservationHistory : IDataItem
    {
        /*Constants*/
        public const string INSERT_FOR_MODIFICATION = "InsertForModification";

        public virtual int ReservationHistoryID { get; set; }
        public virtual Reservation Reservation { get; set; }
        public virtual string UserAction { get; set; }
        public virtual int? LinkedReservationID { get; set; }
        public virtual string ActionSource { get; set; }
        public virtual int? ModifiedByClientID { get; set; }
        public virtual DateTime ModifiedDateTime { get; set; }
        public virtual Account Account { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual double ChargeMultiplier { get; set; }

        public virtual Client GetModifiedByClient()
        {
            if (ModifiedByClientID.HasValue)
                return DA.Current.Single<Client>(ModifiedByClientID.Value);
            else
                return null;
        }
    }
}
