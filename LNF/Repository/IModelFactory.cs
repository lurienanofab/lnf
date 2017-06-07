using LNF.Repository;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository
{
    public interface IModelFactory : ITypeProvider
    {
        T Create<T>(object source);
    }

    public static class ModelFactoryExtensions
    {
        public static T Model<T>(this IDataItem item)
        {
            // need to unproxy otherwise the ModelFactory won't recognize the type
            object unproxy = DA.Current.Unproxy(item);
            return Providers.ModelFactory.Create<T>(unproxy);
        }

        public static IList<T> Model<T>(this IEnumerable<IDataItem> items)
        {
            return items.Select(x => x.Model<T>()).ToList();
        }
    }
}
