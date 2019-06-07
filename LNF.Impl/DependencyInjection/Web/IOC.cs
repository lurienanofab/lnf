using LNF.Impl.Context;
using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Web
{
    public class IOC
    {
        public IDependencyResolver Resolver { get; }

        public IOC()
        {
            var reg = new Registry();
            var fac = new WebContextFactory();
            var ctx = new WebContext(fac);

            reg.For<Models.IContext>().Singleton().Use(ctx);
            reg.For<ISessionManager>().Singleton().Use(SessionManager<WebSessionContext>.Current);
            reg.For<IDataAccessService>().Singleton().Use<NHibernateDataAccess<WebSessionContext>>();

            Resolver = new StructureMapResolver(reg);
        }
    }
}
