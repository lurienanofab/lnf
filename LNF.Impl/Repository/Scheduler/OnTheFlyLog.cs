using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class OnTheFlyLog : IDataItem
    {
        public virtual int OnTheFlyLogID { get; set; }
        public virtual Guid LogGuid { get; set; }
        public virtual DateTime LogTimeStamp { get; set; }
        public virtual String ActionName { get; set; }
        public virtual String ActionData { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual String IPAddress { get; set; }
    }
}