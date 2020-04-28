using System;

namespace LNF.DataAccess
{
    // This is an exact copy of NHiberante.CacheMode
    [Serializable, Flags]
    public enum CacheMode
    {
        Ignore = 0,
        Put = 1,
        Get = 2,
        Normal = 3,
        Refresh = 5
    }
}
