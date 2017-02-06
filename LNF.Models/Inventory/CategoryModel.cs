namespace LNF.Models.Inventory
{
    public class CategoryModel
    {
        public int CatID { get; set; }
        public int ParentID { get; set; }
        public int HierarchyLevel { get; set; }
        public string CatName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool StoreDisplay { get; set; }
    }
}
