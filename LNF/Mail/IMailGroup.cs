﻿using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IMailGroup
    {
        string GroupID { get; set; }
        string GroupName { get; set; }
        bool Empty { get; }
        IEnumerable<IMailGroupMember> GetMembers();
    }
}
