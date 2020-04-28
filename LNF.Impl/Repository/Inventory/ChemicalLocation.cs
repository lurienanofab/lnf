using LNF.DataAccess;

namespace LNF.Impl.Repository.Inventory
{
    public class ChemicalLocation : IDataItem
    {
        public virtual int ChemicalLocationID { get; set; }
        public virtual PrivateChemical PrivateChemical { get; set; }
        public virtual LabelLocation LabelLocation { get; set; }
    }
}
