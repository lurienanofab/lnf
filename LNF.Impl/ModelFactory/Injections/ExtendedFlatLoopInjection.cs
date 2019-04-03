using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;
using System;
using System.Linq;

namespace LNF.Impl.ModelFactory.Injections
{
    /// <summary>
    /// Handles normal FlatLoopInjections: target.AccountActive <==> source.Account.Active
    /// Handles target properties that match a source sub property: target.ResourceID <==> source.Resource.ResourceID
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

            var props = sourceType.GetProperties().Where(x => x.PropertyType.IsClass && !x.PropertyType.FullName.StartsWith("System."));

            foreach (var prop in props)
            {
                // value is some class like Resource, Activity, Client, Account, etc.
                object value = prop.GetValue(source);

                // this will cover all target properties that match value properties, for example target.ResourceID <==> source.Resource.ResourceID
                if (value != null)
                    target.InjectFrom(value);
            }

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
