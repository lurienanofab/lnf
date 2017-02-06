using System;

namespace LNF.Repository.Data
{
    public class GoogleCalendarFeed : IDataItem
    {
        public virtual int GoogleCalendarFeedID { get; set; }
        public virtual string GoogleCalendarID { get; set; }
        public virtual Client Client { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime LastUsed { get; set; }
        public virtual bool Active { get; set; }
    }
}
