using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;

namespace LNF
{
    public interface IEmailGroupUtility
    {
        IEnumerable<IEmailGroup> RetrieveAllGroups();
        IEmailGroup RetrieveGroup(string groupId);
        IEmailGroup RetrieveGroup(Resource resource);
        IEmailGroup CreateGroup(string groupId, string groupName, string description, int emailPermission);
    }
}
