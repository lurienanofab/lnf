using LNF.Impl;
using SimpleInjector;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace LNF.Web.Mvc
{
    public class MvcServiceModule : IHttpModule
    {
        private IDisposable _uow;
        private IProvider _provider;

        public void Init(HttpApplication app)
        {
            Assembly[] assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();

            var webapp = new WebApp();

            var wcc = new WebContainerConfiguration(webapp.Container);
            wcc.RegisterAllTypes();

            RegisterTypes(webapp.Container);

            webapp.BootstrapMvc(assemblies);

            _provider = webapp.GetInstance<IProvider>();

            app.BeginRequest += App_BeginRequest;
            app.EndRequest += App_EndRequest;
        }

        protected virtual void RegisterTypes(Container container)
        {
            // Override to register application specific types.
        }

        private void App_BeginRequest(object sender, EventArgs e)
        {
            _uow = _provider.DataAccess.StartUnitOfWork();
        }

        private void App_EndRequest(object sender, EventArgs e)
        {
            _uow.Dispose();
        }

        public void Dispose()
        {
            // nothing to do here...
        }
    }
}
