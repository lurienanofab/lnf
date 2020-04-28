using LNF.DataAccess;
using LNF.Inventory;
using System.Collections.Generic;

namespace LNF.Impl.Repository.Inventory
{
    public class Category : ICategory, IDataItem
    {
        public Category()
        {
            Items = new List<InventoryItem>();
            Children = new List<Category>();
        }

        public virtual int CatID { get; set; }
        public virtual int ParentID { get; set; }
        public virtual int HierarchyLevel { get; set; }
        public virtual string CatName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool StoreDisplay { get; set; }
        public virtual bool RequireLocation { get; set; }
        public virtual bool IsRootNode => CatID == ParentID;
        public virtual IList<InventoryItem> Items { get; set; }
        public virtual IList<Category> Children { get; set; }
    }
}
