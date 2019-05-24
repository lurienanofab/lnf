using LNF.Impl.Context;
using System.Web;

namespace LNF.Impl.Testing
{
    public class TestContextFactory : IHttpContextFactory
    {
        private readonly HttpContextBase _context;

        public TestContextFactory(HttpContextBase context)
        {
            _context = context;
        }

        public HttpContextBase CreateContext() => _context;
    }
}
