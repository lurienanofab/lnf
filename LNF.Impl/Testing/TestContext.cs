using LNF.Impl.Context;
using System;
using System.Web;

namespace LNF.Impl.Testing
{
    public class TestContext : WebContext
    {
        public TestContext(HttpContextBase context) : base(new TestContextFactory(context)) { }
    }
}
