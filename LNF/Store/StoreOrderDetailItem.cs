namespace LNF.Store
{
    public class StoreOrderDetailItem : IStoreOrderDetail
    {
        public int SODID { get; set; }
        public int SOID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public int PriceID { get; set; }
        public int AccountID { get; set; }
        public string ManufacturerPN { get; set; }
        public string Description { get; set; }
    }
}
