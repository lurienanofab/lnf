using LNF.DataAccess;

namespace LNF.Repository
{
    public static class SessionExtensions
    {
        public static void Insert(this ISession repo, IDataItem item)
        {
            repo.Insert(new[] { item });
        }

        public static void Delete(this ISession repo, IDataItem item)
        {
            repo.Delete(new[] { item });
        }
    }
}
