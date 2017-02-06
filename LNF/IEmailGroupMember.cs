using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF
{
    public interface IEmailGroupMember
    {
        string Name { get; set; }
        string Email { get; set; }
        IEmailGroup Group { get; }
    }
}
