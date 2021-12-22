using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IRecipientCriteria
    {
        string GroupName { get; set; }
        IEnumerable<MassEmailRecipient> Recipients { get; set; }
    }
}
