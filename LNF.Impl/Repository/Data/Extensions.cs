using LNF.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Repository.Data
{
    public static class Extensions
    {
        public static MenuItem GetMenuItem(this Menu item)
        {
            if (item == null) return null;
            return item.CreateModel<MenuItem>();
        }
    }
}
