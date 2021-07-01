using System;
using LNF.Store;

namespace LNF.Impl.Store
{
    public class StoreOrder : IStoreOrder
    {
        public int SOID { get; set; }
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int? InventoryLocationID { get; set; }
    }
}
