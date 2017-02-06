using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LNF.Logging;

namespace LNF
{
    public interface ILogProvider : ITypeProvider
    {
        ILog Current {get;}
        string Name { get; set; }
        bool Enabled { get; set; }
    }

    public interface ILog : IEnumerable<LogMessage>
    {
        void Write(LogMessage message);
        int Purge(DateTime cutoff);
    }
}
