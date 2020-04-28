namespace LNF.Store
{
    public interface IStoreOrderDetail
    {
        int SODID { get; set; }
        int SOID { get; set; }
        int ItemID { get; set; }
        int Quantity { get; set; }
        int PriceID { get; set; }
        int AccountID { get; set; }
        string ManufacturerPN { get; set; }
        string Description { get; set; }
    }
}
