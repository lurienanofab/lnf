using LNF.DataAccess;
using LNF.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SimpleInjector;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace LNF.Web
{
    [Obsolete]
    public abstract class OwinStartup
    {
        public virtual bool UseCookieAuthentication
        {
            get { return false; }
        }

        public virtual void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureDataContext(app);
            ConfigureFilters(GlobalFilters.Filters);
            ConfigureRoutes(RouteTable.Routes);
        }

        public virtual void ConfigureRoutes(RouteCollection routes)
        {
            // do nothing unless overridden
        }

        public virtual void ConfigureFilters(GlobalFilterCollection filters)
        {
            // do nothing unless overridden
        }

        public virtual void ConfigureDataContext(IAppBuilder app)
        {
            // do nothing unless overridden
        }

        public virtual void ConfigureAuth(IAppBuilder app)
        {
            if (UseCookieAuthentication)
            {
                // Enable the application to use a cookie to store information for the signed in user
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    CookieName = "sselAuth.cookie",
                    CookieDomain = ".umich.edu",
                    CookiePath = "/",
                    ReturnUrlParameter = "ReturnUrl",
                    ExpireTimeSpan = TimeSpan.FromHours(8),
                    LoginPath = new PathString(ServiceProvider.Current.LoginUrl()),
                    Provider = new CookieAuthenticationProvider()
                    {
                        OnApplyRedirect = context =>
                        {
                            context.RedirectUri = ServiceProvider.Current.LoginUrl() + new QueryString(context.Options.ReturnUrlParameter, context.Request.Uri.PathAndQuery);
                            context.Response.Redirect(context.RedirectUri);
                        }
                    }
                });
            }
        }
    }

    public class DataAccessMiddleware : OwinMiddleware
    {
        private readonly Container _container;

        public DataAccessMiddleware(OwinMiddleware next, Container container) : base(next)
        {
            _container = container;
        }

        private static bool IsStaticContent(Uri uri)
        {
            string[] staticExtensions = { ".js", ".css", ".png", ".jpg", ".gif", ".ico", ".bmp" };
            if (uri == null) return false;
            string ext = Path.GetExtension(uri.LocalPath);
            return staticExtensions.Contains(ext);
        }

        public override async Task Invoke(IOwinContext context)
        {
            Uri uri = context.Request.Uri;

            IDisposable uow = null;

            if (!IsStaticContent(uri))
            {
                uow = _container.GetInstance<IDataAccessService>().StartUnitOfWork();
            }

            await Next.Invoke(context);

            if (uow != null)
                uow.Dispose();
        }
    }


    public static class DataAccessMiddlewareExtensions
    {
        public static void UseDataAccess(this IAppBuilder app, Container container)
        {
            if (container == null) throw new ArgumentNullException("container");
            app.Use(typeof(DataAccessMiddleware), container);
        }
    }
}