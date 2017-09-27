using System;

namespace LNF.Repository.Scheduler
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

        public static AutoEndLog AddEntry(Reservation rsv, string action)
        {
            var entry = new AutoEndLog()
            {
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.Resource.ResourceID,
                ResourceName = rsv.Resource.ResourceName,
                ClientID = rsv.Client.ClientID,
                DisplayName = rsv.Client.DisplayName,
                Timestamp = DateTime.Now,
                Action = action
            };

            DA.Current.Insert(entry);

            return entry;
        }
    }
}
