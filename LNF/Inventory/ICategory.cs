namespace LNF.Inventory
{
    public interface ICategory
    {
        int CatID { get; set; }
        int ParentID { get; set; }
        int HierarchyLevel { get; set; }
        string CatName { get; set; }
        string Description { get; set; }
        bool Active { get; set; }
        bool StoreDisplay { get; set; }
        bool RequireLocation { get; set; }
        bool IsRootNode { get; }
    }
}