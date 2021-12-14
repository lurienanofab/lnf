using SimpleInjector.Advanced;
using System;
using System.Linq;
using System.Reflection;

namespace LNF.Impl.DependencyInjection
{
    // for more information see:
    // https://simpleinjector.readthedocs.io/en/latest/advanced.html#property-injection

    public class InjectPropertySelectionBehavior : IPropertySelectionBehavior
    {
        public bool SelectProperty(Type implementationType, PropertyInfo prop)
        {
            var result = prop.GetCustomAttributes(typeof(InjectAttribute)).Any();
            return result;
        }
    }
}
