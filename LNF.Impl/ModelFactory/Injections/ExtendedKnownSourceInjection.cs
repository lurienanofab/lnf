using Omu.ValueInjecter.Injections;
using System;
using System.Linq;

namespace LNF.Impl.ModelFactory.Injections
{
    public abstract class ExtendedKnownSourceInjection<T> : IValueInjection
    {
        public object Map(object source, object target)
        {
            Inject(source, target);
            return target;
        }

        protected void SetTargetProperty(object target, string targetPropertyName, T obj, Func<T, object> value)
        {
            var tp = target.GetType().GetProperty(targetPropertyName);
            if (tp != null)
            {
                // A function is passed so that getting the value is delayed until after we know the target property exists.
                object val = value(obj);

                bool canAssign = tp.PropertyType.IsAssignableFrom(val.GetType());

                if (canAssign)
                {    
                    tp.SetValue(target, val);
                }
            }
        }

        protected abstract void SetTarget(object target, T obj);

        protected void Inject(object source, object target)
        {
            Type sourceType = source.GetType();

            // first check if source is T
            if (typeof(T).IsAssignableFrom(sourceType))
                SetTarget(target, (T)source);

            // then check for a property of source that is T
            var prop = sourceType.GetProperties().FirstOrDefault(x => typeof(T).IsAssignableFrom(x.PropertyType));

            if (prop != null)
            {
                T value = (T)prop.GetValue(source);
                if (value != null)
                {
                    SetTarget(target, value);
                }
            }
        }
    }
}
