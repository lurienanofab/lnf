using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class PropertyMap:ClassMap<Property>
    {
        public PropertyMap()
        {
            Schema("sselControl.dbo");
            Table("GlobalProperties");
            Id(x => x.PropertyID);
            Map(x => x.PropertyName);
            Map(x => x.PropertyValue);
        }
    }
}
