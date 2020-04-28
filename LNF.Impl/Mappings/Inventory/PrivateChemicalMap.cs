using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class PrivateChemicalMap : ClassMap<PrivateChemical>
    {
        internal PrivateChemicalMap()
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
