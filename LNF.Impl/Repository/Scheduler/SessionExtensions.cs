using LNF.Scheduler;
using NHibernate;
using System.Linq;

namespace LNF.Impl.Repository.Scheduler
{
    public static class SessionExtensions
    {

        public static IQueryable<ResourceClientInfo> SelectResourceClients(this ISession session, int resourceId = 0, int clientId = 0, ClientAuthLevel authLevel = 0)
        {
            var query = session.Query<ResourceClientInfo>();

            if (resourceId > 0)
                query = query.Where(x => x.ResourceID == resourceId);

            if (clientId > 0)
                query = query.Where(x => x.ClientID == clientId || x.ClientID == -1);

            if (authLevel > 0)
                query = query.Where(x => (x.AuthLevel & authLevel) > 0);

            return query;
        }

        public static IQueryable<ResourceClientInfo> SelectEmailClients(this ISession session, int resourceId)
        {
            return session.SelectResourceClients(resourceId).Where(x => x.EmailNotify != null);
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnCancelClients(this ISession session, int resourceId)
        {
            return session.SelectEmailClients(resourceId).Where(x => x.AuthLevel > ClientAuthLevel.UnauthorizedUser && x.EmailNotify.Value == 1);
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnOpeningClients(this ISession session, int resourceId)
        {
            return session.SelectEmailClients(resourceId).Where(x => x.AuthLevel > ClientAuthLevel.UnauthorizedUser && x.EmailNotify.Value == 2);
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnPracticeRes(this ISession session, int resourceId)
        {
            return session.SelectEmailClients(resourceId).Where(x => x.AuthLevel == ClientAuthLevel.ToolEngineer && (x.PracticeResEmailNotify != null && x.PracticeResEmailNotify.Value == 1));
        }
    }
}
