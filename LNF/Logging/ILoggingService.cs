using System;

namespace LNF.Logging
{
    public interface ILoggingService
    {
        string Name { get; }
        void Write(LogMessage message);
        int Purge(DateTime cutoff);
        bool Enabled { get; }
        void WriteToSystemLog(int clientId, Guid messageGuid, LogMessageTypes messageType, string message);
        Guid SetPassError(string errorMsg, int clientId, string clientName, string filePath);
        IPassError GetPassError(Guid errorId);
    }
}
