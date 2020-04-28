namespace LNF.Ordering
{
    public class PurchaseOrderAccountItem : IPurchaseOrderAccount
    {
        public int AccountID { get; set; }
        public int ClientID { get; set; }
        public bool Active { get; set; }
    }
}
