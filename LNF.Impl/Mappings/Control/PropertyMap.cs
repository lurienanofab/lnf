using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class PropertyMap:ClassMap<Property>
    {
        internal PropertyMap()
        {
            Schema("sselControl.dbo");
            Table("GlobalProperties");
            Id(x => x.PropertyID);
            Map(x => x.PropertyName);
            Map(x => x.PropertyValue);
        }
    }
}
