using LNF.Impl.DataAccess;
using LNF.Logging;
using System;

namespace LNF.Impl.Logging
{
    public class ConsoleLoggingService : LoggingServiceBase
    {
        public ConsoleLoggingService(ISessionManager mgr) : base(mgr) { }

        protected override void AddMessage(LogMessage message)
        {
            Console.WriteLine(FormatMessage(message));
        }
    }
}
