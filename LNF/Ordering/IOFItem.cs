using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Ordering
{
    public class IOFItem
    {
        public int PODID { get; set; }
        public int POID { get; set; }
        public int ItemID { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public string Description { get; set; }
        public string PartNum { get; set; }
        public int CatID { get; set; }
        public string CatName { get; set; }
        public int ParentID { get; set; }
        public string CatNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Price { get { return Quantity * UnitPrice; } }
        public bool IsNotes { get; set; }
    }
}
