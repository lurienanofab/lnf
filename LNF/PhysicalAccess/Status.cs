using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.PhysicalAccess
{
    public enum zStatus
    {
        Unknown = 0,
        Active = 1,
        Disabled = 2,
        Lost = 3,
        Stolen = 4,
        Terminated = 5,
        Unaccounted = 6,
        Void = 7,
        Expired = 8,
        AutoDisable = 9
    }
}
