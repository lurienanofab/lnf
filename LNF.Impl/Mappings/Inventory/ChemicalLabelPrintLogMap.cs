using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class ChemicalLabelPrintLogMap : ClassMap<ChemicalLabelPrintLog>
    {
        public ChemicalLabelPrintLogMap()
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

