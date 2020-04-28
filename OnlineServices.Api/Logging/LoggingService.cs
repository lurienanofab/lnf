using LNF.Logging;
using System;

namespace OnlineServices.Api.Logging
{
    public class LoggingService : ILoggingService
    {
        public string Name => throw new NotImplementedException();

        public bool Enabled => throw new NotImplementedException();

        public IPassError GetPassError(Guid errorId)
        {
            throw new NotImplementedException();
        }

        public int Purge(DateTime cutoff)
        {
            throw new NotImplementedException();
        }

        public Guid SetPassError(string errorMsg, int clientId, string clientName, string filePath)
        {
            throw new NotImplementedException();
        }

        public void Write(LogMessage message)
        {
            throw new NotImplementedException();
        }

        public void WriteToSystemLog(int clientId, Guid messageGuid, LogMessageTypes messageType, string message)
        {
            throw new NotImplementedException();
        }
    }
}
