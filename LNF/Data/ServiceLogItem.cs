using System;

namespace LNF.Data
{
    public class ServiceLogItem : IServiceLog
    {
        public int ServiceLogID { get; set; }
        public Guid? MessageID { get; set; }
        public string ServiceName { get; set; }
        public DateTime LogDateTime { get; set; }
        public string LogSubject { get; set; }
        public string LogLevel { get; set; }
        public string LogMessage { get; set; }
        public string Data { get; set; }
    }
}
