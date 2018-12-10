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
            object unproxy = DA.Current.Unproxy(item);
            return ServiceProvider.Current.ModelFactory.Create<T>(unproxy);
        }

        public static IList<T> Model<T>(this IEnumerable<IDataItem> items)
        {
            var list = items.ToList();
            return list.Select(x => x.Model<T>()).ToList();
        }
    }
}
