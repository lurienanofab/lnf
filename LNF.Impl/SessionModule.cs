using LNF.Repository;
using System;
using System.Diagnostics;
using System.Web;

namespace LNF.Impl
{
    public class SessionModule : IHttpModule
    {
        private IUnitOfWork _uow;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OpenNHibernateSession);
            context.EndRequest += new EventHandler(CloseNHibernateSession);
        }

        private void OpenNHibernateSession(object sender, EventArgs e)
        {
            if (!Providers.IsProduction())
                Debug.WriteLine("Beginning request for {0}", HttpContext.Current.Request.Url);
            _uow = Providers.DataAccess.StartUnitOfWork();
        }

        private void CloseNHibernateSession(object sender, EventArgs e)
        {
            if (!Providers.IsProduction())
                Debug.WriteLine("Ending request for {0}", HttpContext.Current.Request.Url);
            _uow.Dispose();
        }

        public void Dispose()
        {
            // nothing to do here...
        }
    }
}
