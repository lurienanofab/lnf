using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Default
{
    public static class IOC
    {
        static IOC()
        {
            var reg = new Registry();
            reg.For<ISessionManager>().Singleton().Use<SessionManager<ThreadStaticSessionContext>>();
            reg.For<IContext>().Singleton().Use<DefaultContext>();

            Resolver = new DependencyResolver(reg);
        }

        public static IDependencyResolver Resolver { get; }
    }
}
