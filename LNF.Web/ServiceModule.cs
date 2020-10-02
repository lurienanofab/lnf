using LNF.Impl;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using System;
using System.Web;

namespace LNF.Web
{
    public class ServiceModule : IHttpModule
    {
        private IDisposable _uow;
        private IProvider _provider;

        public void Init(HttpApplication app)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            var config = new WebContainerConfiguration(container);
            config.Configure();

            _provider = container.GetInstance<IProvider>();
            ServiceProvider.Setup(_provider);

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
