﻿using LNF.Impl.Context;
using LNF.Impl.DataAccess;
using NHibernate.Context;
using StructureMap;

namespace LNF.Impl.DependencyInjection.Default
{
    public class IOC : IIOC
    {
        public IDependencyResolver Resolver { get; }

        public IOC(IHttpContextFactory httpContextFactory)
        {
            var ctx = new WebContext(httpContextFactory);
            var reg = new Registry();
            reg.For<ISessionManager>().Singleton().Use<SessionManager<ThreadStaticSessionContext>>();
            reg.For<IContext>().Singleton().Use(ctx);

            Resolver = new DependencyResolver(reg);
        }
    }
}
