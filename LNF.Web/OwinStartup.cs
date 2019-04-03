using LNF.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Routing;

namespace LNF.Web
{
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
            ConfigureRoutes(RouteTable.Routes);
        }

        public abstract void ConfigureRoutes(RouteCollection routes);

        public virtual void ConfigureDataContext(IAppBuilder app)
        {
            app.UseDataAccess();
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
                    LoginPath = new PathString(ServiceProvider.Current.Context.LoginUrl),
                    Provider = new CookieAuthenticationProvider()
                    {
                        OnApplyRedirect = context =>
                        {
                            context.RedirectUri = ServiceProvider.Current.Context.LoginUrl + new QueryString(context.Options.ReturnUrlParameter, context.Request.Uri.PathAndQuery);
                            context.Response.Redirect(context.RedirectUri);
                        }
                    }
                });
            }
        }
    }

    public class DataAccessMiddleware : OwinMiddleware
    {
        public DataAccessMiddleware(OwinMiddleware next) : base(next) { }

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
                uow = DA.StartUnitOfWork();
            }

            await Next.Invoke(context);

            if (uow != null)
                uow.Dispose();
        }
    }


    public static class DataAccessMiddlewareExtensions
    {
        public static void UseDataAccess(this IAppBuilder app)
        {
            if (ServiceProvider.Current.DataAccess != null)
            {
                app.Use(typeof(DataAccessMiddleware));
            }
        }
    }
}