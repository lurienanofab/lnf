using System;

namespace LNF.Store
{
    public interface IStoreOrder
    {
        int SOID { get; set; }
        int ClientID { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
        DateTime CreationDate { get; set; }
        string Status { get; set; }
        DateTime StatusChangeDate { get; set; }
        int? InventoryLocationID { get; set; }
    }
}
