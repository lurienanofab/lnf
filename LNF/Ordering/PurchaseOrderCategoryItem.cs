namespace LNF.Ordering
{
    public class PurchaseOrderCategoryItem : IPurchaseOrderCategory
    {
        public int CatID { get; set; }
        public string CatName { get; set; }
        public int ParentID { get; set; }
        public bool Active { get; set; }
        public string CatNo { get; set; }
    }
}
