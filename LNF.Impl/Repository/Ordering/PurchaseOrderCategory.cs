using LNF.DataAccess;

namespace LNF.Impl.Repository.Ordering
{
    public class PurchaseOrderCategory : IDataItem
    {
        public virtual int CatID { get; set; }
        public virtual string CatName { get; set; }
        public virtual int ParentID { get; set; }
        public virtual bool Active { get; set; }
        public virtual string CatNo { get; set; }

        public virtual bool IsParent()
        {
            return ParentID == 0;
        }

        public virtual string GetFullName()
        {
            return string.Format("{0} - {1}", CatNo, CatName);
        }
    }
}
