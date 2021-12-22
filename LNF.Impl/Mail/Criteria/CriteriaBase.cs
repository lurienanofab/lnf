using LNF.Mail;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Mail.Criteria
{
    public abstract class CriteriaBase : RecipientCriteria
    {
        private string _groupName;
        private IEnumerable<MassEmailRecipient> _recipients;

        public NHibernate.ISession Session { get; set; }
        public IProvider Provider { get; set; }

        public override string GroupName
        {
            get
            {
                if (string.IsNullOrEmpty(_groupName))
                    _groupName = GetGroupName();
                return _groupName;
            }
            set
            {
                _groupName = value;
            }
        }

        public override IEnumerable<MassEmailRecipient> Recipients
        {
            get
            {
                if (_recipients == null || _recipients.Count() == 0)
                    _recipients = GetRecipients();
                return _recipients;
            }
            set
            {
                _recipients = value;
            }
        }

        protected abstract string GetGroupName();
        protected abstract IEnumerable<MassEmailRecipient> GetRecipients();
    }
}
