using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LNF.Logging;

namespace LNF
{
    public interface ILogService
    {
        ILog Current {get;}
        string Name { get; }
        bool Enabled { get; }
    }

    public interface ILog : IEnumerable<LogMessage>
    {
        void Write(LogMessage message);
        int Purge(DateTime cutoff);
    }
}
