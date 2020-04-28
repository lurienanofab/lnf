using System;
using System.Linq;

namespace LNF.Impl.DataAccess.ModelFactory.Injections
{
    public class NullableUseDefaultInjection : NullableInjection
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
                    // set the target value to default only if the source property is nullable (and is null) and the target property is not nullable
                    var underlyingType = Nullable.GetUnderlyingType(sp.PropertyType);
                    if (CanAssign(underlyingType, tp) && !IsNullable(tp))
                    {
                        object value = sp.GetValue(source);
                        if (value == null)
                        {
                            value = Activator.CreateInstance(underlyingType);
                        }
                        tp.SetValue(target, value);
                    }
                }
            }
        }
    }
}
