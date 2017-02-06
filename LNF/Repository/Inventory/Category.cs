using System.Collections.Generic;

namespace LNF.Repository.Inventory
{
    public class Category : IDataItem
    {
        public Category()
        {
            Items = new List<Item>();
            Children = new List<Category>();
        }

        public virtual int CatID { get; set; }
        public virtual Category Parent { get; set; }
        public virtual int HierarchyLevel { get; set; }
        public virtual string CatName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool StoreDisplay { get; set; }
        public virtual IList<Item> Items { get; set; }
        public virtual IList<Category> Children { get; set; }
        public virtual bool RequireLocation { get; set; }

        public virtual bool IsRootNode
        {
            get { return CatID == Parent.CatID; }
        }
    }
}
