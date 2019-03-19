using LNF.Impl.DependencyInjection.Web;
using LNF.Repository;
using System;
using System.Web;

namespace LNF.Impl
{
    public class ServiceModule : IHttpModule
    {
        private IDisposable _uow;

        public void Init(HttpApplication app)
        {
            var ioc = new IOC();
            ServiceProvider.Current = ioc.Resolver.GetInstance<ServiceProvider>();
            app.BeginRequest += Context_BeginRequest;
            app.EndRequest += Context_EndRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            _uow = DA.StartUnitOfWork();
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            _uow.Dispose();
        }

        public void Dispose()
        {
            // nothing to do here...
        }
    }
}
