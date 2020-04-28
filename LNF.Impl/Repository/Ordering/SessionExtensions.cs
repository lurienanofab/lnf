using LNF.Ordering;
using NHibernate;

namespace LNF.Impl.Repository.Ordering
{
    public static class SessionExtensions
    {
        public static Status Draft(this ISession session)
        {
            return session.Get<Status>((int)OrderStatus.Draft);
        }

        public static Status AwaitingApproval(this ISession session)
        {
            return session.Get<Status>((int)OrderStatus.AwaitingApproval);
        }

        public static Status Approved(this ISession session)
        {
            return session.Get<Status>((int)OrderStatus.Approved);
        }

        public static Status Ordered(this ISession session)
        {
            return session.Get<Status>((int)OrderStatus.Ordered);
        }

        public static Status Completed(this ISession session)
        {
            return session.Get<Status>((int)OrderStatus.Completed);
        }

        public static Status Cancelled(this ISession session)
        {
            return session.Get<Status>((int)OrderStatus.Cancelled);
        }

        public static Status ProcessedManually(this ISession session)
        {
            return session.Get<Status>((int)OrderStatus.ProcessedManually);
        }
    }
}
