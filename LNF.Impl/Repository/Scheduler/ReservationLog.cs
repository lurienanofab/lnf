using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationLog : IDataItem
    {
        public virtual int ReservationLogID { get; set; }
        public virtual ResourceLogProperty ResourceLogProperty { get; set; }
        public virtual Reservation Reservation { get; set; }
        public virtual String Value { get; set; }
    }
}