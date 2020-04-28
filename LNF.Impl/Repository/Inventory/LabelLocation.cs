using LNF.DataAccess;

namespace LNF.Impl.Repository.Inventory
{
    public class LabelLocation : IDataItem
    {
        public virtual int LabelLocationID { get; set; }
        public virtual LabelRoom LabelRoom { get; set; }
        public virtual string LocationName { get; set; }
    }
}
