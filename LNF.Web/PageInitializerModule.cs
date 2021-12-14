using LNF.Impl.DependencyInjection;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Web;
using System.Web.UI;

namespace LNF.Web
{
    public abstract class PageInitializerModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += (sender, e) =>
            {
                IHttpHandler handler = context.Context.CurrentHandler;
                if (handler != null)
                {
                    var name = handler.GetType().Assembly.FullName;
                    if (!name.StartsWith("System.Web") && !name.StartsWith("Microsoft"))
                    {
                        InitializeHandler(handler);
                    }
                }
            };
        }

        public static void RegisterModule(Type pageInitializerType)
        {
            DynamicModuleUtility.RegisterModule(pageInitializerType);
        }

        // This should be overridden and call a static method in Global.asax
        // (see https://docs.simpleinjector.org/en/latest/webformsintegration.html)
        protected virtual void InitializeHandler(IHttpHandler handler)
        {
            var handlerType = handler is Page ? handler.GetType().BaseType : handler.GetType();
            var container = ContainerContextFactory.Current.GetContainer();
            container.GetRegistration(handlerType, true).Registration.InitializeInstance(handler);
        }

        public void Dispose() { }
    }
}
