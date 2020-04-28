using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class PhysicalAccessArea : IDataItem
    {
        public virtual int AreaID { get; set; }
        public virtual string AreaName { get; set; }
    }
}
