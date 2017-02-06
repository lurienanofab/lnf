using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Logging;

namespace LNF.Impl.Logging
{
    public class ConsoleLogProvider : LogProviderBase
    {
        protected override LogBase NewLog()
        {
            return new ConsoleLog();
        }
    }

    public class ConsoleLog : LogBase
    {
        protected override void OnWrite(LogMessage message)
        {
            Console.WriteLine("{0}> [{1}:{2}] {3}", message.Level, Providers.Log.Name, message.Subject, message.Body);
        }

        protected override int OnPurge(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }
}
