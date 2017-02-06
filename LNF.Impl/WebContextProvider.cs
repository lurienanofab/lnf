using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LNF.Impl
{
    public class WebContextProvider : IContextProvider
    {
        private IContext _context;

        public string LoginUrl { get; set; }

        public virtual IContext Current
        {
            get
            {
                if (_context == null)
                    _context = new WebContext();
                return _context;
            }
        }

        public void Dispose()
        {

        }
    }
}
