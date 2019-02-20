﻿using LNF.Impl.DependencyInjection.Web;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LNF.Impl
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class ServiceMiddleware
    {
        private AppFunc _next;

        public ServiceMiddleware(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            using (DA.StartUnitOfWork())
                await _next.Invoke(environment);
        }
    }
}
