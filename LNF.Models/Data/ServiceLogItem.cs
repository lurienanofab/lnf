using System;

namespace LNF.Models.Data
{
    public class ServiceLogItem
    {
        public virtual int ServiceLogID { get; set; }
        public virtual Guid? MessageID { get; set; }
        public virtual string ServiceName { get; set; }
        public virtual DateTime LogDateTime { get; set; }
        public virtual string LogSubject { get; set; }
        public virtual string LogLevel { get; set; }
        public virtual string LogMessage { get; set; }
        public virtual string Data { get; set; }
    }
}
