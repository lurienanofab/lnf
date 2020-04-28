using System;

namespace LNF.Data
{
    public interface IServiceLog
    {
        int ServiceLogID { get; set; }
        Guid? MessageID { get; set; }
        string ServiceName { get; set; }
        DateTime LogDateTime { get; set; }
        string LogSubject { get; set; }
        string LogLevel { get; set; }
        string LogMessage { get; set; }
        string Data { get; set; }
    }
}
