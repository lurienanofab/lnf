using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Default
{
    public class IOC : IIOC
    {
        public IDependencyResolver Resolver { get; }

        public IOC(IContext ctx)
        {
            var reg = new Registry();

            reg.For<IContext>().Singleton().Use(ctx);
            reg.For<ISessionManager>().Singleton().Use<SessionManager<ThreadStaticSessionContext>>();

            Resolver = new StructureMapResolver(reg);
        }
    }
}
