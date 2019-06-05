using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Web
{
    public class IOC : IIOC
    {
        public IDependencyResolver Resolver { get; }

        public IOC(Models.IContext ctx)
        {
            var reg = new Registry();

            reg.For<Models.IContext>().Singleton().Use(ctx);
            reg.For<ISessionManager>().Singleton().Use<SessionManager<WebSessionContext>>();

            Resolver = new StructureMapResolver(reg);
        }
    }
}
