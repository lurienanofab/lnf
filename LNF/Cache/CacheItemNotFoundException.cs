using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Cache
{
    public class CacheItemNotFoundException : Exception
    {
        public override string Message { get; }

        public CacheItemNotFoundException(string name, string id, int value)
        {
            Message = string.Format("Unable to find {0} with {1} = {2} in cache.", name, id, value);
        }
    }
}
