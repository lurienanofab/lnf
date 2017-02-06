using LNF.Repository.Data;
using System;

namespace LNF.Repository.Inventory
{
    public class LabelQueue : IDataItem
    {
        public virtual int LabelQueueID { get; set; }
        public virtual int? ItemID { get; set; }
        public virtual int? PrivateChemicalID { get; set; }
        public virtual Client PrintedBy { get; set; }
        public virtual DateTime PrintedDate { get; set; }
        public virtual string LabelType { get; set; }
        public virtual int LabelLocationID { get; set; }
    }
}
