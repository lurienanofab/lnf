using LNF.DataAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LNF.Impl.DataAccess
{
    public class NHibernateMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly ISessionManager _sessionManager;

        public NHibernateMiddleware(Func<IDictionary<string, object>, Task> next, ISessionManager sessionManager)
        {
            _next = next;
            _sessionManager = sessionManager;  
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            using (IUnitOfWork uow = new NHibernateUnitOfWork(_sessionManager))
            {
                await _next.Invoke(environment);
            }
        }
    }
}
