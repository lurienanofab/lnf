using LNF.Mail;
using System.Collections.Generic;

namespace LNF.Impl.Mail.Criteria
{
    public abstract class CriteriaBase : IRecipientCriteria
    {
        public NHibernate.ISession Session { get; set; }
        public IProvider Provider { get; set; }

        public abstract IEnumerable<MassEmailRecipient> GetRecipients();

        public abstract string GetGroupName();
    }
}
