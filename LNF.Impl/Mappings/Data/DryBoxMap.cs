using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class DryBoxMap : ClassMap<DryBox>
    {
        public DryBoxMap()
        {
            Id(x => x.DryBoxID);
            Map(x => x.DryBoxName);
            Map(x => x.Visible);
            Map(x => x.Active);
            Map(x => x.Deleted);
        }
    }
}
