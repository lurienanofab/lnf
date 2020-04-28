using LNF.DataAccess;

namespace LNF.Impl.Repository.Scheduler
{
    public class ResourceLogProperty : IDataItem
    {
        public virtual int ResourceLogPropertyID { get; set; }
        public virtual LogProperty LogProperty { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual string PropertyType { get; set; }
    }
}