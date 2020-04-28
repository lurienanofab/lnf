using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IMailGroupRepository
    {
        IEnumerable<IMailGroup> RetrieveAllGroups();
        IMailGroup RetrieveGroup(string groupId);
        IMailGroup RetrieveGroup(int resourceId);
        IMailGroup CreateGroup(string groupId, string groupName, string description, int emailPermission);
    }
}
