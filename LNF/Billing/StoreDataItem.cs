using System;

namespace LNF.Billing
{
    public class StoreDataItem
    {
        public int StoreDataID { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int ItemID { get; set; }
        public DateTime OrderDate { get; set; }
        public int AccountID { get; set; }
        public double Quantity { get; set; }
        public double UnitCost { get; set; }
        public int CategoryID { get; set; }
        public DateTime StatusChangeDate { get; set; }
    }
}
