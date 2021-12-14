using LNF.Logging;
using RestSharp;
using System;

namespace OnlineServices.Api.Logging
{
    public class LoggingService : ApiClient, ILoggingService
    {
        internal LoggingService(IRestClient rc) : base(rc) { }

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
