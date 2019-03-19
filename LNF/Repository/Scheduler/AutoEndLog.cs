using LNF.Models.Data;
using LNF.Models.Scheduler;
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

        public static AutoEndLog AddEntry(ReservationItem item, string action)
        {
            var entry = new AutoEndLog()
            {
                ReservationID = item.ReservationID,
                ResourceID = item.ResourceID,
                ResourceName = item.ResourceName,
                ClientID = item.ClientID,
                DisplayName = item.GetClientDisplayName(),
                Timestamp = DateTime.Now,
                Action = action
            };

            DA.Current.Insert(entry);

            return entry;
        }
    }
}
