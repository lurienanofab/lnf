using LNF.Impl.Context;
using LNF.Impl.DataAccess;
using System.Web;

namespace LNF.Impl.Testing
{
    public class TestContext : WebContext
    {
        public TestContext(HttpContextBase context, ISessionManager mgr) : base(new TestContextFactory(context), mgr) { }
    }
}
