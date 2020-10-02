using LNF.Repository;
using NHibernate;
using NHibernate.Transform;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Repository
{
    public static class NHibernateExtensions
    {
        public static void DeleteMany<T>(this ISession session, IEnumerable<T> items) where T : LNF.DataAccess.IDataItem
        {
            var list = items.ToList();
            foreach (T item in list)
            {
                session.Delete(item);
            }
        }

        public static IDataCommand Command(this ISession session, CommandType type = CommandType.StoredProcedure) => new NHibernateDataCommand(session, type);

        public static IQuery SetParameter(this IQuery query, string name, bool condition, object val)
        {
            if (condition)
                query.SetParameter(name, val);
            else
                query.SetParameter(name, null);

            return query;
        }

        public static IList<IDictionary> FillTable(this IQuery query)
        {
            return query.SetResultTransformer(Transformers.AliasToEntityMap).List<IDictionary>();
        }

        public static T Require<T>(this ISession session, object id)
        {
            var result = session.Get<T>(id);

            if (result == null)
                throw new ItemNotFoundException(typeof(T), id);

            return result;
        }
    }
}
