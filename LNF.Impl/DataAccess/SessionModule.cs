using LNF.Impl.DependencyInjection.Web;
using LNF.Repository;
using System;
using System.Diagnostics;
using System.Web;

namespace LNF.Impl.DataAccess
{
    public class SessionModule : IHttpModule
    {
        private IDisposable _uow;

        public void Init(HttpApplication context)
        {
            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();
            context.BeginRequest += new EventHandler(OpenNHibernateSession);
            context.EndRequest += new EventHandler(CloseNHibernateSession);
        }

        private void OpenNHibernateSession(object sender, EventArgs e)
        {
            if (!ServiceProvider.Current.IsProduction())
                Debug.WriteLine("Beginning request for {0}", HttpContext.Current.Request.Url);

            _uow = DA.StartUnitOfWork();
        }

        private void CloseNHibernateSession(object sender, EventArgs e)
        {
            if (!ServiceProvider.Current.IsProduction())
                Debug.WriteLine("Ending request for {0}", HttpContext.Current.Request.Url);
            _uow.Dispose();
        }

        public void Dispose()
        {
            // nothing to do here...
        }
    }
}
