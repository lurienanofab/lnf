using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF
{
    public interface IContextProvider : ITypeProvider
    {
        IContext Current { get; }
        string LoginUrl { get; set; }
    }
}
