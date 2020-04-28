using System;

namespace LNF.Store
{
    public class StoreOrderItem : IStoreOrder
    {
        public int SOID { get; set; }
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int? InventoryLocationID { get; set; }
        public string AccountName { get; set; }
    }
}
