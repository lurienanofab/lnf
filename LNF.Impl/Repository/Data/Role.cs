using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class Role : IDataItem
    {
        public Role() { }
        public virtual int RoleID { get; set; }
        public virtual string RoleName { get; set; }
    }
}
