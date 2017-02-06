using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class ChemicalLocationMap : ClassMap<ChemicalLocation>
    {
        public ChemicalLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ChemicalLocationID);
            References(x => x.PrivateChemical);
            References(x => x.LabelLocation);
        }
    }
}
