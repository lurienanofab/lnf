using System;

namespace LNF.Models.Billing
{
    public class StoreDataCleanItem
    {
        public int StoreDataID { get; set; }
        public int ClientID { get; set; }
        public int ItemID { get; set; }
        public DateTime OrderDate { get; set; }
        public int AccountID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public int CategoryID { get; set; }
        public bool RechargeItem { get; set; }
        public DateTime StatusChangeDate { get; set; }
    }
}