using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class ChemicalLocationMap : ClassMap<ChemicalLocation>
    {
        internal ChemicalLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ChemicalLocationID);
            References(x => x.PrivateChemical);
            References(x => x.LabelLocation);
        }
    }
}
