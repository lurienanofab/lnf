using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Logging
{
    public interface ILogMessage
    {
        string Name { get; }
        void Write();
    }
}
