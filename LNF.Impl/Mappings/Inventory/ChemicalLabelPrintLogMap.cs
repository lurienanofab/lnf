using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class ChemicalLabelPrintLogMap : ClassMap<ChemicalLabelPrintLog>
    {
        internal ChemicalLabelPrintLogMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ChemicalLabelPrintLogID);
            References(x => x.ChemicalLocation);
            References(x => x.Client);
            Map(x => x.PrintDateTime);
            Map(x => x.IPAddress);

        }
    }
}

