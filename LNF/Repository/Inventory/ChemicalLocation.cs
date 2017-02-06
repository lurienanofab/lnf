using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Repository.Inventory
{
    public class ChemicalLocation : IDataItem
    {
        public virtual int ChemicalLocationID { get; set; }
        public virtual PrivateChemical PrivateChemical { get; set; }
        public virtual LabelLocation LabelLocation { get; set; }
    }
}
