using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    internal class ControlActionMap : ClassMap<ControlAction>
    {
        internal ControlActionMap()
        {
            Schema("sselControl.dbo");
            Table("Action");
            Id(x => x.ActionID);
            Map(x => x.ActionName);
            Map(x => x.ActionTableName);
        }
    }
}
