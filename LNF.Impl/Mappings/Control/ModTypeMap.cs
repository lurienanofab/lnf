using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class ModTypeMap: ClassMap<ModType>
    {
        internal ModTypeMap()
        {
            Schema("sselControl.dbo");
            Id(x => x.ModTypeID);
            Map(x => x.WagoType);
            Map(x => x.Direction);
            Map(x => x.NumPoints);
            Map(x => x.PointSize);
            Map(x => x.Special);
            Map(x => x.Description);
        }
    }
}
