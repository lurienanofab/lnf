using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF
{
    public interface IEmailGroup
    {
        string GroupID { get; set; }
        string GroupName { get; set; }
        bool Empty { get; }
        IEnumerable<IEmailGroupMember> GetMembers();
    }
}
