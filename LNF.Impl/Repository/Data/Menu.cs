using LNF.Data;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class Menu : IDataItem
    {
        public virtual int MenuID { get; set; }
        public virtual int MenuParentID { get; set; }
        public virtual string MenuText { get; set; }
        public virtual string MenuURL { get; set; }
        public virtual string MenuCssClass { get; set; }
        public virtual int MenuPriv { get; set; }
        public virtual bool TopWindow { get; set; }
        public virtual bool NewWindow { get; set; }
        public virtual bool IsLogout { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }

        public virtual bool IsVisible(IPrivileged c)
        {
            return MenuItem.IsVisible(c, MenuPriv);
        }
    }
}
