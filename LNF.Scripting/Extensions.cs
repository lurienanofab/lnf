using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Scripting
{
    public static class Extensions
    {
        public static IronPython.Runtime.List ToPythonList<T>(this IEnumerable<T> collection)
        {
            IronPython.Runtime.List result = new IronPython.Runtime.List();
            collection.Select(x => { result.Add(x); return x; }).ToList();
            return result;
        }
    }
}
