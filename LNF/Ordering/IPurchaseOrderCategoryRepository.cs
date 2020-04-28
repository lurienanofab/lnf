namespace LNF.Ordering
{
    public interface IPurchaseOrderCategoryRepository
    {
        IPurchaseOrderCategory GetParent(int parentId);
    }
}
