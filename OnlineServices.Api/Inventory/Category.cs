using LNF.Inventory;

namespace OnlineServices.Api.Inventory
{
    public class Category : ICategory
    {
        public int CatID { get; set; }
        public int ParentID { get; set; }
        public int HierarchyLevel { get; set; }
        public string CatName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool StoreDisplay { get; set; }
        public bool RequireLocation { get; set; }
        public bool IsRootNode => CatID == ParentID;
    }
}
