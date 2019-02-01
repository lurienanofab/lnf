using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public interface IMailGroupUtility
    {
        IEnumerable<IMailGroup> RetrieveAllGroups();
        IMailGroup RetrieveGroup(string groupId);
        IMailGroup RetrieveGroup(int resourceId);
        IMailGroup CreateGroup(string groupId, string groupName, string description, int emailPermission);
    }
}
