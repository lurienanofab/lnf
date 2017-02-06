using System; 
using System.Collections.Generic; 
using System.Text; 

namespace LNF.Repository.Data
{    
    public class Role : IDataItem
    {
        public Role() { }
        public virtual int RoleID { get; set; }
        public virtual string RoleName { get; set; }
    }
}
