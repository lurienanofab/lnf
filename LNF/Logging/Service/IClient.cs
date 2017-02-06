using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Logging.Service
{
    public interface IClient
    {
        ServiceResponse Add(ServiceLogMessage msg);
    }
}
