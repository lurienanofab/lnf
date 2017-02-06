using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class ModTypeMap: ClassMap<ModType>
    {
        public ModTypeMap()
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
