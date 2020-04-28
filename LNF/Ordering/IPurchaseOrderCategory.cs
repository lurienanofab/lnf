namespace LNF.Ordering
{
    public interface IPurchaseOrderCategory
    {
        int CatID { get; set; }
        string CatName { get; set; }
        int ParentID { get; set; }
        bool Active { get; set; }
        string CatNo { get; set; }
    }
}
