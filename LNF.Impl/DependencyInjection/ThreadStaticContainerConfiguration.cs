using LNF.DataAccess;
using LNF.DependencyInjection;
using LNF.Impl.Context;
using LNF.Impl.DataAccess;
using NHibernate.Context;

namespace LNF.Impl.DependencyInjection
{
    public class ThreadStaticContainerConfiguration : ContainerConfiguration
    {
        public ThreadStaticContainerConfiguration(IContainerContext context) : base(context) { }

        public override void RegisterContext() => Context.RegisterSingleton<IContext, DefaultContext>();
        public override void RegisterSessionManager() => Context.RegisterSingleton(() => SessionManager<ThreadStaticSessionContext>.Current);
        public override void RegisterDataAccessService() => Context.RegisterSingleton<IDataAccessService, NHibernateDataAccess<ThreadStaticSessionContext>>();
    }
}
