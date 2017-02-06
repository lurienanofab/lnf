using LNF.Repository.Data;
using System;

namespace LNF.Repository.Inventory
{
    public class PrivateChemical : IDataItem
    {
        public virtual int PrivateChemicalID { get; set; }
        public virtual Client RequestedByClient { get; set; }
        public virtual int ApprovedByClientID { get; set; }
        public virtual DateTime? ApprovedDate { get; set; }
        public virtual string ChemicalName { get; set; }
        public virtual string Notes { get; set; }
        public virtual string MsdsUrl { get; set; }
        public virtual bool Restricted { get; set; }
        public virtual bool Shared { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
