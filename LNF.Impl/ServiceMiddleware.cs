using LNF.DataAccess;
using LNF.Impl.DataAccess;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LNF.Impl
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    [Obsolete("Use LNF.Impl.DataAccess.NHibernateMiddleware instead.")]
    public class ServiceMiddleware
    {
        private AppFunc _next;

        public ServiceMiddleware(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            using (IUnitOfWork uow = new NHibernateUnitOfWork(SessionManager<WebSessionContext>.Current))
                await _next.Invoke(environment);
        }
    }
}
