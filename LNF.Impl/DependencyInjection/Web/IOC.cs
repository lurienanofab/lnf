﻿using LNF.Impl.Context;
using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Web
{
    public class IOC
    {
        public IDependencyResolver Resolver { get; }

        public IOC()
        {
            var ctx = new WebContext(new CurrentContextFactory());
            var reg = new Registry();
            reg.For<ISessionManager>().Singleton().Use<SessionManager<WebSessionContext>>();
            reg.For<IContext>().Singleton().Use(ctx);

            Resolver = new DependencyResolver(reg);
        }
    }
}
