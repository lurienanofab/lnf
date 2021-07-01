using LNF.Impl;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace LNF.Web
{
    public class ServiceModule : IHttpModule
    {
        private IDisposable _uow;
        private IProvider _provider;

        public void Init(HttpApplication app)
        {
            Assembly[] assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();

            var webapp = new WebApp();

            var wcc = new WebContainerConfiguration(webapp.Container);
            wcc.EnablePropertyInjection();
            wcc.RegisterAllTypes();

            webapp.Bootstrap(assemblies);

            _provider = webapp.GetInstance<IProvider>();

            app.BeginRequest += App_BeginRequest;
            app.EndRequest += App_EndRequest;
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
