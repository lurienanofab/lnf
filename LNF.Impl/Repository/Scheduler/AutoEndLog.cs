using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class AutoEndLog : IDataItem
    {
        public virtual int AutoEndLogID { get; set; }
        public virtual int ReservationID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual DateTime Timestamp { get; set; }
        public virtual string Action { get; set; }
    }
}
