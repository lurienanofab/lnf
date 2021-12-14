using LNF.DataAccess;
using LNF.DependencyInjection;
using LNF.Impl.Context;
using LNF.Impl.DataAccess;
using NHibernate.Context;

namespace LNF.Impl.DependencyInjection
{
    public class WebContainerConfiguration : ContainerConfiguration
    {
        public WebContainerConfiguration(IContainerContext context) : base(context) { }

        public override void RegisterContext() => Context.RegisterSingleton<IContext, WebContext>();
        public override void RegisterSessionManager() => Context.RegisterSingleton(() => SessionManager<WebSessionContext>.Current);
        public override void RegisterDataAccessService() => Context.RegisterSingleton<IDataAccessService, NHibernateDataAccess<WebSessionContext>>();
    }
}
