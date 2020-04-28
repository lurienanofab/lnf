using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class InvalidEmailList : IDataItem
    {
        public virtual int EmailID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
