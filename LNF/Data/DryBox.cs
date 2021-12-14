using LNF.DataAccess;

namespace LNF.Data
{
    public class DryBox : IDataItem
    {
        public virtual int DryBoxID { get; set; }
        public virtual string DryBoxName { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
