using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Default
{
    public class IOC
    {
        public IDependencyResolver Resolver { get; }

        public IOC()
        {
            var reg = new Registry();

            reg.For<Models.IContext>().Singleton().Use<DefaultContext>();
            reg.For<ISessionManager>().Singleton().Use(SessionManager<ThreadStaticSessionContext>.Current);
            reg.For<IDataAccessService>().Singleton().Use<NHibernateDataAccess<ThreadStaticSessionContext>>();

            Resolver = new StructureMapResolver(reg);
        }
    }
}
