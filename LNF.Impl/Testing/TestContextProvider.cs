using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LNF.Impl.Testing
{
    public class TestContextProvider : IContextProvider
    {
        private IContext _context;

        public TestContextProvider()
        {
            var username = ConfigurationManager.AppSettings["CurrentUserName"];
            var roles = ConfigurationManager.AppSettings["CurrentRoles"].Split('|').ToArray();
            _context = new TestContext(username, roles);
        }

        public IContext Current
        {
            get { return _context; }
        }

        public string LoginUrl { get; set; }
    }
}
