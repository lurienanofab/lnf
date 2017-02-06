using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.PhysicalAccess
{
    public static class Extensions
    {
        public static bool IsActive(this Badge badge)
        {
            if (badge != null)
                return badge.ExpireDate > DateTime.Now;
            else
                return false;
        }
    }
}
