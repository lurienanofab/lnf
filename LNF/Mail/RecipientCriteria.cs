using System.Collections.Generic;

namespace LNF.Mail
{
    public class RecipientCriteria : IRecipientCriteria
    {
        public virtual string GroupName { get; set; }
        public virtual IEnumerable<MassEmailRecipient> Recipients { get; set; }
    }
}
