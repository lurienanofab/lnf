using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF
{
    public class DefaultContextProvider : IContextProvider
    {
        private IContext _context;

        public string LoginUrl { get; set; }

        public IContext Current
        {
            get
            {
                if (_context == null)
                    _context = CreateContext();

                return _context;
            }
        }

        protected virtual IContext CreateContext()
        {
            return new DefaultContext();
        }

        public void Dispose()
        {

        }
    }
}
