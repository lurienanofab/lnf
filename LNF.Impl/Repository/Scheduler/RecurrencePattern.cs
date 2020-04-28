using LNF.DataAccess;
using System.Collections.Generic;

namespace LNF.Impl.Repository.Scheduler
{
    public class RecurrencePattern : IDataItem
    {
        public RecurrencePattern()
        {
            ReservationRecurrences = new List<ReservationRecurrence>();
        }

        public virtual int PatternID { get; set; }
        public virtual IList<ReservationRecurrence> ReservationRecurrences { get; set; }
        public virtual string PatternName { get; set; }
    }
}
