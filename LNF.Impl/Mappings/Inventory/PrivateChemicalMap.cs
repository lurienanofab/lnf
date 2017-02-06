using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class PrivateChemicalMap : ClassMap<PrivateChemical>
    {
        public PrivateChemicalMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.PrivateChemicalID);
            References(x => x.RequestedByClient);
            Map(x => x.ApprovedByClientID);
            Map(x => x.ApprovedDate);
            Map(x => x.ChemicalName);
            Map(x => x.Notes);
            Map(x => x.MsdsUrl, "MSDS_Url");
            Map(x => x.Restricted);
            Map(x => x.Shared);
            Map(x => x.Deleted);
        }
    }
}
