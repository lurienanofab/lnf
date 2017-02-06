using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Impl
{
    public class AppContextProvider : IContextProvider
    {
        public AppContextProvider()
        {
            Current = new AppContext();
        }

        public IContext Current { get; }

        public string LoginUrl { get; set; }
    }
}
