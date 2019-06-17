using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;
using System;

namespace LNF.Impl.ModelFactory.Injections
{
    /// <summary>
    /// Handles normal FlatLoopInjections: target.AccountActive <==> source.Account.Active
    /// Handles target properties that match the sub property type name plus property name: target.AccountActive <==> source.Active (when type of source is Account)
    /// </summary>
    public class ExtendedFlatLoopInjection : ValueInjection
    {
        protected override void Inject(object source, object target)
        {
            if (source == null) return;

            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            // this will cover target properties that conform to the standard flattened name structure, for example target.AccountActive <==> source.Account.Active
            target.InjectFrom(new FlatLoopInjection(), source);

            // Cannot check for target.ResourceID <==> source.Resource.ResourceID because this causes issues when there 
            // is a matching property and we do not want the target to be set because they mean different things.
            // There is no way to differentiate when we want the property to be set and when we don't. For example:
            // target.ResourceID <==> source.Resource.ResourceID vs target.IsActive vs source.Resource.IsActive
            // when target is Reservation or ReservationRecurrence. This must be handled specially by ModelBuilders
            // unfortunately because there is no convention that always works.

            // check for target property names like AccountActive when the source is an Account
            var sourceTypeName = sourceType.Name;
            
            foreach (var sp in sourceType.GetProperties())
            {
                var targetPropertyName = sourceTypeName + sp.Name;
                var tp = targetType.GetProperty(targetPropertyName);
                if (tp != null && tp.PropertyType.IsAssignableFrom(sp.PropertyType))
                {
                    object value = sp.GetValue(source);
                    tp.SetValue(target, value);
                }
            }
        }
    }
}
