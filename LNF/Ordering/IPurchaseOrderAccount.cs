namespace LNF.Ordering
{
    public interface IPurchaseOrderAccount
    {
        int AccountID { get; set; }
        int ClientID { get; set; }
        bool Active { get; set; }
    }
}
