using LNF.Impl.Context;
using Moq;
using System.Web;

namespace LNF.Tests
{
    public class ContextFactory : IHttpContextFactory
    {
        public HttpContextBase Create()
        {
            var mock = new Mock<HttpContextBase>();
            return mock.Object;
        }
    }
}
