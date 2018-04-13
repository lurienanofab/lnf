using LNF.Impl.Context;
using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Web
{
    public static class IOC
    {
        static IOC()
        {
            var reg = new Registry();
            reg.For<ISessionManager>().Singleton().Use<SessionManager<WebSessionContext>>();
            reg.For<IContext>().Singleton().Use<WebContext>();

            Resolver = new DependencyResolver(reg);
        }

        public static IDependencyResolver Resolver { get; }
    }
}
