using Omu.ValueInjecter.Injections;
using System;
using System.Linq;
using System.Reflection;

namespace LNF.Impl.DataAccess.ModelFactory.Injections
{
    /// <summary>
    /// Handles mapping of nullable and non-nullable properties where the propety names and underlying types match.
    /// </summary>
    public class NullableInjection : ValueInjection
    {
        protected override void Inject(object source, object target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();

            // get the nullable properties from the source
            var nullableProps = sourceType.GetProperties().Where(IsNullable);

            foreach (var sp in nullableProps)
            {
                // get a matching target prop
                var tp = targetType.GetProperty(sp.Name);
                if (tp != null)
                {
                    var underlyingType = Nullable.GetUnderlyingType(sp.PropertyType);
                    if (CanAssign(underlyingType, tp))
                    {
                        // only set if the nullable has a value (assume the target propety current value is the default value)
                        object value = sp.GetValue(source);
                        if (value != null)
                            tp.SetValue(target, value);
                    }
                }
            }

            // now do the same for the target
            nullableProps = targetType.GetProperties().Where(IsNullable);

            foreach (var tp in nullableProps)
            {
                // get a matching source prop
                var sp = sourceType.GetProperty(tp.Name);
                if (sp != null)
                {
                    var underlyingType = Nullable.GetUnderlyingType(tp.PropertyType);
                    if (CanAssign(underlyingType, sp))
                    {
                        object value = sp.GetValue(source);
                        tp.SetValue(target, value);
                    }
                }
            }
        }

        protected bool IsNullable(PropertyInfo p)
        {
            return p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        protected bool CanAssign(Type type, PropertyInfo p)
        {
            return type == p.PropertyType || p.PropertyType.IsAssignableFrom(type);
        }
    }
}
