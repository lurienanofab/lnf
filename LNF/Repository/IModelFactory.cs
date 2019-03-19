using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository
{
    public interface IModelFactory
    {
        T Create<T>(object source);
    }

    public static class ModelFactoryExtensions
    {
        public static T Model<T>(this IDataItem item)
        {
            // need to unproxy otherwise the ModelFactory won't recognize the type
            object obj = DA.Current.Unproxy(item);
            return ServiceProvider.Current.ModelFactory.Create<T>(obj);
        }

        public static IList<T> Model<T>(this IEnumerable<IDataItem> items)
        {
            var list = items.ToList();
            return list.Select(x => x.Model<T>()).ToList();
        }

        public static T CreateModel<T>(this IDataItem item)
        {
            if (item == null) return default(T);
            var list = new List<IDataItem> { item };
            return list.AsQueryable().CreateModels<T>().FirstOrDefault();
        }

        public static IEnumerable<T> CreateModels<T>(this IQueryable<IDataItem> query)
        {
            if (query == null) return null;
            return query.Select(x => ServiceProvider.Current.ModelFactory.Create<T>(x)).ToList();
        }
    }
}
